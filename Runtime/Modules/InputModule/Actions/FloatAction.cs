using UnityEngine;

namespace Moirai.Atropos.Input
{
    [System.Serializable]
    public struct FloatAction
    {
        [SerializeField] private float m_Value;

        /// <summary>
        /// 动作的当前值。
        /// </summary>
        public float Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        /// <summary>
        /// 重置该动作的值。
        /// </summary>
        public void Reset()
        {
            m_Value = 0f;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // EDITOR ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

#if UNITY_EDITOR
    
    [UnityEditor.CustomPropertyDrawer(typeof(FloatAction))]
    public class FloatActionEditor : ActionPropertyDrawerBase { }

#endif
}
