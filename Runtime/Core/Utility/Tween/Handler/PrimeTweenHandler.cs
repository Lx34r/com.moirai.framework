#if PRIMETWEEN_INSTALLED
using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Moirai.Atropos
{
    /// <summary>
    /// 基于 PrimeTween 实现的补间动画处理器。
    /// </summary>
    /// <remarks>需要手动复制框架下的 <c>Templates~/Plugins/PrimeTween</c> 到项目</remarks>
    [Serializable]
    public sealed partial class PrimeTweenHandler : TweenHandler
    {
        #region 字段

        [Tooltip("Tween的最大容量")]
        [SerializeField] private int m_TweenCapacity = 128;

        [SerializeField] private bool m_WarnEndValueEqualsCurrent;

        // 缓存Tween的字典，键为Tween的ID，值为Tween对象
        private static readonly Dictionary<long, PrimeTween.Tween> s_CacheTweenDic = new Dictionary<long, PrimeTween.Tween>();

        // 临时列表，用于存储需要释放的Tween的ID
        private static readonly List<long> s_TempList = new List<long>();

        #endregion

        #region 辅助方法

        /// <summary>
        /// 缓存Tween对象。
        /// </summary>
        /// <param name="tween">需要缓存的Tween对象。</param>
        private void CacheTween(PrimeTween.Tween tween)
        {
            if (tween.Id <= 0)
            {
                return;
            }

            s_CacheTweenDic.TryAdd(tween.Id, tween);
        }

        /// <summary>
        /// 根据Tween的ID获取Tween对象。
        /// </summary>
        /// <param name="tweenId">Tween的ID。</param>
        /// <returns>对应的Tween对象，如果不存在则返回null。</returns>
        public static PrimeTween.Tween GetTween(long tweenId)
        {
            return s_CacheTweenDic.GetValueOrDefault(tweenId);
        }

        #endregion

        #region 基础方法

        /// <summary>
        /// 初始化Tween配置
        /// </summary>
        protected override void OnInit()
        {
            PrimeTweenConfig.SetTweensCapacity(m_TweenCapacity);
#if PRIME_TWEEN_EXPERIMENTAL
            PrimeTweenConfig.ManualInitialize();
            PrimeTweenConfig.warnEndValueEqualsCurrent = m_WarnEndValueEqualsCurrent;
#endif
            Log.Info("Init PrimeTweenConfig.");
        }

        protected override void Shutdown()
        {
            s_CacheTweenDic.Clear();
        }

        public override void ReleaseUnusedTween()
        {
            s_TempList.Clear();
            foreach (var kvp in s_CacheTweenDic)
            {
                var tween = kvp.Value;
                var tempId = kvp.Key;
                // 如果Tween自己的Id为0，且缓存的Id不等于0；
                if (tween.Id == 0 && tempId != 0)
                {
                    s_TempList.Add(tempId);
                }
                else if (!tween.isAlive)
                {
                    s_TempList.Add(tempId);
                }
            }

            for (int i = 0; i < s_TempList.Count; i++)
            {
                s_CacheTweenDic.Remove(s_TempList[i]);
            }

            s_TempList.Clear();
        }

        public override bool IsTweening(object onTarget)
        {
            return GetTweenCount(onTarget) > 0;
        }

        public override int GetTweenCount(object onTarget)
        {
            return PrimeTween.Tween.GetTweensCount(onTarget);
        }

        public override bool IsAlive(long tweenId)
        {
            if (s_CacheTweenDic.TryGetValue(tweenId, out var tween))
            {
                return tween.isAlive;
            }

            return false;
        }

        public override void Stop(long tweenId)
        {
            if (s_CacheTweenDic.TryGetValue(tweenId, out var tween))
            {
                tween.Stop();
            }
        }

        public override void Complete(long tweenId)
        {
            if (s_CacheTweenDic.TryGetValue(tweenId, out var tween))
            {
                tween.Complete();
            }
        }

        public override int StopAll(object onTarget = null)
        {
            return PrimeTween.Tween.StopAll(onTarget);
        }

        public override int CompleteAll(object onTarget = null)
        {
            return PrimeTween.Tween.CompleteAll(onTarget);
        }
        
        #endregion

        #region Delay

        public override long Delay(float duration, Action onComplete = null, bool useUnscaledTime = false, bool warnIfTargetDestroyed = true)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Delay(duration, onComplete, useUnscaledTime, warnIfTargetDestroyed);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Delay(object target, float duration, Action onComplete = null, bool useUnscaledTime = false, bool warnIfTargetDestroyed = true)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Delay(target, duration, onComplete, useUnscaledTime, warnIfTargetDestroyed);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

        #region Transform 补间 — LocalRotation (Vector3)

        public override long LocalRotation(Transform target, Vector3 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.LocalRotation(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long LocalRotation(Transform target, Vector3 startValue, Vector3 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.LocalRotation(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

        #region Transform 补间 — Scale (float)

        public override long Scale(Transform target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Scale(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Scale(Transform target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Scale(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

        #region Transform 补间 — Rotation (Vector3)

        public override long Rotation(Transform target, Vector3 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Rotation(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Rotation(Transform target, Vector3 startValue, Vector3 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Rotation(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

        #region Transform 补间 — Position

        public override long Position(Transform target, Vector3 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Position(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Position(Transform target, Vector3 startValue, Vector3 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Position(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

        #region Transform 补间 — PositionX / Y / Z

        public override long PositionX(Transform target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.PositionX(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long PositionX(Transform target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.PositionX(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long PositionY(Transform target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.PositionY(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long PositionY(Transform target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.PositionY(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long PositionZ(Transform target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.PositionZ(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long PositionZ(Transform target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.PositionZ(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

        #region Transform 补间 — LocalPosition

        public override long LocalPosition(Transform target, Vector3 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.LocalPosition(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long LocalPosition(Transform target, Vector3 startValue, Vector3 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.LocalPosition(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

        #region Transform 补间 — LocalPositionX / Y / Z

        public override long LocalPositionX(Transform target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.LocalPositionX(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long LocalPositionX(Transform target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.LocalPositionX(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long LocalPositionY(Transform target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.LocalPositionY(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long LocalPositionY(Transform target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.LocalPositionY(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long LocalPositionZ(Transform target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.LocalPositionZ(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long LocalPositionZ(Transform target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.LocalPositionZ(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

        #region Transform 补间 — Rotation (Quaternion)

        public override long Rotation(Transform target, Quaternion endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Rotation(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Rotation(Transform target, Quaternion startValue, Quaternion endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Rotation(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

        #region Transform 补间 — LocalRotation (Quaternion)

        public override long LocalRotation(Transform target, Quaternion endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.LocalRotation(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long LocalRotation(Transform target, Quaternion startValue, Quaternion endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.LocalRotation(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

        #region Transform 补间 — Scale (Vector3)

        public override long Scale(Transform target, Vector3 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Scale(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Scale(Transform target, Vector3 startValue, Vector3 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Scale(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

        #region Transform 补间 — ScaleX / Y / Z

        public override long ScaleX(Transform target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.ScaleX(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long ScaleX(Transform target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.ScaleX(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long ScaleY(Transform target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.ScaleY(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long ScaleY(Transform target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.ScaleY(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long ScaleZ(Transform target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.ScaleZ(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long ScaleZ(Transform target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.ScaleZ(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

        #region SpriteRenderer / Material 补间

        public override long Color(SpriteRenderer target, Color endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Color(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Color(SpriteRenderer target, Color startValue, Color endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Color(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Alpha(SpriteRenderer target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Alpha(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Alpha(SpriteRenderer target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Alpha(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

        #region UI 补间

        public override long UISliderValue(Slider target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UISliderValue(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UISliderValue(Slider target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UISliderValue(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UINormalizedPosition(ScrollRect target, Vector2 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UINormalizedPosition(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UINormalizedPosition(ScrollRect target, Vector2 startValue, Vector2 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UINormalizedPosition(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UIHorizontalNormalizedPosition(ScrollRect target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UIHorizontalNormalizedPosition(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UIHorizontalNormalizedPosition(ScrollRect target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UIHorizontalNormalizedPosition(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0,
                useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UIAnchoredPosition(RectTransform target, Vector2 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UIAnchoredPosition(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UIAnchoredPosition(RectTransform target, Vector2 startValue, Vector2 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UIAnchoredPosition(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UIAnchoredPositionX(RectTransform target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UIAnchoredPositionX(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UIAnchoredPositionX(RectTransform target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UIAnchoredPositionX(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UIAnchoredPositionY(RectTransform target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UIAnchoredPositionY(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UIAnchoredPositionY(RectTransform target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UIAnchoredPositionY(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UIVerticalNormalizedPosition(ScrollRect target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UIVerticalNormalizedPosition(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UIVerticalNormalizedPosition(ScrollRect target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UIVerticalNormalizedPosition(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UIAnchoredPosition3D(RectTransform target, Vector3 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UIAnchoredPosition3D(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UIAnchoredPosition3D(RectTransform target, Vector3 startValue, Vector3 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UIAnchoredPosition3D(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UISizeDelta(RectTransform target, Vector2 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UISizeDelta(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UISizeDelta(RectTransform target, Vector2 startValue, Vector2 endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UISizeDelta(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Color(Graphic target, Color endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Color(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Color(Graphic target, Color startValue, Color endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Color(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long MaterialColor(Material target, Color startValue, Color endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.MaterialColor(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Alpha(CanvasGroup target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Alpha(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Alpha(CanvasGroup target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Alpha(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Alpha(Graphic target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Alpha(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Alpha(Graphic target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Alpha(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UIFillAmount(Image target, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UIFillAmount(target, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long UIFillAmount(Image target, float startValue, float endValue, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            PrimeTween.Tween tween = PrimeTween.Tween.UIFillAmount(target, startValue, endValue, duration, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

        #region Bezier Path

        public override long MoveBezierPath(Transform target, Vector3[] path, float duration, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (path.Length < 2)
            {
                throw new ArgumentException("Path must have at least 2 points.");
            }

            PrimeTween.Tween tween = PrimeTween.Tween.Custom<Transform>(target, 0f, 1f, duration, (transform, t) =>
                {
                    // 计算贝塞尔曲线上的点
                    Vector3 position = CalculateBezierPoint(t, path);
                    transform.position = position;

                    if (Mathf.Approximately(t, 1f))
                    {
                        transform.position = path[^1];
                    }
                }, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0, useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

        #region Custom

        public override long Custom<T>(T target, Vector3 startValue, Vector3 endValue, float duration, Action<T, Vector3> onValueChange, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
            where T : class
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Custom<T>(target, startValue, endValue, duration, onValueChange, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0,
                useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Custom<T>(T target, int startValue, int endValue, float duration, Action<T, int> onValueChange, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
            where T : class
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Custom<T>(target, startValue, endValue, duration, (arg1, f) => { onValueChange?.Invoke(arg1, (int)f); }, GetEase(ease), cycles,
                GetCycleMode(cycleMode), startDelay, 0,
                useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Custom<T>(T target, long startValue, long endValue, float duration, Action<T, long> onValueChange, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
            where T : class
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Custom<T>(target, startValue, endValue, duration, (arg1, f) => { onValueChange?.Invoke(arg1, (long)f); }, GetEase(ease), cycles,
                GetCycleMode(cycleMode), startDelay, 0,
                useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        public override long Custom<T>(T target, float startValue, float endValue, float duration, Action<T, float> onValueChange, TweenUtility.EEase ease = TweenUtility.EEase.Default,
            int cycles = 1, TweenUtility.ECycleMode cycleMode = TweenUtility.ECycleMode.Restart, float startDelay = 0, bool useUnscaledTime = false, Action onComplete = null)
            where T : class
        {
            PrimeTween.Tween tween = PrimeTween.Tween.Custom<T>(target, startValue, endValue, duration, onValueChange, GetEase(ease), cycles, GetCycleMode(cycleMode), startDelay, 0,
                useUnscaledTime);
            CacheTween(tween);
            return tween.Id;
        }

        #endregion

    }
}
#endif