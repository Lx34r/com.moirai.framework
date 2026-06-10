#if ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine;
using System.Collections.Generic;

namespace Moirai.Atropos.Input
{
    /// <summary>
    /// 按照 Unity 的输入管理器规范来实现输入检测功能。
    /// </summary>
    /// <remarks>此方案用于桌面游戏。</remarks>
    public sealed class UnityInputManagerHandler : IInputHandler
    {
        private struct Vector2Action
        {
            public string x;
            public string y;

            public Vector2Action(string x, string y)
            {
                this.x = x;
                this.y = y;
            }
        }

        private readonly Dictionary<string, Vector2Action> _vector2Actions = new Dictionary<string, Vector2Action>();

        public bool GetButtonDown(string actionName, string actionGroup)
        {
            bool output = false;
            try
            {
                output = UnityEngine.Input.GetButtonDown(actionName);
            }
            catch (System.Exception)
            {
                PrintInputWarning(actionName);
            }

            return output;
        }

        public bool GetButtonUp(string actionName, string actionGroup)
        {
            bool output = false;
            try
            {
                output = UnityEngine.Input.GetButtonUp(actionName);
            }
            catch (System.Exception)
            {
                PrintInputWarning(actionName);
            }

            return output;
        }
        
        public bool GetBool(string actionName, string actionGroup = "")
        {
            bool output = false;
            try
            {
                output = UnityEngine.Input.GetButton(actionName);
            }
            catch (System.Exception)
            {
                PrintInputWarning(actionName);
            }

            return output;
        }

        public float GetFloat(string actionName, string actionGroup = "")
        {
            float output = 0f;
            try
            {
                output = UnityEngine.Input.GetAxisRaw(actionName);
            }
            catch (System.Exception)
            {
                PrintInputWarning(actionName);
            }

            return output;
        }

        public Vector2 GetVector2(string actionName, string actionGroup = "")
        {
            // 不被 Unity 的输入管理器正式支持。
            // 例如：“Movement（移动）”会拆分为“Movement X（移动 X 轴）”和“Movement Y（移动 Y 轴）” 

            bool found = _vector2Actions.TryGetValue(actionName, out Vector2Action vector2Action);

            if (!found)
            {
                vector2Action = new Vector2Action(
                    string.Concat(actionName, " X"),
                    string.Concat(actionName, " Y")
                );

                _vector2Actions.Add(actionName, vector2Action);
            }

            Vector2 output = Vector2.zero;
            try
            {
                output = new Vector2(UnityEngine.Input.GetAxisRaw(vector2Action.x), UnityEngine.Input.GetAxisRaw(vector2Action.y));
            }
            catch (System.Exception)
            {
                PrintInputWarning(vector2Action.x, vector2Action.y);
            }

            return output;
        }

        public bool GetMouseButtonPressed(EMouseButton button)
        {
            switch (button)
            {
                case EMouseButton.Right: 
                    return UnityEngine.Input.GetMouseButton(1);
                case EMouseButton.Middle: 
                    return UnityEngine.Input.GetMouseButton(2);
                default:
                    return UnityEngine.Input.GetMouseButton(0);
            }
        }

        public bool GetMouseButtonDown(EMouseButton button)
        {
            switch (button)
            {
                case EMouseButton.Right:
                    return UnityEngine.Input.GetMouseButtonDown(1);
                case EMouseButton.Middle:
                    return UnityEngine.Input.GetMouseButtonDown(2);
                default:
                    return UnityEngine.Input.GetMouseButtonDown(0);
            }
        }
        
        public bool GetMouseButtonUp(EMouseButton button)
        {
            switch (button)
            {
                case EMouseButton.Right:
                    return UnityEngine.Input.GetMouseButtonUp(1);
                case EMouseButton.Middle:
                    return UnityEngine.Input.GetMouseButtonUp(2);
                default:
                    return UnityEngine.Input.GetMouseButtonUp(0);
            }
        }

        public Vector2 GetMousePosition()
        {
            return UnityEngine.Input.mousePosition;
        }

        public Vector2 GetScrollDelta()
        {
            return UnityEngine.Input.mouseScrollDelta;
        }

        public void OnInit()
        {
        }

        public void ResetAllInputStates()
        {
            UnityEngine.Input.ResetInputAxes();
        }

        private void PrintInputWarning(string actionName)
        {
            Debug.LogWarning($"[{actionName}] action not found! Please make sure this action is included in your input settings (axis).");
        }

        private void PrintInputWarning(string actionXName, string actionYName)
        {
            Debug.LogWarning($"[{actionXName}] and/or [{actionYName}] actions not found! Please make sure both of these actions are included in your input settings (axis).");
        }
    }
}
#endif