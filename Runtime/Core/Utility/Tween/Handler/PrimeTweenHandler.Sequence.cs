#if PRIMETWEEN_INSTALLED
using System.Collections.Generic;
using PrimeTween;

namespace Moirai.Atropos
{
    public sealed partial class PrimeTweenHandler
    {
        // 缓存Sequence的字典，键为Sequence的ID，值为Sequence对象
        private static readonly Dictionary<long, Sequence> s_CacheSequenceDic = new Dictionary<long, Sequence>();

        // 临时列表，用于存储需要释放的Sequence的ID
        private static readonly List<long> s_TempSequenceList = new List<long>();

        /// <summary>
        /// 根据Sequence的ID获取Sequence对象。
        /// </summary>
        /// <param name="Id">Sequence的ID。</param>
        /// <returns>对应的Sequence对象，如果不存在则返回null。</returns>
        public static Sequence GetSequence(long Id)
        {
            return s_CacheSequenceDic.GetValueOrDefault(Id);
        }

    }
}
#endif