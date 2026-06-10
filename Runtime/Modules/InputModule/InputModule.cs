using System;
using Moirai.Atropos.Events;
using Moirai.Atropos.UI;
using UnityEngine;

namespace Moirai.Atropos.Input
{
    /// <summary>
    /// 输入模块。
    /// </summary>
    public sealed class InputModule : Module, IInputModule
    {
        [Flags]
        private enum InputStateFlags
        {
            None = 0,
            LockPlayerController = 1, // 禁止角色控制器移动
            PreventInteractionUI = 2, // 禁止交互UI
        }
        
        private InputStateFlags _inputStateFlags;
        
        private IInputHandler _inputHandler;

        private bool _hasUIModal;

        public bool Enabled { get; set; } = true;

        public bool LockPlayerController
        {
            get => !Enabled || _inputStateFlags.HasFlag(InputStateFlags.LockPlayerController) || _hasUIModal;
            set
            {
                if (value)
                {
                    _inputStateFlags |= InputStateFlags.LockPlayerController;
                }
                else
                {
                    _inputStateFlags &= ~InputStateFlags.LockPlayerController;
                }
                // Log.Info($"[Input] {(value ? "Lock" : "Unlock")} Input[Player]");
            }
        }
        
        public bool PreventInteractionUI
        {
            get => !Enabled || _inputStateFlags.HasFlag(InputStateFlags.PreventInteractionUI);
            set
            {
                if (value)
                {
                    _inputStateFlags |= InputStateFlags.PreventInteractionUI;
                }
                else
                {
                    _inputStateFlags &= ~InputStateFlags.PreventInteractionUI;
                }
                // Log.Info($"[Input] {(value ? "Lock" : "Unlock")} Input[UI]");
            }
        }
        
        #region 实现方法 [IMPLEMENTATION METHODS]

        // Module
        public override void OnInit()
        {
            _inputHandler = InputSettings.InputHandler;
            EventManager.RegisterCallback<MessageEvent>(ResetInput);
            EventManager.RegisterCallback<UIModuleEvent>(RefreshUIModal);
        }
        
        public override void Shutdown()
        {
            EventManager.UnregisterCallback<MessageEvent>(ResetInput);
            EventManager.UnregisterCallback<UIModuleEvent>(RefreshUIModal);
        }
        
        // IInputModule
        public bool GetButtonDown(string actionName, string actionGroup = "") => _inputHandler.GetButtonDown(actionName, actionGroup);
        public bool GetButtonUp(string actionName, string actionGroup = "") => _inputHandler.GetButtonUp(actionName, actionGroup);
        public bool GetButtonPressed(string actionName, string actionGroup = "") => _inputHandler.GetButtonPressed(actionName, actionGroup);
        public bool GetBool(string actionName, string actionGroup = "") => _inputHandler.GetBool(actionName, actionGroup);
        public float GetFloat(string actionName, string actionGroup = "") => _inputHandler.GetFloat(actionName, actionGroup);
        public Vector2 GetVector2(string actionName, string actionGroup = "") => _inputHandler.GetVector2(actionName, actionGroup);
        public bool GetMouseButtonDown(EMouseButton button) => _inputHandler.GetMouseButtonDown(button);
        public bool GetMouseButtonUp(EMouseButton button) => _inputHandler.GetMouseButtonUp(button);
        public bool GetMouseButtonPressed(EMouseButton button) => _inputHandler.GetMouseButtonPressed(button);
        public Vector2 GetMousePosition() => _inputHandler.GetMousePosition();
        public Vector2 GetScrollDelta() => _inputHandler.GetScrollDelta();

        #endregion

        #region 事件 [EVENT]

        private void ResetInput(MessageEvent evt)
        {
            switch (evt.EventType)
            {
                case MessageEventType.ApplicationFocus:
                    Enabled = true;
                    _inputHandler.ResetAllInputStates();
                    break;

                case MessageEventType.NotApplicationFocus:
                    Enabled = false;
                    _inputHandler.ResetAllInputStates();
                    break;
            }
        }

        private void RefreshUIModal(UIModuleEvent evt)
        {
            if (evt.Mode == UIModuleEvent.EMode.Shown || evt.Mode == UIModuleEvent.EMode.Closed)
                _hasUIModal = GameModule.UI.CurrentModal != null;
        }

        #endregion 事件 [EVENT]
    }
}