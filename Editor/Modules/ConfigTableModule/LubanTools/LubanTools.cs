using Moirai.Atropos.Editor;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Moirai.Atropos.ConfigTable.Editor
{
    public static class LubanTools
    {
        [MenuItem("Tools/Config/Luban 转表 &X", false, 25)]
        public static void BuildLubanExcel()
        {
            if (!CheckConfigRoot()) return;

            string path = ConfigTableSettings.ConfigRootFullPath + "/gen_code_bin_to_project";
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            path += ".sh";
#elif UNITY_EDITOR_WIN
            path += ".bat";
#endif
            Log.Info("执行转表：{0}", path);
            Application.OpenURL(path);
            // ShellHelper.RunByPath(path);
        }

        [MenuItem("Tools/Config/打开表格目录", false, 26)]
        public static void OpenConfigFolder()
        {
            if (!CheckConfigRoot()) return;

            OpenFolderHelper.Execute(ConfigTableSettings.ConfigRootFullPath);
        }

        private static bool CheckConfigRoot()
        {
            if (Directory.Exists(ConfigTableSettings.ConfigRootFullPath)) return true;

            if (EditorUtility.DisplayDialog("配置目录不存在", $"ConfigRootPath 无效:\n{ConfigTableSettings.ConfigRootFullPath}\n\n是否打开设置界面进行配置？", "打开设置", "取消"))
            {
                Selection.activeObject = ConfigTableSettings.Instance;
            }
            return false;
        }
    }
}