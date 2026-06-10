#if ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Moirai.Atropos.Input
{
    /// <summary>
    /// 基于 Unity Input System 实现的输入处理器
    /// </summary>
    /// <remarks>在编辑器的 Edit > Project Settings > Input System Package 的中设置</remarks>
    public sealed class UnityInputSystemHandler : IInputHandler
    {
        // 缓存 InputAction 引用提升性能
        private readonly Dictionary<string, InputAction> _inputActionsDictionary = new Dictionary<string, InputAction>();

        public bool GetButtonDown(string actionName, string actionGroup)
        {
            bool output = false;
            
            var action = GetInputAction(actionGroup, actionName);
            if (action != null) output = action.WasPressedThisFrame();
            
            return output;
        }

        public bool GetButtonUp(string actionName, string actionGroup)
        {
            bool output = false;
            
            var action = GetInputAction(actionGroup, actionName);
            if (action != null) output = action.WasReleasedThisFrame();
            
            return output;
        }
        
        public bool GetBool(string actionName, string actionGroup = "")
        {
            bool output = false;
            
            var action = GetInputAction(actionGroup, actionName);
            if (action != null) output = action.IsPressed();
            
            return output;
        }

        public float GetFloat(string actionName, string actionGroup = "")
        {
            float output = 0f;
            
            var action = GetInputAction(actionGroup, actionName);
            if (action != null) output = action.ReadValue<float>();
            
            return output;
        }

        public Vector2 GetVector2(string actionName, string actionGroup = "")
        {
            Vector2 output = Vector2.zero;
            
            var action = GetInputAction(actionGroup, actionName);
            if (action != null) output = action.ReadValue<Vector2>();
            
            return output;
        }

        public bool GetMouseButtonDown(EMouseButton button)
        {
            switch (button)
            {
                case EMouseButton.Middle:
                    return Mouse.current.middleButton.wasPressedThisFrame;
                case EMouseButton.Right:
                    return Mouse.current.rightButton.wasPressedThisFrame;
                default:
                    return Mouse.current.leftButton.wasPressedThisFrame;
            }
        }

        public bool GetMouseButtonUp(EMouseButton button)
        {
            switch (button)
            {
                case EMouseButton.Middle:
                    return Mouse.current.middleButton.wasReleasedThisFrame;
                case EMouseButton.Right:
                    return Mouse.current.rightButton.wasReleasedThisFrame;
                default:
                    return Mouse.current.leftButton.wasReleasedThisFrame;
            }
        }

        public bool GetMouseButtonPressed(EMouseButton button)
        {
            switch (button)
            {
                case EMouseButton.Middle:
                    return Mouse.current.middleButton.isPressed;
                case EMouseButton.Right:
                    return Mouse.current.rightButton.isPressed;
                default:
                    return Mouse.current.leftButton.isPressed;
            }
        }

        public Vector2 GetMousePosition()
        {
            return Mouse.current.position.ReadValue();
        }

        private static readonly Vector2 s_ScalingFactor = new Vector2(0.00833f, 0.00833f); // 1/120
        public Vector2 GetScrollDelta()
        {
            // 新输入系统的 scroll 返回的是 tick（刻度），每滚一格通常是 120
            // 除以 120 是为了与旧系统值范围相似
            return Vector2.Scale(Mouse.current.scroll.ReadValue(), s_ScalingFactor);
        }

        public void OnInit()
        {
            _inputActionsDictionary.Clear();

            for (int i = 0; i < InputSystem.actions.actionMaps.Count; i++)
            {
                InputActionMap actionMap = InputSystem.actions.actionMaps[i];
                for (int j = 0; j < actionMap.actions.Count; j++)
                {
                    InputAction action = actionMap.actions[j];
                    _inputActionsDictionary.Add($"{actionMap.name}/{action.name}", action);
                }
            }
        }

        public void ResetAllInputStates()
        {
            InputSystem.FlushDisconnectedDevices();
            foreach (var device in InputSystem.devices)
            {
                if (device.added) InputSystem.ResetDevice(device);
            }
        }

        private InputAction GetInputAction(string actionGroup, string actionName)
        {
            string fullActionName = string.IsNullOrEmpty(actionGroup) ? actionName : $"{actionGroup}/{actionName}";
            if (!_inputActionsDictionary.TryGetValue(fullActionName, out InputAction action))
            {
                action = InputSystem.actions.FindAction(fullActionName);
                if (action != null)
                {
                    action.Enable();
                    _inputActionsDictionary.Add(fullActionName, action);
                }
                else
                {
                    _inputActionsDictionary.Add(fullActionName, null);
                    Debug.LogWarning($"Action '{fullActionName}' not found! " +
                                     "Please check Input Action Asset configuration.");
                }
                // Debug.Log($"GetInputAction: {fullActionName} - {action}");
            }

            return action;
        }
    }
}
#endif