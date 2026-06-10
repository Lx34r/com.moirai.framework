using UnityEngine;

namespace Moirai.Atropos.Input
{
    [System.Serializable]
    public struct Vector2Action
    {
        [SerializeField] private Vector2 m_Value;

        /// <summary>
        /// 动作的当前值。
        /// </summary>
        public Vector2 Value
        {
            get => m_Value;
            set => m_Value = value;
        }
        
        /// <summary>
        /// 如果值不等于零（例如，当按下方向键时），则返回 <c>true</c>。
        /// </summary>
        public bool Detected => m_Value != Vector2.zero;

        /// <summary>
        /// 如果 x 分量为正，则返回 <c>true</c>。
        /// </summary>
        public bool Right => m_Value.x > 0;

        /// <summary>
        /// 如果 x 分量为负，则返回 <c>true</c>。
        /// </summary>
        public bool Left => m_Value.x < 0;

        /// <summary>
        /// 如果 y 分量为正，则返回 <c>true</c>。
        /// </summary>
        public bool Up => m_Value.y > 0;

        /// <summary>
        /// 如果 y 分量为负，则返回 <c>true</c>。
        /// </summary>
        public bool Down => m_Value.y < 0;
        
        /// <summary>
        /// 重置该动作的值。
        /// </summary>
        public void Reset()
        {
            m_Value = Vector2.zero;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // EDITOR ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

#if UNITY_EDITOR
    
    [UnityEditor.CustomPropertyDrawer(typeof(Vector2Action))]
    public class Vector2ActionEditor : ActionPropertyDrawerBase { }

#endif
}
