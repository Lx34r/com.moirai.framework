using System;
using UnityEngine;

namespace Moirai.Atropos
{
    /// <summary>
    /// 路径相关的实用函数。
    /// </summary>
    public static class PathUtility
    {
        /// <summary>
        /// 获取符合Unity格式的路径。
        /// </summary>
        /// <param name="path"></param>
        /// <example>D:\Usr\Framework\ -> D:/Usr/Framework/</example>
        public static string FormatToUnityPath(string path) => GetRegularPath(path);
        
        /// <summary>
        /// 获取符合系统文件格式的路径。
        /// </summary>
        /// <param name="path"></param>
        /// <example>D:/Usr/Framework/ -> D:\Usr\Framework\</example>
        public static string FormatToSysFilePath(string path) => FormatUNCPath(path);
        
        /// <summary>
        /// 获取当前绝对路径
        /// </summary>
        /// <returns>path</returns>
        public static string ApplicationPath()
        {
            return System.IO.Path.GetFullPath(".");
        }
        
        /// <summary>
        /// 获取沙盒路径。
        /// </summary>
        /// <param name="filePath">路径。</param>
        /// <returns>沙盒路径。</returns>
        public static string GetPersistentDataPlatformPath(string filePath)
        {
            filePath =
#if UNITY_ANDROID && !UNITY_EDITOR
                Application.dataPath + "!assets" + "/" + filePath;
#else
                Application.streamingAssetsPath + "/" + filePath;
#endif
            return filePath;
        }

        /// <summary>
        /// 获取远程格式的路径（带有file:// 或 http:// 前缀）。
        /// </summary>
        /// <param name="path">原始路径。</param>
        /// <returns>远程格式路径。</returns>
        public static string GetRemotePath(string path)
        {
            string regularPath = GetRegularPath(path);
            if (regularPath == null)
            {
                return null;
            }

            return regularPath.Contains("://") ? regularPath : ("file:///" + regularPath).Replace("file:////", "file:///");
        }

        /// <summary>
        /// 从路径的末尾向前截取指定级别的目录
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="levels"></param>
        /// <returns></returns>
        public static string TruncatePath(string fullPath, int levels)
        {
            for (int i = 0; i < levels; i++)
            {
                fullPath = System.IO.Path.GetDirectoryName(fullPath);
                if (string.IsNullOrEmpty(fullPath))
                    break;
            }

            return fullPath;
        }
        
        /// <summary>
        /// 获取共同的路径；
        /// </summary>
        /// <param name="paths">传入的路径合集</param>
        /// <returns>共同的路径</returns>
        public static string CommonPath(string[] paths)
        {
            var firstPath = paths[0];
            bool isSame = true;
            int index = 0;
            string commonPath = string.Empty;
            while (isSame && index < firstPath.Length)
            {
                for (int i = 1; i < paths.Length && isSame; i++)
                {
                    isSame = firstPath[index] == paths[i][index];
                }

                if (isSame)
                    commonPath += firstPath[index];
                index++;
            }

            return commonPath;
        }

        /// <summary>
        /// 获取规范的路径。
        /// </summary>
        /// <param name="path">要规范的路径。</param>
        /// <returns>规范的路径。</returns>
        /// <example>D:\Usr\Framework\ -> D:/Usr/Framework/</example>
        private static string GetRegularPath(string path)
        {
            if (path == null)
            {
                return null;
            }

            return path.Replace('\\', '/');
        }

        /// <summary>
        /// 标准 Windows 文件路径地址合并；
        /// 返回结果示例：Resources\JsonData\
        /// </summary>
        /// <param name="paths">路径params</param>
        /// <returns>合并的路径</returns>
        // ReSharper disable once InconsistentNaming
        public static string CombineUNCPath(params string[] paths)
        {
            var resultPath = System.IO.Path.Combine(paths);
            resultPath = FormatUNCPath(resultPath);
            return resultPath;
        }
        
        /// <summary>
        /// 格式化UNC地址<br/>
        /// </summary>
        /// <param name="path">需要格式化的地址</param>
        /// <returns>格式化后的UNC地址</returns>
        /// <para>关于UNC的介绍：https://learn.microsoft.com/zh-cn/dotnet/standard/io/file-path-formats#unc-paths</para>
        /// <example>D:/Usr/Framework/ -> D:\Usr\Framework\</example>
        // ReSharper disable once InconsistentNaming
        private static string FormatUNCPath(string path)
        {
            var fmtPath = path.Replace("/", "\\");
            return fmtPath;
        }
        
        /// <summary>
        /// 返回结果示例：github.com/Usr/Framework
        /// </summary>
        /// <param name="paths">路径</param>
        /// <returns>合并的路径</returns>
        public static string CombineURL(params string[] paths)
        {
            var pathResult = System.IO.Path.Combine(paths);
            pathResult = GetRegularPath(pathResult);
            return pathResult;
        }
        
        public static bool IsLegalUri(string uri)
        {
            return !string.IsNullOrEmpty(uri) && uri.Contains("://");
        }
    
        public static bool IsLegalHttpUri(string uri)
        {
            return !string.IsNullOrEmpty(uri) && (uri.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || uri.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
        }
    }
}
