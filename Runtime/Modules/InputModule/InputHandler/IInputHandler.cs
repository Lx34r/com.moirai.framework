using UnityEngine;

namespace Moirai.Atropos.Input
{
    /// <summary>
    /// 这个抽象类包含了角色大脑所使用的所有输入方法。它是所有可用的输入检测方法的基类。
    /// </summary>
    public interface IInputHandler
    {
        #region Button
        
        /// <summary>
        /// 按钮是否被按下
        /// </summary>
        /// <param name="actionName">输入动作名，如果为全称则 actionGroup 置空</param>
        /// <param name="actionGroup">输入动作分组</param>
        /// <returns>是否按下</returns>
        public bool GetButtonDown(string actionName, string actionGroup);
        
        /// <summary>
        /// 按钮是否被松开
        /// </summary>
        /// <param name="actionName">输入动作名，如果为全称则 actionGroup 置空</param>
        /// <param name="actionGroup">输入动作分组</param>
        /// <returns>是否抬起</returns>
        public bool GetButtonUp(string actionName, string actionGroup);

        /// <summary>
        /// 按钮是否被按住
        /// </summary>
        /// <param name="actionName">输入动作名，如果为全称则 actionGroup 置空</param>
        /// <param name="actionGroup">输入动作分组</param>
        /// <returns>是否按住</returns>
        public bool GetButtonPressed(string actionName, string actionGroup) => GetBool(actionName, actionGroup);
        
        #endregion
        
        #region Action Value

        /// <summary>
        /// 获取指定输入动作的 bool
        /// </summary>
        /// <param name="actionName">输入动作名，如果为全称则 actionGroup 置空</param>
        /// <param name="actionGroup">输入动作分组</param>
        /// <returns></returns>
        public bool GetBool(string actionName, string actionGroup = "");
        
        /// <summary>
        /// 获取指定输入动作的 float
        /// </summary>
        /// <param name="actionName">输入动作名，如果为全称则 actionGroup 置空</param>
        /// <param name="actionGroup">输入动作分组</param>
        /// <returns></returns>
        public float GetFloat(string actionName, string actionGroup = "");
        
        /// <summary>
        /// 获取指定输入动作的 Vector2
        /// </summary>
        /// <param name="actionName">输入动作名，如果为全称则 actionGroup 置空</param>
        /// <param name="actionGroup">输入动作分组</param>
        /// <returns></returns>
        public Vector2 GetVector2(string actionName, string actionGroup = "");
      
        #endregion

        #region Mouse

        /// <summary>
        /// 按下鼠标按键
        /// </summary>
        /// <returns></returns>
        public bool GetMouseButtonDown(EMouseButton button);
        
        /// <summary>
        /// 抬起鼠标按键
        /// </summary>
        /// <returns></returns>
        public bool GetMouseButtonUp(EMouseButton button);
        
        /// <summary>
        /// 按住鼠标按键
        /// </summary>
        /// <returns></returns>
        public bool GetMouseButtonPressed(EMouseButton button);
        
        /// <summary>
        /// 返回鼠标的当前位置
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMousePosition();

        /// <summary>
        /// 获取鼠标滚轮滚动值
        /// </summary>
        /// <returns></returns>
        public Vector2 GetScrollDelta();

        #endregion

        #region Common

        /// <summary>
        /// 初始化
        /// </summary>
        public void OnInit();

        /// <summary>
        /// 清除所有输入状态
        /// </summary>
        public void ResetAllInputStates();

        #endregion

    }
}