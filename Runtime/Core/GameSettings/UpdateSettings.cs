using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Moirai.Atropos
{
    /// <summary>
    /// 强制更新类型。
    /// </summary>
    public enum EUpdateStyle
    {
        /// <summary>
        /// 强制更新(不更新无法进入游戏。)
        /// </summary>
        Force = 1,

        /// <summary>
        /// 非强制(不更新可以进入游戏。)
        /// </summary>
        Optional = 2,
    }

    /// <summary>
    /// 是否提示更新。
    /// </summary>
    public enum EUpdateNotice
    {
        /// <summary>
        /// 更新存在提示。
        /// </summary>
        Notice = 1,

        /// <summary>
        /// 更新非提示。
        /// </summary>
        NoNotice = 2,
    }

    /// <summary>
    /// WebGL平台下，
    /// StreamingAssets：跳过远程下载资源直接访问StreamingAssets
    /// Remote：访问远程资源
    /// </summary>
    public enum ELoadResWayWebGL
    {
        Remote,
        StreamingAssets,
    }

    // ReSharper disable once InconsistentNaming
    public class UpdateSettings : ScriptableObject
    {
        [Tooltip("项目名称")]
        [SerializeField] private string m_ProjectName = "DEMO";
        /// <summary>项目名称</summary>
        public static string ProjectName => Instance.m_ProjectName;

        [Header("自动同步 [HybridCLRGlobalSettings]")]
        [SerializeField] private List<string> m_HotUpdateAssemblies = new List<string>() { "GameLib.dll", "GameLogic.dll", "GameProto.dll" };
        /// <summary>热更新 dll</summary>
        public static List<string> HotUpdateAssemblies => Instance.m_HotUpdateAssemblies;

        [Header("需要手动设置！")]
        [SerializeField] private List<string> m_AOTMetaAssemblies = new List<string>() { "mscorlib.dll", "System.dll", "System.Core.dll", "UnityEngine.CoreModule.dll", "Moirai.Atropos.dll" ,"UniTask.dll", "YooAsset.dll", "R3.dll", "R3.Unity.dll" };
        /// <summary>补充元数据 dll</summary>
        public static List<string> AOTMetaAssemblies => Instance.m_AOTMetaAssemblies;

        [Tooltip("主业务逻辑 dll")]
        [SerializeField] private string m_LogicMainDllName = "GameLogic.dll";
        /// <summary>主业务逻辑 dll</summary>
        public static string LogicMainDllName => Instance.m_LogicMainDllName;

        [SerializeField] private string m_EntranceClass = "GameLogic.HotfixEntry";
        /// <summary>主业务逻辑入口类</summary>
        public static string EntranceClass => Instance.m_EntranceClass;
        [SerializeField] private string m_EntranceMethod = "Entrance";
        /// <summary>主业务逻辑入口方法</summary>
        public static string EntranceMethod => Instance.m_EntranceMethod;

        [Space]
        [Tooltip("程序集文本资产打包Asset后缀名")]
        [SerializeField] private string m_AssemblyTextAssetExtension = ".bytes";
        /// <summary>程序集文本资产打包Asset后缀名</summary>
        public static string AssemblyTextAssetExtension => Instance.m_AssemblyTextAssetExtension;

        [Tooltip("程序集文本资产资源目录")]
        [SerializeField] private string m_AssemblyTextAssetPath = "AssetRaw/Default/DLL";
        /// <summary>程序集文本资产资源目录</summary>
        public static string AssemblyTextAssetPath => Instance.m_AssemblyTextAssetPath;

        [Header("更新设置")]
        [Tooltip("强制更新类型")]
        [SerializeField] private EUpdateStyle m_UpdateStyle = EUpdateStyle.Force;
        /// <summary>强制更新类型</summary>
        public static EUpdateStyle UpdateStyle => Instance.m_UpdateStyle;

        [Tooltip("是否提示更新")]
        [SerializeField] private EUpdateNotice m_UpdateNotice = EUpdateNotice.Notice;
        /// <summary>是否提示更新</summary>
        public static EUpdateNotice UpdateNotice => Instance.m_UpdateNotice;

        [Tooltip("资源服务器地址")]
        // SECURITY: Use HTTPS in production to prevent man-in-the-middle asset injection.
        [SerializeField] private string m_ResDownLoadPath = "https://your-cdn-domain.com/resources";
        /// <summary>资源服务器地址。</summary>
        private static string ResDownLoadPath => Instance.m_ResDownLoadPath;

        [Tooltip("资源服务备用地址")]
        // SECURITY: Use HTTPS in production to prevent man-in-the-middle asset injection.
        [SerializeField] private string m_FallbackResDownLoadPath = "https://your-cdn-fallback.com/resources";
        /// <summary>资源服务备用地址。</summary>
        private static string FallbackResDownLoadPath => Instance.m_FallbackResDownLoadPath;

        [Header("WebGL设置")]
        [SerializeField] private ELoadResWayWebGL m_LoadResWayWebGL = ELoadResWayWebGL.Remote;
        /// <summary>WebGL平台加载本地资源/加载远程资源。</summary>
        public static ELoadResWayWebGL LoadResWayWebGL => Instance.m_LoadResWayWebGL;

        [Header("构建资源设置")]
        [Tooltip("是否自动将打包资源复制到打包后的 StreamingAssets 地址")]
        [SerializeField] private bool m_IsAutoAssetCopeToBuildAddress = false;
        /// <summary>是否自动将打包资源复制到打包后的 StreamingAssets 地址</summary>
        public static bool IsAutoAssetCopeToBuildAddress => Instance.m_IsAutoAssetCopeToBuildAddress;

        [Tooltip("打包程序资源地址")]
        [SerializeField] private string m_BuildAddress = "../../Builds/Unity_Data/StreamingAssets";
        /// <summary>获取打包程序资源地址</summary>
        public static string BuildAddress => Instance.m_BuildAddress;

        [Tooltip("是否使用可寻址资源代替资源路径（开启此项可以节省运行时清单占用的内存！）")]
        [SerializeField] private bool m_ReplaceAssetPathWithAddress = false;
        /// <summary>获取是否使用可寻址资源代替资源路径</summary>
        /// <remarks>开启此项可以节省运行时清单占用的内存！</remarks>
        public static bool ReplaceAssetPathWithAddress => Instance.m_ReplaceAssetPathWithAddress;

        [Header("内置热更UI自定义")]
        [SerializeField] private string m_UIWindowPath = "UIWindow/";
        public static string UIWindowPath => Instance.m_UIWindowPath;

        /// <summary>是否启用热更新。</summary>
        public static bool Enable
        {
            get
            {
#if ENABLE_HYBRIDCLR
                return true;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// 获取资源下载路径。
        /// </summary>
        public static string GetResDownLoadPath()
        {
            return Path.Combine(ResDownLoadPath, ProjectName, GetPlatformName()).Replace("\\", "/");
        }

        /// <summary>
        /// 获取备用资源下载路径。
        /// </summary>
        public static string GetFallbackResDownLoadPath()
        {
            return Path.Combine(FallbackResDownLoadPath, ProjectName, GetPlatformName()).Replace("\\", "/");
        }
        
        /// <summary>
        /// 获取当前的平台名称。
        /// </summary>
        /// <returns>平台名称。</returns>
        public static string GetPlatformName()
        {
#if UNITY_ANDROID
            return "Android";
#elif UNITY_IOS
            return "IOS";
#elif UNITY_WEBGL
            return "WebGL";
#else
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    return "Windows64";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows64";

                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "MacOS";

                case RuntimePlatform.IPhonePlayer:
                    return "IOS";

                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";

                case RuntimePlatform.PS5:
                    return "PS5";
                default:
                    throw new NotSupportedException($"Platform '{Application.platform.ToString()}' is not supported.");
            }
#endif
        }

        #region 设置单例

        private const string SETTINGS_DATA_NAME = "UpdateSettings";
        private const string SETTINGS_DATA_FILE = "Assets/Settings/Framework/Resources/" + SETTINGS_DATA_NAME + ".asset";
        private static UpdateSettings s_Instance;

        private static UpdateSettings Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = Resources.Load<UpdateSettings>(SETTINGS_DATA_NAME);
                    if (s_Instance == null)
                    {
#if UNITY_EDITOR
                        s_Instance = SettingHelper.LoadSettingSO<UpdateSettings>(SETTINGS_DATA_FILE);
#else
                        Log.Error($"Could not find Settings at path '{SETTINGS_DATA_FILE} - Create using Tools->Settings->{SETTINGS_DATA_NAME}'");
#endif
                    }
                }
                return s_Instance;
            }
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Settings/" + SETTINGS_DATA_NAME, priority = -450)]
        private static void CreateSettings()
        {
            UnityEditor.Selection.activeObject = SettingHelper.LoadSettingSO<UpdateSettings>(SETTINGS_DATA_FILE);
        }
#endif

        #endregion
    }
}