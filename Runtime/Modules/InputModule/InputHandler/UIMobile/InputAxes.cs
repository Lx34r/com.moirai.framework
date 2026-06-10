using UnityEngine;
using UnityEngine.EventSystems;

namespace Moirai.Atropos.Input
{
    /// <summary>
    /// 这个类读取 2D 用户界面（UI）操纵杆的动作，然后将这些值发送到一个移动端输入组件。
    /// </summary>
    [AddComponentMenu("Tools/Input/UI/Input Axes")]
    public class InputAxes : MonoBehaviour, IDragHandler, IEndDragHandler, IUIVector2Action
    {
        // 目标
        [Header("目标 [Targets]")]

        // [SerializeField] private MobileInput m_HorizontalAxisMobileInput = null;
        // [SerializeField] private MobileInput m_VerticalAxisMobileInput = null;

        [SerializeField] private string m_ActionName = "";

        // 属性处理
        [Header("属性处理 [Handles properties]")]

        [SerializeField] private bool m_InvertHorizontal = false;

        [SerializeField] private bool m_InvertVertical = false;

        private enum EDeadZoneMode
        {
            /// <summary>使用圆形区域来确定死区。</summary>
            /// <remarks>输入矢量的大小与死区距离进行比较，如果落在圆形死区内，两个轴都设为0。</remarks>
            Radial,

            /// <summary>使用按轴区域来确定死区。</summary>
            /// <remarks>每个轴的值独立与死区距离进行比较，如果某个轴的绝对值落在死区距离内，该轴设为0。</remarks>
            PerAxis
        }

        [Tooltip ("死区是如何影响输出值的呢？为了更好地可视化死区，可以把 “径向（Radial）” 想象成一个圆形，把 “按轴（PerAxis）” 想象成一个十字形。")]
        [SerializeField] private EDeadZoneMode m_DeadZoneMode = EDeadZoneMode.Radial;

        [Tooltip ("产生非零输出所需的最小幅度（考虑轴的缩放）。幅度低于此值将被视为零。")]
        [SerializeField, Range(0f, 1f)] private float m_DeadZoneDistance = 0.2f;

        [SerializeField] private int m_BoundsRadius = 50;

        // 视觉对象处理
        [Header("视觉对象处理 [Handle visuals]")]

        [SerializeField, Range(2f, 50f)] private float m_ReturnLerpSpeed = 10f;

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        #region IVector2Action

        public string ActionName => m_ActionName;

        public Vector2 Vector2Value
        {
            get => _vector2Value;
            set => _vector2Value = value;
        }

        #endregion

        private Vector2 _vector2Value;
        private Vector2 _virtualPosition = default(Vector2);
        private Vector2 _visiblePosition = default(Vector2);

        private RectTransform _rectTransform = null;

        private Vector2 _origin = Vector2.zero;

        private bool _drag = false;

        private void Awake()
        {
            _virtualPosition = _origin;

            _rectTransform = GetComponent<RectTransform>();
        }


        private void Update()
        {

            // Motion -------------------------------------------------------------------------------------------------     
            if (!_drag)
            {
                _virtualPosition = _visiblePosition;
                _virtualPosition = Vector2.Lerp(_virtualPosition, _origin, m_ReturnLerpSpeed * Time.deltaTime);
            }

            Vector2 delta = _virtualPosition - _origin;


            _visiblePosition = _origin + Vector2.ClampMagnitude(delta, m_BoundsRadius);


            _rectTransform.anchoredPosition = _visiblePosition;

            Vector2 axesValue = (_visiblePosition - _origin) / m_BoundsRadius;

            // Axes ------------------------------------------------------------------------------------------------        

            if (m_DeadZoneMode == EDeadZoneMode.Radial)
            {
                float radius = Vector3.Magnitude(axesValue);

                axesValue.x = radius > m_DeadZoneDistance ? axesValue.x : 0f;
                axesValue.y = radius > m_DeadZoneDistance ? axesValue.y : 0f;
            }
            else
            {
                float absX = Mathf.Abs(axesValue.x);
                float absY = Mathf.Abs(axesValue.y);

                axesValue.x = absX > m_DeadZoneDistance ? axesValue.x : 0f;
                axesValue.y = absY > m_DeadZoneDistance ? axesValue.y : 0f;
            }

            if (m_InvertHorizontal)
                axesValue.x *= -1;

            if (m_InvertVertical)
                axesValue.y *= -1;

            // vector2Action.value = axesValue;
            _vector2Value = axesValue;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _drag = true;

            _virtualPosition += eventData.delta / 2f;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _drag = false;
        }
    }
}