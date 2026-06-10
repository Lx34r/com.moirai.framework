using UnityEngine;

namespace Moirai.Atropos
{
    /// <summary>
    /// 图形相关设置。
    /// </summary>
    public partial class GraphicsSettings : ScriptableObject
    {
        #region 设置单例

        private const string SETTINGS_DATA_NAME = "GraphicsSettings";
        private const string SETTINGS_DATA_FILE = "Assets/Settings/Framework/Resources/" + SETTINGS_DATA_NAME + ".asset";
        private static GraphicsSettings s_Instance;

        private static GraphicsSettings Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = Resources.Load<GraphicsSettings>(SETTINGS_DATA_NAME);
                    if (s_Instance == null)
                    {
#if UNITY_EDITOR
                        s_Instance = SettingHelper.LoadSettingSO<GraphicsSettings>(SETTINGS_DATA_FILE);
#else
                        Log.Error($"Could not find Settings at path '{SETTINGS_DATA_FILE} - Create using Tools->Settings->{SETTINGS_DATA_NAME}'");
#endif
                    }
                }
                return s_Instance;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Settings/" + SETTINGS_DATA_NAME, priority = 0)]
        private static void CreateSettings()
        {
            UnityEditor.Selection.activeObject = SettingHelper.LoadSettingSO<GraphicsSettings>(SETTINGS_DATA_FILE);
        }
#endif

        #endregion
    }
}