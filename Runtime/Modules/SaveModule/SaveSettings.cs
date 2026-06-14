using Sirenix.OdinInspector;
using UnityEngine;

namespace Moirai.Atropos.Save
{
    public class SaveSettings : ScriptableObject
    {
        private enum ESaveType { Binary, BinaryEncrypted, Json, JsonEncrypted }

        [SerializeField] private ESaveType m_SaveType = ESaveType.Binary;
        [ShowIf(nameof(IsEncrypted))]
        // SECURITY: Must be changed to a unique, per-project secret before shipping.
        [SerializeField] private string m_EncryptionKey = "CHANGE_ME_BEFORE_SHIPPING";
        [SerializeField] private string m_SaveFileExtension = ".sav";

        private bool IsEncrypted => m_SaveType == ESaveType.BinaryEncrypted ||  m_SaveType == ESaveType.JsonEncrypted;

        private ISaveHandler _saveHandler = null;
        /// <summary>
        /// 获取/设置当前的保存处理器组件。
        /// </summary>
        public static ISaveHandler SaveHandler
        {
            get
            {
                if (Instance._saveHandler != null) return Instance._saveHandler;

                // 初始化
                switch (Instance.m_SaveType)
                {
                    case ESaveType.Binary:
#pragma warning disable CS0618 // 类型或成员已过时
                        Instance._saveHandler = new BinarySaveHandler();
#pragma warning restore CS0618 // 类型或成员已过时
                        break;

                    case ESaveType.BinaryEncrypted:
#pragma warning disable CS0618 // 类型或成员已过时
                        Instance._saveHandler = new BinaryEncryptedSaveHandler();
#pragma warning restore CS0618 // 类型或成员已过时
                        break;

                    case ESaveType.Json:
                        Instance._saveHandler = new JsonSaveHandler();
                        break;

                    case ESaveType.JsonEncrypted:
                        Instance._saveHandler = new JsonEncryptedSaveHandler();
                        break;
                }

                return Instance._saveHandler;
            }
            set => Instance._saveHandler = value;
        }

        public static string EncryptionKey => Instance.m_EncryptionKey; // TODO 使用用户ID加密？
        public static string SaveFileExtension => Instance.m_SaveFileExtension;

        #region 设置单例

        private const string SETTINGS_DATA_NAME = "SaveSettings";
        private const string SETTINGS_DATA_FILE = "Assets/Settings/Framework/Resources/" + SETTINGS_DATA_NAME + ".asset";
        private static SaveSettings s_Instance;
        private static SaveSettings Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = Resources.Load<SaveSettings>(SETTINGS_DATA_NAME);
                    if (s_Instance == null)
                    {
#if UNITY_EDITOR
                        s_Instance = SettingHelper.LoadSettingSO<SaveSettings>(SETTINGS_DATA_FILE);
#else
                        Log.Error($"Could not find Settings at path '{SETTINGS_DATA_FILE} - Create using Tools->Settings->{SETTINGS_DATA_NAME}'");
#endif
                    }
                }

                return s_Instance;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Settings/" + SETTINGS_DATA_NAME, priority = -470)]
        private static void CreateSettings()
        {
            UnityEditor.Selection.activeObject = SettingHelper.LoadSettingSO<SaveSettings>(SETTINGS_DATA_FILE);
        }
#endif

        #endregion
    }
}