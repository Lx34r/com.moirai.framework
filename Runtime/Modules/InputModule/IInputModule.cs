using UnityEngine;

namespace Moirai.Atropos.Input
{
    public enum EMouseButton { Left = 0, Right = 1, Middle = 2 }

    public interface IInputModule
    {
        /// <summary>是否启用输入</summary>
        bool Enabled { get; set; }

        /// <summary>是否锁定玩家控制器</summary>
        bool LockPlayerController { get; set; }

        /// <summary>是否禁止UI交互</summary>
        bool PreventInteractionUI { get; set; }

        #region Action Value
        
        /// <summary>
        /// 按钮是否被按下
        /// </summary>
        /// <param name="actionName">输入动作名，如果为全称则 actionGroup 置空</param>
        /// <param name="actionGroup">输入动作分组</param>
        /// <returns>是否按下</returns>
        bool GetButtonDown(string actionName, string actionGroup = "");
        
        /// <summary>
        /// 按钮是否被松开
        /// </summary>
        /// <param name="actionName">输入动作名，如果为全称则 actionGroup 置空</param>
        /// <param name="actionGroup">输入动作分组</param>
        /// <returns>是否抬起</returns>
        bool GetButtonUp(string actionName, string actionGroup = "");
        
        /// <summary>
        /// 按钮是否被按住
        /// </summary>
        /// <param name="actionName">输入动作名，如果为全称则 actionGroup 置空</param>
        /// <param name="actionGroup">输入动作分组</param>
        /// <returns>是否按住</returns>
        bool GetButtonPressed(string actionName, string actionGroup = "");

        /// <summary>
        /// 按钮是否被按住
        /// </summary>
        /// <param name="actionName">输入动作名，如果为全称则 actionGroup 置空</param>
        /// <param name="actionGroup">输入动作分组</param>
        /// <returns>是否按住</returns>
        public bool GetButton(string actionName, string actionGroup = "") => GetButtonPressed(actionName, actionGroup);
        
        /// <summary>
        /// 获取指定输入动作的 bool
        /// </summary>
        /// <param name="actionName">输入动作名，如果为全称则 actionGroup 置空</param>
        /// <param name="actionGroup">输入动作分组</param>
        /// <returns></returns>
        bool GetBool(string actionName, string actionGroup = "");

        /// <summary>
        /// 获取指定输入动作的 float
        /// </summary>
        /// <param name="actionName">输入动作名，如果为全称则 actionGroup 置空</param>
        /// <param name="actionGroup">输入动作分组</param>
        /// <returns></returns>
        float GetFloat(string actionName, string actionGroup = "");

        /// <summary>
        /// 获取指定输入动作的 Vector2
        /// </summary>
        /// <param name="actionName">输入动作名，如果为全称则 actionGroup 置空</param>
        /// <param name="actionGroup">输入动作分组</param>
        /// <returns></returns>
        Vector2 GetVector2(string actionName, string actionGroup = "");

        #endregion

        #region Mouse

        /// <summary>
        /// 按下鼠标按键
        /// </summary>
        /// <returns></returns>
        bool GetMouseButtonDown(EMouseButton button);

        /// <summary>
        /// 抬起鼠标按键
        /// </summary>
        /// <returns></returns>
        bool GetMouseButtonUp(EMouseButton button);

        /// <summary>
        /// 按住鼠标按键
        /// </summary>
        /// <returns></returns>
        bool GetMouseButtonPressed(EMouseButton button);

        /// <summary>
        /// 返回鼠标的当前位置
        /// </summary>
        /// <returns></returns>
        Vector2 GetMousePosition();

        /// <summary>
        /// 获取鼠标滚轮滚动值
        /// </summary>
        /// <returns></returns>
        Vector2 GetScrollDelta();

        #endregion

    }
}