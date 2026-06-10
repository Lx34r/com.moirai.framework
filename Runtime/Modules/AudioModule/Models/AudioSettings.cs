using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Moirai.Atropos.Audio
{
    // ReSharper disable once InconsistentNaming
    public class AudioSettings : ScriptableObject
    {
        [Tooltip("如果不配置 AudioGroupConfigs，则会从 AudioMixer 读取音轨配置")]
        [SerializeField] private AudioMixer m_AudioMixer;
        
        [SerializeField] private AudioGroupConfig[] m_AudioGroupConfigs;

        public static AudioMixer AudioMixer
        {
            get
            {
                // 如果没有配置 audioMixer，则从 Resources 中读取默认 AudioMixer
                if (Instance.m_AudioMixer == null)
                {
                    Instance.m_AudioMixer = Resources.Load<AudioMixer>("AudioMixer");
                }

                return Instance.m_AudioMixer;
            }
        }
        
        public static AudioGroupConfig[] AudioGroupConfigs
        {
            get
            {
                // 如果没有配置 audioGroupConfigs，则从传入的 audioMixer 读取音轨配置
                if (Instance.m_AudioGroupConfigs.Length == 0)
                {
                    var audioMixerGroups = AudioMixer.FindMatchingGroups("Master/");
                    Instance.m_AudioGroupConfigs = new AudioGroupConfig[audioMixerGroups.Length];
                    for (int i = 0; i < audioMixerGroups.Length; i++)
                    {
                        Instance.m_AudioGroupConfigs[i] = new AudioGroupConfig();
                        Instance.m_AudioGroupConfigs[i].AudioMixerGroup = audioMixerGroups[i];
                    
                        Enum.TryParse<AudioTrack>(audioMixerGroups[i].name, out var audioTrack);
                        Instance.m_AudioGroupConfigs[i].AudioTrack = audioTrack;
                    }
                }
                
                return Instance.m_AudioGroupConfigs;
            }
        }

        #region 设置单例

        private const string SETTINGS_DATA_NAME = "AudioSettings";
        private const string SETTINGS_DATA_FILE = "Assets/Settings/Framework/Resources/" + SETTINGS_DATA_NAME + ".asset";
        private static AudioSettings s_Instance;

        private static AudioSettings Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = Resources.Load<AudioSettings>(SETTINGS_DATA_NAME);
                    if (s_Instance == null)
                    {
#if UNITY_EDITOR
                        s_Instance = SettingHelper.LoadSettingSO<AudioSettings>(SETTINGS_DATA_FILE);
#else
                        Log.Error($"Could not find Settings at path '{SETTINGS_DATA_FILE} - Create using Tools->Settings->{SETTINGS_DATA_NAME}'");
#endif
                    }
                }
                return s_Instance;
            }
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Settings/" + SETTINGS_DATA_NAME, priority = -480)]
        private static void CreateSettings()
        {
            UnityEditor.Selection.activeObject = SettingHelper.LoadSettingSO<AudioSettings>(SETTINGS_DATA_FILE);
        }
#endif

        #endregion
    }
}