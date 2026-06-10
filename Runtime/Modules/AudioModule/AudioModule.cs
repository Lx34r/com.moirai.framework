using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using System.Reflection;
#endif
using Moirai.Atropos.Events;
using Moirai.Atropos.Resource;
using Moirai.Atropos.Schedulers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using YooAsset;

namespace Moirai.Atropos.Audio
{
    /// <summary>
    /// 音效管理，为游戏提供统一的音效播放接口。
    /// </summary>
    /// <remarks>场景3D音效挂到场景物件、技能3D音效挂到技能特效上，并在 <see cref="AudioSource"/> 的Output上设置对应分类的 <see cref="AudioMixerGroup"/></remarks>
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class AudioModule : Module, IAudioModule, IUpdateModule
    {
        private AudioGroupConfig[] _audioGroupConfigs;
        private bool _bUnityAudioDisabled;
        private IResourceModule _resourceModule;
        
        // todo 引入 Tween 干掉这些内置自定义过渡协程
        // 音效渐入协程
        private readonly Dictionary<AudioSource, Coroutine> _fadeInSoundCoroutines = new Dictionary<AudioSource, Coroutine>();
        // 音效渐出协程
        private readonly Dictionary<AudioSource, Coroutine> _fadeOutSoundCoroutines = new Dictionary<AudioSource, Coroutine>();
        // Master 音轨过渡协程
        private Coroutine _masterTrackCoroutine;
        // 音轨过渡协程
        private readonly Dictionary<AudioTrack, Coroutine> _fadeTrackCoroutines = new Dictionary<AudioTrack, Coroutine>();
        // 音轨暂停状态
        private readonly Dictionary<AudioTrack, bool> _pausedTracks = new Dictionary<AudioTrack, bool>();

        private AudioMixer _audioMixer;
        public AudioMixer AudioMixer => _audioMixer;

        private Transform _instanceRoot;
        /// <summary>
        /// 实例化根节点。
        /// </summary>
        public Transform InstanceRoot
        {
            get => _instanceRoot;
            set => _instanceRoot = value;
        }
        
        public Dictionary<string, AssetHandle> AssetHandlePool { get; private set; } = new Dictionary<string, AssetHandle>();
        
        #region 音轨状态 [TRACK STATUS]
        
        public AudioCategory[] AudioCategories { get; private set; }
        
        private float _volume = 1f;
        public float MasterVolume
        {
            get
            {
                if (_bUnityAudioDisabled)
                {
                    return 0f;
                }

                return _volume;
            }
            set
            {
                if (_bUnityAudioDisabled || Mathf.Approximately(_volume, value))
                {
                    return;
                }

                _volume = value;
                ApplyMasterVolume();
            }
        }

        private bool _isMuted;
        public bool MasterMute
        {
            get
            {
                if (_bUnityAudioDisabled)
                {
                    return false;
                }

                return _isMuted;
            }
            set
            {
                if (_bUnityAudioDisabled || _isMuted == value)
                {
                    return;
                }
                
                _isMuted = value;
                ApplyMasterVolume();
            }
        }
        
        public void SetMasterSettings()
        {
            SettingUtility.SetFloat(Constant.Setting.AUDIO_MASTER_VOLUME, _volume);
            SettingUtility.SetBool(Constant.Setting.AUDIO_MASTER_MUTED, _isMuted);
        }

        public void LoadMasterSettings()
        {
            _isMuted = SettingUtility.GetBool(Constant.Setting.AUDIO_MASTER_MUTED, false);
            _volume = SettingUtility.GetFloat(Constant.Setting.AUDIO_MASTER_VOLUME, 1f);
            
            ApplyMasterVolume();
        }

        public void RemoveMasterSetting()
        {
            SettingUtility.RemoveSetting(Constant.Setting.AUDIO_MASTER_MUTED);
            SettingUtility.RemoveSetting(Constant.Setting.AUDIO_MASTER_VOLUME);
            
            _isMuted = false;
            _volume = 1f;
            ApplyMasterVolume();
        }
        
        /// <summary>
        /// 应用主音轨（总音量）音量
        /// </summary>
        private void ApplyMasterVolume()
        {
            AudioListener.volume = _isMuted ? 0f : Mathf.Clamp(_volume, 0f, 1f);
        }
        
        public float GetTrackVolume(AudioTrack track)
        {
            if (_bUnityAudioDisabled)
            {
                return 0f;
            }

            foreach (var audioGroupConfig in _audioGroupConfigs)
            {
                if (audioGroupConfig.AudioTrack == track)
                {
                    return audioGroupConfig.Volume;
                }
            }

            return 1f;
        }
        
        public void SetTrackVolume(AudioTrack track, float volume)
        {
            if (_bUnityAudioDisabled) return;

            foreach (var audioGroupConfig in _audioGroupConfigs)
            {
                if (audioGroupConfig.AudioTrack == track)
                {
                    audioGroupConfig.Volume = volume;
                }
            }
        }
        
        public bool GetTrackMute(AudioTrack track)
        {
            if (_bUnityAudioDisabled) return false;

            foreach (var audioGroupConfig in _audioGroupConfigs)
            {
                if (audioGroupConfig.AudioTrack == track)
                {
                    return audioGroupConfig.Mute;
                }
            }

            return false;
        }
        
        public void SetTrackMute(AudioTrack track, bool mute)
        {
            if (_bUnityAudioDisabled) return;

            foreach (var audioGroupConfig in _audioGroupConfigs)
            {
                if (audioGroupConfig.AudioTrack == track)
                {
                    audioGroupConfig.Mute = mute;
                }
            }
        }
        
        #endregion 音轨状态 [TRACK STATUS
        
        #region 模块方法 [MODULE METHOD]

        public override void OnInit()
        {
            if (!Application.isPlaying) return;
            
            _resourceModule = ModuleSystem.GetModule<IResourceModule>();
            
            Initialize();
            
            // Register Events
            EventManager.RegisterCallback<AudioPlayEvent>(OnAudioPlayEvent);
            EventManager.RegisterCallback<AudioModuleEvent>(OnAudioModuleEvent);
            EventManager.RegisterCallback<AudioTrackEvent>(OnAudioTrackEvent);
            EventManager.RegisterCallback<AudioControlEvent>(OnAudioControlEvent);
            EventManager.RegisterCallback<AudioTrackFadeEvent>(OnAudioTrackFadeEvent);
            EventManager.RegisterCallback<AudioFadeEvent>(OnAudioFadeEvent);
            EventManager.RegisterCallback<AllAudiosControlEvent>(OnAllAudiosControlEvent);
            
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // 加载音频设置，必须等一帧设置才能生效
            Scheduler.WaitFrame(1, () =>
            {
                AudioModuleEvent.Trigger(AudioModuleEventType.LoadSettings);
            });
        }
        
        public void Initialize(Transform instanceRoot = null, AudioMixer audioMixer = null, AudioGroupConfig[] audioGroupConfigs = null)
        {
            _instanceRoot = instanceRoot;
            if (_instanceRoot == null)
            {
                _instanceRoot = new GameObject("[AudioModule]").transform;
                _instanceRoot.localScale = Vector3.one;
                UnityEngine.Object.DontDestroyOnLoad(_instanceRoot);
            }

#if UNITY_EDITOR

            if (!_instanceRoot.GetComponent<AudioDebugger>())
            {
                _instanceRoot.gameObject.AddComponent<AudioDebugger>();
            }

            try
            {
                TypeInfo typeInfo = typeof(UnityEngine.AudioSettings).GetTypeInfo();
                PropertyInfo propertyInfo = typeInfo.GetDeclaredProperty("unityAudioDisabled");
                _bUnityAudioDisabled = (bool)propertyInfo.GetValue(null);
                if (_bUnityAudioDisabled)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error($"[AudioModule] Failed to check AudioSettings.unityAudioDisabled via reflection: {e}");
            }
#endif
            
            _audioMixer = audioMixer;
            if (_audioMixer == null)
            {
                _audioMixer = AudioSettings.AudioMixer;
            }
            
            _audioGroupConfigs = audioGroupConfigs;
            if (_audioGroupConfigs == null)
            {
                _audioGroupConfigs = AudioSettings.AudioGroupConfigs;
            }
            
            AudioCategories = new AudioCategory[_audioGroupConfigs.Length];
            for (int i = 0; i < _audioGroupConfigs.Length; i++)
            {
                AudioCategories[i] = new AudioCategory(_audioGroupConfigs[i]);
            }
        }
        
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var audioCategory in AudioCategories)
            {
                if (audioCategory != null)
                {
                    audioCategory.Update(elapseSeconds);
                }
            }
        }
        
        public override void Shutdown()
        {
            if (!Application.isPlaying) return;

            StopAll(fadeoutDuration: 0f);
            CleanAudioPool();
            
            // Unregister Events
            EventManager.UnregisterCallback<AudioPlayEvent>(OnAudioPlayEvent);
            EventManager.UnregisterCallback<AudioModuleEvent>(OnAudioModuleEvent);
            EventManager.UnregisterCallback<AudioTrackEvent>(OnAudioTrackEvent);
            EventManager.UnregisterCallback<AudioControlEvent>(OnAudioControlEvent);
            EventManager.UnregisterCallback<AudioTrackFadeEvent>(OnAudioTrackFadeEvent);
            EventManager.UnregisterCallback<AudioFadeEvent>(OnAudioFadeEvent);
            EventManager.UnregisterCallback<AllAudiosControlEvent>(OnAllAudiosControlEvent);
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        public void Restart()
        {
            if (_bUnityAudioDisabled) return;

            CleanAudioPool();

            foreach (var category in AudioCategories)
            {
                if (category == null) continue;
                
                foreach (var audioAgent in category.AudioAgents.Where(audioAgent => audioAgent != null))
                {
                    audioAgent.Destroy();
                }
            }
            
            Initialize(audioGroupConfigs:_audioGroupConfigs);
        }
        
        #endregion 模块方法 [MODULE METHOD]

        #region 播放音频 [PLAY AUDIO]
        
        public AudioAgent Play(AudioClip clip, AudioPlayOptions options)
        {
            if (_bUnityAudioDisabled) return null;

            var category = AudioCategories.FirstOrDefault(category => category.AudioTrack == options.AudioTrack);
            if (category == null)
            {
                Log.Error($"{options.AudioTrack} is not found in AudioCategories.");
                return null;
            }
            AudioAgent audioAgent = category.GetAvailableAgent(options.DoNotAutoRecycleIfNotDonePlaying);
            if (audioAgent != null)
            {
                audioAgent.Play(clip, options);
                return audioAgent;
            }
            
            return null;
        }

        public AudioAgent Play(AudioClip clip, AudioTrack track, Vector3 location,
            bool loop = false,
            float volume = 1, int id = 0, bool fade = false, float fadeInitialVolume = 0, float fadeDuration = 1,
            Tween fadeTween = null, bool persistent = false, AudioSource recycleAudioSource = null,
            AudioMixerGroup audioGroup = null, float pitch = 1, float panStereo = 0, float spatialBlend = 0,
            bool soloSingleTrack = false, bool soloAllTracks = false, bool autoUnSoloOnEnd = false,
            bool bypassEffects = false,
            bool bypassListenerEffects = false, bool bypassReverbZones = false, int priority = 128,
            float reverbZoneMix = 1,
            float dopplerLevel = 1, int spread = 0, AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic,
            float minDistance = 1, float maxDistance = 500, bool doNotAutoRecycleIfNotDonePlaying = false,
            float playbackTime = 0, float playbackDuration = 0, Transform attachToTransform = null,
            bool useSpreadCurve = false,
            AnimationCurve spreadCurve = null, bool useCustomRolloffCurve = false,
            AnimationCurve customRolloffCurve = null,
            bool useSpatialBlendCurve = false, AnimationCurve spatialBlendCurve = null,
            bool useReverbZoneMixCurve = false, AnimationCurve reverbZoneMixCurve = null,
            float initialDelay = 0f
            )
        {
            var option = new AudioPlayOptions
            {
                Initialized = true,
                            
                AudioTrack = track,
                AudioGroup = audioGroup,
                
                Loop = loop,
                Volume = volume,
                Pitch = pitch,
                
                ID = id,
                
                FadeInOnPlay = fade,
                FadeInInitialVolume = fadeInitialVolume,
                FadeInDuration = fadeDuration,
                FadeInTween = fadeTween,
                
                Persistent = persistent,
                RecycleAudioSource = recycleAudioSource,

                InitialDelay = initialDelay,
                PlaybackTime = playbackTime,
                PlaybackDuration = playbackDuration,
                
                PanStereo = panStereo,
                SpatialBlend = spatialBlend,
                AttachToTransform = attachToTransform,
                
                SoloSingleTrack = soloSingleTrack,
                SoloAllTracks = soloAllTracks,
                AutoUnSoloOnEnd = autoUnSoloOnEnd,
                BypassEffects = bypassEffects,
                BypassListenerEffects = bypassListenerEffects,
                BypassReverbZones = bypassReverbZones,
                Priority = priority,
                ReverbZoneMix = reverbZoneMix,
                
                DopplerLevel = dopplerLevel,
                Location = location,
                Spread = spread,
                RolloffMode = rolloffMode,
                MinDistance = minDistance,
                MaxDistance = maxDistance,
                
                DoNotAutoRecycleIfNotDonePlaying = doNotAutoRecycleIfNotDonePlaying,
                
                UseCustomRolloffCurve = useCustomRolloffCurve,
                CustomRolloffCurve = customRolloffCurve,
                
                UseSpatialBlendCurve = useSpatialBlendCurve,
                SpatialBlendCurve = spatialBlendCurve,
                
                UseReverbZoneMixCurve = useReverbZoneMixCurve,
                ReverbZoneMixCurve = reverbZoneMixCurve,
                
                UseSpreadCurve = useSpreadCurve,
                SpreadCurve = spreadCurve
            };

            return Play(clip, option);
        }

        public AudioAgent Play(string path, AudioPlayOptions options, bool bAsync = false, bool bInPool = false)
        {
            if (_bUnityAudioDisabled) return null;
            
            var category = AudioCategories.FirstOrDefault(category => category.AudioTrack == options.AudioTrack);
            if (category == null)
            {
                Log.Error($"{options.AudioTrack} is not found in AudioCategories.");
                return null;
            }
            AudioAgent audioAgent = category.GetAvailableAgent(options.DoNotAutoRecycleIfNotDonePlaying);
            if (audioAgent != null)
            {
                audioAgent.Load(path, options, bAsync, bInPool);
                return audioAgent;
            }
            
            return null;
        }

        public AudioAgent Play(string path, AudioTrack track, Vector3 location, bool bAsync = false, bool bInPool = false,
            bool loop = false, float volume = 1.0f, int id = 0,
            bool fade = false, float fadeInitialVolume = 0f, float fadeDuration = 1f, Tween fadeTween = null,
            bool persistent = false,
            AudioSource recycleAudioSource = null, AudioMixerGroup audioGroup = null,
            float pitch = 1f, float panStereo = 0f, float spatialBlend = 0.0f,
            bool soloSingleTrack = false, bool soloAllTracks = false, bool autoUnSoloOnEnd = false,
            bool bypassEffects = false, bool bypassListenerEffects = false, bool bypassReverbZones = false,
            int priority = 128, float reverbZoneMix = 1f,
            float dopplerLevel = 1f, int spread = 0, AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic,
            float minDistance = 1f, float maxDistance = 500f,
            bool doNotAutoRecycleIfNotDonePlaying = false, float playbackTime = 0f, float playbackDuration = 0f,
            Transform attachToTransform = null,
            bool useSpreadCurve = false, AnimationCurve spreadCurve = null, bool useCustomRolloffCurve = false,
            AnimationCurve customRolloffCurve = null,
            bool useSpatialBlendCurve = false, AnimationCurve spatialBlendCurve = null,
            bool useReverbZoneMixCurve = false, AnimationCurve reverbZoneMixCurve = null,
            float initialDelay = 0f)
        {
            var option = new AudioPlayOptions
            {
                Initialized = true,
                
                AudioTrack = track,
                AudioGroup = audioGroup,
                
                Loop = loop,
                Volume = volume,
                Pitch = pitch,
                
                ID = id,
                
                FadeInOnPlay = fade,
                FadeInInitialVolume = fadeInitialVolume,
                FadeInDuration = fadeDuration,
                FadeInTween = fadeTween,
                
                Persistent = persistent,
                RecycleAudioSource = recycleAudioSource,
                
                PlaybackTime = playbackTime,
                PlaybackDuration = playbackDuration,
                
                PanStereo = panStereo,
                SpatialBlend = spatialBlend,
                AttachToTransform = attachToTransform,
                
                SoloSingleTrack = soloSingleTrack,
                SoloAllTracks = soloAllTracks,
                AutoUnSoloOnEnd = autoUnSoloOnEnd,
                BypassEffects = bypassEffects,
                BypassListenerEffects = bypassListenerEffects,
                BypassReverbZones = bypassReverbZones,
                Priority = priority,
                ReverbZoneMix = reverbZoneMix,
                
                DopplerLevel = dopplerLevel,
                Location = location,
                Spread = spread,
                RolloffMode = rolloffMode,
                MinDistance = minDistance,
                MaxDistance = maxDistance,
                
                DoNotAutoRecycleIfNotDonePlaying = doNotAutoRecycleIfNotDonePlaying,
                
                UseCustomRolloffCurve = useCustomRolloffCurve,
                CustomRolloffCurve = customRolloffCurve,
                
                UseSpatialBlendCurve = useSpatialBlendCurve,
                SpatialBlendCurve = spatialBlendCurve,
                
                UseReverbZoneMixCurve = useReverbZoneMixCurve,
                ReverbZoneMixCurve = reverbZoneMixCurve,
                
                UseSpreadCurve = useSpreadCurve,
                SpreadCurve = spreadCurve
            };
            
            return Play(path, option, bAsync, bInPool);
        }

        #endregion 播放音频 [PLAY AUDIO]

        #region 音频控制 [AUDIO CONTROLS]
         
        public void Pause(int id)
        {
            if (_bUnityAudioDisabled) return;
            
            foreach (var agent in FindAgentsByID(id).Where(agent => agent.IsPlaying))
            {
                agent.Pause();
            }
        }

        public void UnPause(int id)
        {
            if (_bUnityAudioDisabled) return;
            
            foreach (var agent in FindAgentsByID(id).Where(agent => agent.IsPaused))
            {
                agent.UnPause();
            }
        }
        
        public void Stop(int id, float fadeoutDuration = 0f)
        {
            if (_bUnityAudioDisabled) return;
            
            foreach (var agent in FindAgentsByID(id).Where(agent => agent.IsPlaying || agent.IsPaused))
            {
                agent.Stop(fadeoutDuration);
            }
        }
        
        #endregion 音频控制 [AUDIO CONTROLS]
        
        #region 获取 [FIND]

        public AudioAgent[] FindAgentsByID(int id)
        {
            return (from category in AudioCategories from agent in category.AudioAgents
                where agent.ID == id select agent).ToArray();
        }
        
        public AudioAgent[] FindAgentsByClip(AudioClip clip)
        {
            if (clip == null) return null;

            return (from category in AudioCategories from agent in category.AudioAgents
                where agent.AudioResource.clip == clip select agent).ToArray();
        }

        public int CurrentlyPlayingCount(AudioClip clip)
        {
            if (clip == null) return 0;

            int count = 0;
            foreach (var category in AudioCategories)
            {
                foreach (var agent in category.AudioAgents)
                {
                    if ((agent.AudioResource.clip == clip) && (agent.AudioResource.isPlaying))
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        
        #endregion 获取 [FIND]
        
        #region 音轨控制 [TRACK CONTROLS]

        public void Pause(AudioTrack track)
        {
            if (_bUnityAudioDisabled) return;

            _pausedTracks[track] = true;
            AudioCategories
                .FirstOrDefault(category => category.AudioTrack == track)?.PauseAll();
        }
        
        public void UnPause(AudioTrack track)
        {
            if (_bUnityAudioDisabled) return;

            _pausedTracks[track] = false;
            AudioCategories
                .FirstOrDefault(category => category.AudioTrack == track)?.UnPauseAll();
        }

        public bool IsPaused(AudioTrack track)
        {
            if (_pausedTracks.TryGetValue(track, out bool muted))
            {
                return muted;
            }

            return false;
        }

        public void Stop(AudioTrack track, float fadeoutDuration = 0f)
        {
            if (_bUnityAudioDisabled) return;

            AudioCategories
                .FirstOrDefault(category => category.AudioTrack == track)?.StopAll();
        }
        
        #endregion 音轨控制 [TRACK CONTROLS]
        
        #region 所有音频控制 [ALL AUDIO CONTROLS]

        public void PauseAll()
        {
            if (_bUnityAudioDisabled) return;

            foreach (var category in AudioCategories)
            {
                category?.PauseAll();
            }
        }

        public void UnPauseAll()
        {
            if (_bUnityAudioDisabled) return;

            foreach (var category in AudioCategories)
            {
                category?.UnPauseAll();
            }
        }

        public void StopAll(float fadeoutDuration = 0f)
        {
            if (_bUnityAudioDisabled) return;

            foreach (var category in AudioCategories)
            {
                category?.StopAll();
            }
        }

        public void StopAllButPersistent(float fadeoutDuration = 0f)
        {
            if (_bUnityAudioDisabled) return;
            
            foreach (var category in AudioCategories)
            {
                category?.StopAllButPersistent();
            }
        }
        
        public void StopAllLooping(float fadeoutDuration = 0f)
        {
            if (_bUnityAudioDisabled) return;
            
            foreach (var category in AudioCategories)
            {
                category?.StopAllLooping(fadeoutDuration);
            }
        }
        
        #endregion 所有音频控制 [ALL AUDIO CONTROLS]

        #region 过渡 [FADES]
        
        public void FadeMasterTrack(float duration, float initialVolume = 0f, float finalVolume = 1f, Tween tween = null)
        {
            _masterTrackCoroutine = UnityUtility.StartCoroutine(FadeMasterTrackCoroutine(duration, initialVolume, finalVolume, tween));
        }
        
        /// <summary>
        /// 随着时间的流逝，整个音轨会逐渐淡出
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="initialVolume"></param>
        /// <param name="finalVolume"></param>
        /// <param name="tween"></param>
        /// <returns></returns>        
        private IEnumerator FadeMasterTrackCoroutine(float duration, float initialVolume, float finalVolume, Tween tween)
        {
            float startedAt = GameTime.unscaledTime;
            if (tween == null)
            {
                tween = new Tween(EEaseType.InOutQuart);
            }
            while (GameTime.unscaledTime - startedAt <= duration)
            {
                float elapsedTime = GameTime.unscaledTime - startedAt;
                float newVolume = EaseUtility.Tween(elapsedTime, 0f, duration, initialVolume, finalVolume, tween);
                MasterVolume = newVolume;
                yield return null;
            }
            MasterVolume = finalVolume;
        }
        
        public void StopFadeMasterTrack()
        {
            if (_masterTrackCoroutine != null)
            {
                UnityUtility.StopCoroutine(_masterTrackCoroutine);
                _masterTrackCoroutine = null;
            }
        }
        
        public void FadeTrack(AudioTrack track, float duration, float initialVolume = 0f, float finalVolume = 1f, Tween tween = null)
        {
            Coroutine coroutine = UnityUtility.StartCoroutine(FadeTrackCoroutine(track, duration, initialVolume, finalVolume, tween));
            _fadeTrackCoroutines[track] = coroutine;
        }
        
        /// <summary>
        /// 随着时间的流逝，整个音轨会逐渐淡出
        /// </summary>
        /// <param name="track"></param>
        /// <param name="duration"></param>
        /// <param name="initialVolume"></param>
        /// <param name="finalVolume"></param>
        /// <param name="tween"></param>
        /// <returns></returns>        
        private IEnumerator FadeTrackCoroutine(AudioTrack track, float duration, float initialVolume, float finalVolume, Tween tween)
        {
            float startedAt = GameTime.unscaledTime;
            if (tween == null)
            {
                tween = new Tween(EEaseType.InOutQuart);
            }
            while (GameTime.unscaledTime - startedAt <= duration)
            {
                float elapsedTime = GameTime.unscaledTime - startedAt;
                float newVolume = EaseUtility.Tween(elapsedTime, 0f, duration, initialVolume, finalVolume, tween);
                SetTrackVolume(track, newVolume);
                yield return null;
            }
            SetTrackVolume(track, finalVolume);            
        }
        
        public void StopFadeTrack(AudioTrack track)
        {
            if (_fadeTrackCoroutines.TryGetValue(track, out var outCoroutine))
            {
                UnityUtility.StopCoroutine(outCoroutine);
                _fadeTrackCoroutines.Remove(track);
            }
        }
        
        public void FadeAudio(AudioSource source, float duration, float initialVolume, float finalVolume, Tween tween)
        {
            Coroutine coroutine = UnityUtility.StartCoroutine(FadeAudioCoroutine(source, duration, initialVolume, finalVolume, tween));
            if (initialVolume < finalVolume)
            {
                _fadeInSoundCoroutines[source] = coroutine;	
            }
            else
            {
                _fadeOutSoundCoroutines[source] = coroutine;
            }
        }
        
        /// <summary>
        /// 随着时间的流逝，过渡音频源的音量
        /// </summary>
        /// <param name="source"></param>
        /// <param name="duration"></param>
        /// <param name="initialVolume"></param>
        /// <param name="finalVolume"></param>
        /// <param name="tween"></param>
        /// <returns></returns>
        private IEnumerator FadeAudioCoroutine(AudioSource source, float duration, float initialVolume, float finalVolume, Tween tween)
        {
            float startedAt = GameTime.unscaledTime;
            if (tween == null)
            {
                tween = new Tween(EEaseType.InOutQuart);
            }
            while (GameTime.unscaledTime - startedAt <= duration)
            {
                float elapsedTime = GameTime.unscaledTime - startedAt;
                float newVolume = EaseUtility.Tween(elapsedTime, 0f, duration, initialVolume, finalVolume, tween);
                source.volume = newVolume;
                yield return null;
            }
            source.volume = finalVolume;
            
            if (initialVolume < finalVolume)
            {
                _fadeInSoundCoroutines[source] = null;	
            }
            else
            {
                _fadeOutSoundCoroutines[source] = null;
            }
        }
        
        public void StopFadeAudio(AudioSource source)
        {
            if ((source != null) && (_fadeInSoundCoroutines.TryGetValue(source, out var outCoroutine)))
            {
                if (outCoroutine != null)
                {
                    UnityUtility.StopCoroutine(outCoroutine);
                    _fadeInSoundCoroutines.Remove(source);	
                }
            }
            if ((source != null) && (_fadeOutSoundCoroutines.TryGetValue(source, out outCoroutine)))
            {
                if (outCoroutine != null)
                {
                    UnityUtility.StopCoroutine(outCoroutine);
                    _fadeOutSoundCoroutines.Remove(source);
                }
            }
        }
        
		public bool SoundIsFadingOut(AudioSource source)
		{
			if (_fadeOutSoundCoroutines.TryGetValue(source, out _))
			{
				return (_fadeOutSoundCoroutines[source] != null);	
			}

			return false;
		}

        #endregion 过渡 [FADES]

        #region 资源池 [ASSET POOL]

        public void PutInAudioPool(List<string> list)
        {
            if (_bUnityAudioDisabled) return;

            foreach (string path in list)
            {
                if (AssetHandlePool != null && !AssetHandlePool.ContainsKey(path))
                {
                    AssetHandle assetData = _resourceModule.LoadAssetAsyncHandle<AudioClip>(path);
                    AssetHandlePool?.Add(path, assetData);
                }
            }
        }

        public void RemoveClipFromPool(List<string> list)
        {
            if (_bUnityAudioDisabled) return;

            foreach (string path in list)
            {
                if (AssetHandlePool.ContainsKey(path))
                {
                    AssetHandlePool[path].Dispose();
                    AssetHandlePool.Remove(path);
                }
            }
        }

        public void CleanAudioPool()
        {
            if (_bUnityAudioDisabled) return;

            foreach (var dic in AssetHandlePool)
            {
                dic.Value.Dispose();
            }

            AssetHandlePool.Clear();
        }
        
        #endregion 资源池 [ASSET POOL]
        
        #region 事件方法 [EVENT METHOD]

        /// <summary>
        /// 播放音频事件
        /// </summary>
        /// <param name="evt"></param>
        private void OnAudioPlayEvent(AudioPlayEvent evt)
        {
            if (evt.Clip != null)
            {
                evt.AudioAgent = Play(evt.Clip, evt.Options);
            }
            else if (!string.IsNullOrEmpty(evt.AudioPath))
            {
                evt.AudioAgent = Play(evt.AudioPath, evt.Options, evt.LoadAssetAsync, evt.CacheAssetHandle);
            }
        }
        
        /// <summary>
        /// 游戏音频设置相关事件
        /// </summary>
        /// <param name="evt"></param>
        private void OnAudioModuleEvent(AudioModuleEvent evt)
        {
            switch (evt.EventType)
            {
                case AudioModuleEventType.SetSettings:
                    SetMasterSettings();
                    foreach (var category in AudioCategories) { category.SetSettings(); }
                    break;
                case AudioModuleEventType.LoadSettings:
                    LoadMasterSettings();
                    foreach (var category in AudioCategories) { category.LoadSettings(); }
                    break;
                case AudioModuleEventType.ResetSettings:
                    RemoveMasterSetting();
                    foreach (var category in AudioCategories) { category.RemoveSetting(); }
                    break;
            }
        }
        
        /// <summary>
        /// 音轨事件
        /// </summary>
        /// <param name="evt"></param>
        private void OnAudioTrackEvent(AudioTrackEvent evt)
        {
            if (evt.IsMaster)
            {
                switch (evt.TrackEventType)
                {
                    case AudioTrackEventType.MuteTrack:
                        MasterMute = true;
                        break;
                    case AudioTrackEventType.UnmuteTrack:
                        MasterMute = false;
                        break;
                    case AudioTrackEventType.SetTrackVolume:
                        MasterVolume = evt.Volume;
                        break;
                    case AudioTrackEventType.PauseTrack:
                        PauseAll();
                        break;
                    case AudioTrackEventType.UnPauseTrack:
                        UnPauseAll();
                        break;
                    case AudioTrackEventType.StopTrack:
                        StopAll();
                        break;
                }
            }
            else
            {
                switch (evt.TrackEventType)
                {
                    case AudioTrackEventType.MuteTrack:
                        SetTrackMute(evt.Track, true);
                        break;
                    case AudioTrackEventType.UnmuteTrack:
                        SetTrackMute(evt.Track, false);
                        break;
                    case AudioTrackEventType.SetTrackVolume:
                        SetTrackVolume(evt.Track, evt.Volume);
                        break;
                    case AudioTrackEventType.PauseTrack:
                        Pause(evt.Track);
                        break;
                    case AudioTrackEventType.UnPauseTrack:
                        UnPause(evt.Track);
                        break;
                    case AudioTrackEventType.StopTrack:
                        Stop(evt.Track);
                        break;
                }
            }
        }
        
        /// <summary>
        /// 音频控制事件
        /// </summary>
        /// <param name="evt"></param>
        private void OnAudioControlEvent(AudioControlEvent evt)
        {
            switch (evt.AudioControlEventType)
            {
                case AudioControlEventType.Pause:
                    Pause(evt.AudioID);
                    break;
                case AudioControlEventType.UnPause:
                    UnPause(evt.AudioID);
                    break;
                case AudioControlEventType.Stop:
                    Stop(evt.AudioID);
                    break;
            }
        }
        
        /// <summary>
        /// 音轨过渡事件
        /// </summary>
        /// <param name="evt"></param>
        private void OnAudioTrackFadeEvent(AudioTrackFadeEvent evt)
        {
            if (evt.IsMaster)
            {
                switch (evt.Mode)
                {
                    case AudioTrackFadeEventMode.PlayFade:
                        FadeMasterTrack(evt.FadeDuration, GetTrackVolume(evt.Track), evt.FinalVolume, evt.FadeTween);
                        break;
                    case AudioTrackFadeEventMode.StopFade:
                        StopFadeMasterTrack();
                        break;
                }
            }
            else
            {
                switch (evt.Mode)
                {
                    case AudioTrackFadeEventMode.PlayFade:
                        FadeTrack(evt.Track, evt.FadeDuration, GetTrackVolume(evt.Track), evt.FinalVolume, evt.FadeTween);
                        break;
                    case AudioTrackFadeEventMode.StopFade:
                        StopFadeTrack(evt.Track);
                        break;
                }
            }
        }
        
        /// <summary>
        /// 音频过渡事件
        /// </summary>
        /// <param name="evt"></param>
        private void OnAudioFadeEvent(AudioFadeEvent evt)
        {
            foreach (var agent in FindAgentsByID(evt.SoundID))
            {
                if (agent == null) continue;
                
                switch (evt.Mode)
                {
                    case AudioFadeEventMode.PlayFade:
                        // 避免各种音量过渡冲突
                        agent.CancelFadeIn();
                        StopFadeAudio(agent.AudioResource);
                        FadeAudio(agent.AudioResource, evt.FadeDuration, agent.AudioResource.volume, evt.FinalVolume, evt.FadeTween);
                        break;
                    case AudioFadeEventMode.StopFade:
                        StopFadeAudio(agent.AudioResource);
                        break;
                }
            }
        }
        
        /// <summary>
        /// 全部音频控制事件
        /// </summary>
        /// <param name="evt"></param>
        private void OnAllAudiosControlEvent(AllAudiosControlEvent evt)
        {
            switch (evt.EventType)
            {
                case AllAudiosControlEventType.Pause:
                    PauseAll();
                    break;
                case AllAudiosControlEventType.Play:
                    UnPauseAll();
                    break;
                case AllAudiosControlEventType.Stop:
                    StopAll(fadeoutDuration:AudioAgent.FADEOUT_DEFAULT_DURATION);
                    break;
                case AllAudiosControlEventType.StopAllButPersistent:
                    StopAllButPersistent(fadeoutDuration:AudioAgent.FADEOUT_DEFAULT_DURATION);
                    break;
                case AllAudiosControlEventType.StopAllLooping:
                    StopAllLooping(fadeoutDuration:AudioAgent.FADEOUT_DEFAULT_DURATION);
                    break;
            }
        }
        
        /// <summary>
        /// 释放除了持久性的音频之外的所有音频。
        /// </summary>
        /// <remarks>每次加载新场景时触发</remarks>
        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadSceneMode)
        {
            StopAllButPersistent(fadeoutDuration:AudioAgent.FADEOUT_DEFAULT_DURATION);
        }

        #endregion
    }
}