using System.Collections.Generic;
using UnityEngine;

namespace Moirai.Atropos.Input
{
    /// <summary>
    /// 这个输入处理器实现了针对移动端 UI 的输入检测功能。
    /// </summary>
    public sealed class UIMobileInputHandler : IInputHandler
    {
        private readonly Dictionary<string, InputButton> _inputButtons = new Dictionary<string, InputButton>();
        private readonly Dictionary<string, InputAxes> _inputAxes = new Dictionary<string, InputAxes>();

        private void Awake()
        {
            InputButton[] inputButtonsArray = UnityUtility.FindObjectsByType<InputButton>();
            for (int i = 0; i < inputButtonsArray.Length; i++)
                _inputButtons.Add(inputButtonsArray[i].ActionName, inputButtonsArray[i]);

            InputAxes[] inputAxesArray = UnityUtility.FindObjectsByType<InputAxes>();
            for (int i = 0; i < inputAxesArray.Length; i++)
                _inputAxes.Add(inputAxesArray[i].ActionName, inputAxesArray[i]);
        }

        public bool GetButtonDown(string actionName, string actionGroup)
        {
            throw new System.NotImplementedException();
        }

        public bool GetButtonUp(string actionName, string actionGroup)
        {
            throw new System.NotImplementedException();
        }
        
        public bool GetBool(string actionName, string actionGroup = "")
        {
            bool found = _inputButtons.TryGetValue(actionName, out InputButton inputButton);

            return found ? inputButton.BoolValue : false;
        }

        public float GetFloat(string actionName, string actionGroup = "")
        {
            return 0f;
        }

        public Vector2 GetVector2(string actionName, string actionGroup = "")
        {
            bool found = _inputAxes.TryGetValue(actionName, out InputAxes element);

            return found ? element.Vector2Value : Vector2.zero;
        }
        
        public bool GetMouseButtonDown(EMouseButton button)
        {
            return false;
        }
        
        public bool GetMouseButtonUp(EMouseButton button)
        {
            return false;
        }
        
        public bool GetMouseButtonPressed(EMouseButton button)
        {
            return false;
        }

        public Vector2 GetMousePosition()
        {
            return Vector2.zero;
        }

        public Vector2 GetScrollDelta()
        {
            return Vector2.zero;
        }

        public void OnInit()
        {
        }

        public void ResetAllInputStates()
        {
            foreach (var inputButton in _inputButtons)
            {
                inputButton.Value.BoolValue = false;
            }

            foreach (var inputAxes in _inputAxes)
            {
                inputAxes.Value.Vector2Value = Vector2.zero;
            }
        }
    }
}