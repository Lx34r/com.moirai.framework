using System.IO;
using UnityEditor;

namespace Moirai.Atropos.Editor
{
    public static class MoiraiPackageHelper
    {
        private const string ASMDEF_NAME = "Moirai.Atropos.asmdef";
        private const string ASMDEF_GUID = "24c092aee38482f4e80715eaa8148782";

        /// <summary>
        /// 获取当前 Package 的根目录
        /// </summary>
        /// <returns></returns>
        public static string GetPackageRootPath()
        {
            string asmdefPath = AssetDatabase.GUIDToAssetPath(ASMDEF_GUID);
            string asmdefDir = Path.GetDirectoryName(asmdefPath);

            string relativePath = Path.GetDirectoryName(asmdefDir);

            if (string.IsNullOrEmpty(relativePath))
            {
                Log.Error("Package root path is empty. Check the GUID of {0}.", ASMDEF_NAME);
                return string.Empty;
            }

            return Path.GetFullPath(relativePath);
        }
    }
}