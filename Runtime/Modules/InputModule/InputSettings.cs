using UnityEngine;

namespace Moirai.Atropos.Input
{
    // ReSharper disable once InconsistentNaming
    public sealed class InputSettings : ScriptableObject
    {
        /// <summary>
        /// 定义了由相关联的输入处理器获取的输入的性质。
        /// </summary>
        private enum EInputType { InputManager, InputSystem, UIMobile }
        
        [Tooltip("输入管理器：Unity 的旧版输入管理器\n\n" +
                 "UI 移动端：它使用场景中的特定 UI 元素（InputButton 和 InputAxes 组件）作为输入。" +
                 "请确保这些元素的“动作名称”与想要触发的角色动作相匹配。")] 
        [SerializeField] private EInputType m_InputType = EInputType.InputSystem;
        
        private IInputHandler _inputHandler = null;
        /// <summary>
        /// 获取/设置当前的输入处理器组件。
        /// </summary>
        public static IInputHandler InputHandler
        {
            get
            {
                if (Instance._inputHandler != null) return Instance._inputHandler;

                // 初始化
                switch (Instance.m_InputType)
                {
                    case EInputType.InputManager:
#if !ENABLE_LEGACY_INPUT_MANAGER
                        Log.Error("Please enable {0} Handling in Project Settings!", Instance.m_InputType);
                        return null;
#else
                        Instance._inputHandler = new UnityInputManagerHandler();
                        Instance._inputHandler.OnInit();
#endif
                        break;

                    case EInputType.InputSystem:
#if !ENABLE_INPUT_SYSTEM
                        Log.Error("Please install {0} package!", Instance.m_InputType);
                        return null;
#else
                        Instance._inputHandler = new UnityInputSystemHandler();
                        Instance._inputHandler.OnInit();
#endif
                        break;

                    case EInputType.UIMobile:
                        Instance._inputHandler = new UIMobileInputHandler();
                        Instance._inputHandler.OnInit();
                        break;
                }

                return Instance._inputHandler;
            }
            set
            {
                Instance._inputHandler = value;
                Instance._inputHandler?.OnInit();
            }
        }

        #region 设置单例

        private const string SETTINGS_DATA_NAME = "InputSettings";
        private const string SETTINGS_DATA_FILE = "Assets/Settings/Framework/Resources/" + SETTINGS_DATA_NAME + ".asset";
        private static InputSettings s_Instance;
        private static InputSettings Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = Resources.Load<InputSettings>(SETTINGS_DATA_NAME);
                    if (s_Instance == null)
                    {
#if UNITY_EDITOR
                        s_Instance = SettingHelper.LoadSettingSO<InputSettings>(SETTINGS_DATA_FILE);
#else
                        Log.Error($"Could not find Settings at path '{SETTINGS_DATA_FILE} - Create using Tools->Settings->{SETTINGS_DATA_NAME}'");
#endif
                    }
                }
                
                return s_Instance;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Settings/" + SETTINGS_DATA_NAME, priority = -461)]
        private static void CreateSettings()
        {
            UnityEditor.Selection.activeObject = SettingHelper.LoadSettingSO<InputSettings>(SETTINGS_DATA_FILE);
        }
#endif

        #endregion
    }
}