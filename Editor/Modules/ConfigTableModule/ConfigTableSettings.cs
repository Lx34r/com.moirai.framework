using System;
using Sirenix.OdinInspector;
using System.IO;
using Moirai.Atropos.Editor;
using UnityEditor;
using UnityEngine;

namespace Moirai.Atropos.ConfigTable.Editor
{
    public sealed class ConfigTableSettings : ScriptableObject
    {
        private bool IsConfigRootValid => Directory.Exists(Application.dataPath + m_ConfigRootRelativePath);

        [InfoBox("@" + nameof(ConfigRootPathInfo))]
        [ReadOnly]
        [SerializeField] private string m_ConfigRootRelativePath = "/../../Config";
        private string ConfigRootPathInfo => $"配置工具的相对根目录\n相对于 '{Application.dataPath}' 文件夹";

        /// <summary>配置表目录的完整路径</summary>
        public static string ConfigRootFullPath => Application.dataPath + Instance.m_ConfigRootRelativePath;

        [Header("配置导出路径")]
        [InfoBox("修改完配置记得手动[更新配置路径]")]

        [FolderPath]
        [SerializeField] private string m_ClientDataOutPutPath = "Assets/AssetRaw/Default/Config/Table";
        private string ClientDataOutPutPath =>
            PathUtility.FormatToUnityPath(Path.GetRelativePath(ConfigRootFullPath, m_ClientDataOutPutPath)) + "/";

        [FolderPath]
        [SerializeField] private string m_ClientCodeOutPutPath = "Assets/Scripts/GameProto";
        private string ClientCodeOutPutPath =>
            PathUtility.FormatToUnityPath(Path.GetRelativePath(ConfigRootFullPath, m_ClientCodeOutPutPath)) + "/";

        #region 初始化配置根目录

        private const string TEMPLATES_RELATIVE_PATH = "Templates~/Config";
        private static string ResolveTemplatesConfigPath()
        {
            string pkgRoot = MoiraiPackageHelper.GetPackageRootPath();
            if (string.IsNullOrEmpty(pkgRoot)) return null;

            return Path.Combine(pkgRoot, TEMPLATES_RELATIVE_PATH).Replace("\\", "/");
        }

        /// <summary>
        /// 将框架内置的 Config 模板复制到用户指定的目录。<br/>
        /// 若选中的目录名不包含 "Config"，则自动在其下创建 Config 子目录。<br/>
        /// 目标路径在 Assets 内时会自动添加 "~" 后缀以避免 Unity 导入。
        /// </summary>
        [Button("生成 Config 到指定目录", ButtonSizes.Large), PropertyOrder(-999f)]
        [HideIf(nameof(IsConfigRootValid))]
        private void CopyTemplatesConfigToTarget()
        {
            string sourcePath = ResolveTemplatesConfigPath();
            if (!Directory.Exists(sourcePath))
            {
                EditorUtility.DisplayDialog("错误", $"模板目录不存在:\n{sourcePath}", "确定");
                return;
            }

            string targetPath = SelectConfigRootRelativePath();
            SetConfigRootRelativePath(targetPath);

            if (string.IsNullOrEmpty(targetPath)) return;

            if (Directory.Exists(targetPath) && Directory.GetFileSystemEntries(targetPath).Length > 0)
            {
                if (!EditorUtility.DisplayDialog("目录非空", $"目标目录已存在文件:\n{targetPath}\n\n是否继续？", "继续", "取消"))
                    return;
            }

            CopyDirectory(sourcePath, targetPath);

            EditorUtility.DisplayDialog("完成", $"生成 Config 到:\n{targetPath}", "确定");
        }

        [Button("重定向 Config 目录", ButtonSizes.Large), PropertyOrder(-998f)]
        [HideIf(nameof(IsConfigRootValid))]
        private void RedirectConfigDirectory()
        {
            SetConfigRootRelativePath(SelectConfigRootRelativePath());
        }

        private const string DEFAULT_CONFIG_FOLDER_NAME = "Config";
        /// <summary>
        /// 弹出目录选择对话框，返回 Config 目录的完整路径。
        /// 若选中目录名不含 "Config"，则自动在其下拼接 Config 子目录。
        /// </summary>
        /// <returns>Config 目录完整路径，若路径在 Assets 内则带 "~" 后缀；用户取消则返回 null。</returns>
        private string SelectConfigRootRelativePath()
        {
            string selectedPath = EditorUtility.SaveFolderPanel("选择 Config 文件夹所在的目录", "", "");
            if (string.IsNullOrEmpty(selectedPath)) return null;

            string targetPath = selectedPath;
            string dirName = Path.GetFileName(selectedPath);

            if (!dirName.Contains(DEFAULT_CONFIG_FOLDER_NAME)) targetPath = Path.Combine(selectedPath, DEFAULT_CONFIG_FOLDER_NAME);

            // 避免被 Unity 读取
            bool insideAssets = targetPath.Contains(Application.dataPath);
            if (insideAssets) targetPath += "~";

            return targetPath;
        }

        /// <summary>
        /// 将完整路径转换为相对于 Application.dataPath 的相对路径，并保存到配置中。
        /// </summary>
        /// <param name="targetPath">Config 目录完整路径（可带 "~" 后缀）。</param>
        private void SetConfigRootRelativePath(string targetPath)
        {
            if (string.IsNullOrEmpty(targetPath)) return;

            bool insideAssets = targetPath.EndsWith("~");

            string targetPathWithoutSuffix = targetPath.TrimEnd('~');
            string relativePath = "/" + PathUtility.FormatToUnityPath(Path.GetRelativePath(Application.dataPath + "/", targetPathWithoutSuffix));
            if (insideAssets) relativePath += "~";
            m_ConfigRootRelativePath = relativePath;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();

            Log.Warning("已自动设置 ConfigRootRelativePath: {0}", relativePath);
        }

        private static void CopyDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);

            foreach (string file in Directory.GetFiles(source))
            {
                if (Path.GetFileName(file) == ".meta") continue;
                string destFile = Path.Combine(destination, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (string dir in Directory.GetDirectories(source))
            {
                if (Path.GetFileName(dir).StartsWith(".")) continue;
                string destDir = Path.Combine(destination, Path.GetFileName(dir));
                CopyDirectory(dir, destDir);
            }
        }

        [Button("更新配置路径", ButtonSizes.Large)]
        [ShowIf(nameof(IsConfigRootValid))]
        private void UpdateGenerateConfigSettings()
        {
            if (!IsConfigRootValid)
            {
                Log.Error("配置表路径无效，请生成或重定向指定目录。");
                return;
            }

            string configRoot = ConfigRootFullPath;

            UpdatePathExportConf(configRoot);
            UpdateConfigTableModuleInit(configRoot);

            Log.Info("已更新 path_export.conf 和 ConfigTableModule_Init.cs");
        }

        private void UpdatePathExportConf(string configRoot)
        {
            string confPath = Path.Combine(configRoot, "path_export.conf");
            if (!File.Exists(confPath))
            {
                Log.Warning("path_export.conf 不存在: {0}", confPath);
                return;
            }

            string clientDataOutPutPath = ClientDataOutPutPath;
            string clientCodeOutPutPath = ClientCodeOutPutPath;

            string content = File.ReadAllText(confPath);
            content = ReplaceConfValue(content, "DATA_OUTPUT_PATH_CLIENT", clientDataOutPutPath);
            content = ReplaceConfValue(content, "CODE_OUTPUT_PATH_CLIENT", clientCodeOutPutPath + "Gen/");
            content = ReplaceConfValue(content, "CONFIG_SCRIPT_TARGET", clientCodeOutPutPath + "ConfigTableModule.cs");
            // ReSharper disable once StringLiteralTypo
            content = ReplaceConfValue(content, "CONFIGINIT_SCRIPT_TARGET", clientCodeOutPutPath + "ConfigTableModule_Init.cs");
            // ReSharper disable once StringLiteralTypo
            content = ReplaceConfValue(content, "EXTERNALTYPEUTIL_SCRIPT_TARGET", clientCodeOutPutPath + "ExternalTypeUtil.cs");
            File.WriteAllText(confPath, content);
        }

        private void UpdateConfigTableModuleInit(string configRoot)
        {
            string initPath = Path.Combine(configRoot, "CustomTemplate", "ConfigTableModule_Init.cs");
            if (!File.Exists(initPath))
            {
                Log.Warning("ConfigTableModule_Init.cs 不存在: {0}", initPath);
                return;
            }

            string configPath = ExtractUnityAssetPath(ClientDataOutPutPath);
            if (string.IsNullOrEmpty(configPath))
            {
                Log.Warning("无法从 ClientDataOutPutPath 提取 Unity 资源路径: {0}", ClientDataOutPutPath);
                return;
            }

            string content = File.ReadAllText(initPath);
            string target = $"private const string CONFIG_PATH = \"{configPath}\";";
            if (content.Contains(target))
                return;

            content = ReplaceConstString(content, "CONFIG_PATH", configPath);
            File.WriteAllText(initPath, content);
        }

        private static string ExtractUnityAssetPath(string relativePath)
        {
            const string marker = "Assets/";
            int idx = relativePath.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return null;
            string assetPath = relativePath.Substring(idx);
            if (!assetPath.EndsWith("/")) assetPath += "/";
            return assetPath;
        }

        private static string ReplaceConfValue(string content, string key, string newValue)
        {
            string[] lines = content.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string trimmed = lines[i].TrimStart();
                if (trimmed.StartsWith(key + "="))
                {
                    lines[i] = key + "=" + newValue;
                    break;
                }
            }
            return string.Join("\n", lines);
        }

        private static string ReplaceConstString(string content, string constName, string newValue)
        {
            string search = $"private const string {constName} = \"";
            int idx = content.IndexOf(search);
            if (idx < 0) return content;

            int valueStart = idx + search.Length;
            int valueEnd = content.IndexOf('"', valueStart);
            if (valueEnd < 0) return content;

            return content.Substring(0, valueStart) + newValue + content.Substring(valueEnd);
        }

        #endregion

        #region 设置单例

        private const string SETTINGS_DATA_NAME = "ConfigTableSettings";
        private const string SETTINGS_DATA_FILE = "Assets/Settings/Framework/Editor/" + SETTINGS_DATA_NAME + ".asset";
        private static ConfigTableSettings s_Instance;
        internal static ConfigTableSettings Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = SettingHelper.LoadSettingSO<ConfigTableSettings>(SETTINGS_DATA_FILE);
                }

                return s_Instance;
            }
        }

        [MenuItem("Tools/Settings/" + SETTINGS_DATA_NAME, priority = -501)]
        private static void CreateSettings()
        {
            Selection.activeObject = SettingHelper.LoadSettingSO<ConfigTableSettings>(SETTINGS_DATA_FILE);
        }

        #endregion
    }
}