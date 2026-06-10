using System.Collections.Generic;
using Moirai.Atropos.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Moirai.Atropos.Input.Prompts
{
    /// <summary>
    /// InputSystem 的设备提示设置
    /// </summary>
    public class InputSystemDevicePromptSettings : ScriptableObject
    {
        [Tooltip("文本替换要考虑的所有动作列表")]
        [ListDrawerSettings(ShowFoldout = true)]
        [SerializeField] private List<InputActionAsset> m_InputActionAssets;
        public List<InputActionAsset> InputActionAssets => m_InputActionAssets;

        [Tooltip("默认使用的字形集")]
        [SerializeField] private GlyphCollection m_GlyphCollection;
        public GlyphCollection GlyphCollection => m_GlyphCollection;
        
        [Tooltip("按下按钮之前提示显示的优先级")]
        [ListDrawerSettings(ShowFoldout = true)]
        [SerializeField] private List<InputDeviceType> m_DefaultDevicePriority = new List<InputDeviceType>
        {
            InputDeviceType.GamePad,
            InputDeviceType.Keyboard,
            InputDeviceType.Mouse
        };
        public List<InputDeviceType> DefaultDevicePriority => m_DefaultDevicePriority;
        
        [Tooltip("特定于平台的设备提示的可选覆盖。在这些平台上，将仅使用指定的输入设置，而不是动态的。")]
        [ListDrawerSettings(ShowFoldout = true)] 
        [SerializeField] private List<PlatformInputOverride> m_RuntimePlatformsOverride = new List<PlatformInputOverride>();
        public List<PlatformInputOverride> RuntimePlatformsOverride => m_RuntimePlatformsOverride;

        [Header("TMP Settings")]
        [Tooltip("额外富文本后缀标记。例如：对输入 sprite 进行重新着色，只有在 sprite 标签中添加 tint=1 时才生效，即<sprite=... tint=1>")]
        [TextAreaResizable]
        [SerializeField] private string m_RichTextTags = "";
        /// <summary>
        /// 用于自定义富文本格式的标记。
        /// </summary>
        /// <remarks>
        /// 此字段可用于定义可与 PromptSpriteFormatter 结合使用的其他富文本标记。
        /// </remarks>
        public string RichTextTags => m_RichTextTags;

        /// <summary>
        /// 用于标识替换图标占位符的起止
        /// </summary>
        public const string OPEN_TAG = "{action:";
        public const string CLOSE_TAG = "}";
        
        /// <summary>
        /// 用于表示 sprite 在 <see cref="m_PromptSpriteFormatter"/> 中的占位符
        /// </summary>
        public const string PROMPT_SPRITE_FORMATTER_SPRITE_PLACEHOLDER = "{SPRITE}";

        [Tooltip("图片格式化富文本。例如“<size=200%>{SPRITE}</size>”")]
        [TextAreaResizable]
        [SerializeField] private string m_PromptSpriteFormatter = PROMPT_SPRITE_FORMATTER_SPRITE_PLACEHOLDER;
        /// <summary>
        /// 用于向从 <see cref="InputDevicePromptSystem.InsertPromptSprites"/> 返回的字符串添加额外富文本的格式化程序
        /// <example>
        /// TMP 支持的富文本格式：https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.2/manual/RichText.html
        /// <br/><br/>- 未格式化
        /// <![CDATA[
        /// {SPRITE} = "<sprite="PS5_Prompts" sprite="ps5_button_cross">"
        /// ]]>
        /// <br/><br/>- 输出双倍大小
        /// <![CDATA[
        /// <size=200%>{SPRITE}</size> = "<size=200%><sprite="PS5_Prompts" sprite="ps5_button_cross"></size>"
        /// ]]>
        /// <br/><br/>- 修改垂直位置
        /// <![CDATA[
        /// <voffset=-3px>{SPRITE}</voffset> = "<voffset=-3px><sprite="PS5_Prompts" sprite="ps5_button_cross"></voffset>"
        /// ]]>
        /// </example>
        /// </summary>
        public string PromptSpriteFormatter => m_PromptSpriteFormatter;
        
        [System.Serializable]
        public class PlatformInputOverride
        {
            public RuntimePlatform platform;
            public GlyphMap devicePromptData;
        }

        #region 设置单例

        private const string SETTINGS_DATA_NAME = "InputSystemDevicePromptSettings";
        private const string SETTINGS_DATA_FILE = "Assets/Settings/InputSystem/Resources/" + SETTINGS_DATA_NAME + ".asset";
        private static InputSystemDevicePromptSettings s_Instance;
        public static InputSystemDevicePromptSettings Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = Resources.Load<InputSystemDevicePromptSettings>(SETTINGS_DATA_NAME);
                    if (s_Instance == null)
                    {
#if UNITY_EDITOR
                        s_Instance = SettingHelper.LoadSettingSO<InputSystemDevicePromptSettings>(SETTINGS_DATA_FILE);
#else
                        Log.Error($"Could not find Settings at path '{SETTINGS_DATA_FILE} - Create using Tools->Settings->{SETTINGS_DATA_NAME}'");
#endif
                    }
                }
                return s_Instance;
            }
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Settings/" + SETTINGS_DATA_NAME, priority = -460)]
        private static void CreateSettings()
        {
            UnityEditor.Selection.activeObject = SettingHelper.LoadSettingSO<InputSystemDevicePromptSettings>(SETTINGS_DATA_FILE);
        }
#endif

        #endregion
    }
}