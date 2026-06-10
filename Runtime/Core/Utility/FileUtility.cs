using System;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Moirai.Atropos
{
    /// <summary>
    /// 文件相关的实用函数。
    /// </summary>
    public static partial class FileUtility
    {
        /// <summary>
        /// 创建文件实例。
        /// </summary>
        /// <param name="filePath">文件夹路径。</param>
        /// <param name="isCreateDir">是否需要创建不存在的文件夹。</param>
        /// <returns></returns>
        public static bool CreateFile(string filePath, bool isCreateDir = true)
        {
            if (!System.IO.File.Exists(filePath))
            {
                string dir = System.IO.Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    if (isCreateDir)
                    {
                        if (dir != null)
                        {
                            Directory.CreateDirectory(dir);
                        }
                    }
                    else
                    {
                        Log.Error("文件夹不存在 Path=" + dir);
                        return false;
                    }
                }

                System.IO.File.Create(filePath);
            }

            return true;
        }
        
        /// <summary>
        /// 创建文件实例。
        /// </summary>
        /// <param name="filePath">文件夹路径。</param>
        /// <param name="info">文件实例信息。</param>
        /// <param name="isCreateDir">是否需要创建不存在的文件夹。</param>
        /// <returns></returns>
        public static bool CreateFile(string filePath, string info, bool isCreateDir = true)
        {
            StreamWriter sw;
            FileInfo t = new FileInfo(filePath);
            if (!t.Exists)
            {
                string dir = System.IO.Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    if (isCreateDir)
                    {
                        Directory.CreateDirectory(dir);
                    }
                    else
                    {
#if UNITY_EDITOR
                        EditorUtility.DisplayDialog("Tips", "文件夹不存在", "CANCEL");
#endif
                        Log.Error("文件夹不存在 Path=" + dir);
                        return false;
                    }
                }

                sw = t.CreateText();
            }
            else
            {
                sw = t.AppendText();
            }

            sw.WriteLine(info);
            sw.Close();
            sw.Dispose();
            return true;
        }

        public static string Md5ByPathName(string pathName)
        {
            try
            {
                using (FileStream file = new FileStream(pathName, FileMode.Open))
                {
                    using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                    {
                        byte[] retVal = md5.ComputeHash(file);

                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < retVal.Length; i++)
                        {
                            sb.Append(retVal[i].ToString("x2"));
                        }

                        return sb.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to compute MD5 for file '{pathName}': {ex}");
                return "";
            }
        }
        
        /// <summary>
        /// 将字节长度转换为易读的字符串格式，
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <remarks>根据大小自动选择单位（Bytes/KB/MB/GB）。依次判断是否小于1024（Bytes）、1MB（转KB）、1GB（转MB），否则转为GB，均保留两位小数。</remarks>
        public static string GetLengthString(long length)
        {
            if (length < 1024)
            {
                return $"{length.ToString()} Bytes";
            }

            if (length < 1024 * 1024)
            {
                return $"{(length / 1024f):F2} KB";
            }

            return length < 1024 * 1024 * 1024 ? $"{(length / 1024f / 1024f):F2} MB" : $"{(length / 1024f / 1024f / 1024f):F2} GB";
        }

        /// <summary>
        /// 将字节数转换为易读的存储容量单位字符串（如KB、MB等），
        /// </summary>
        /// <param name="byteLength"></param>
        /// <returns></returns>
        /// <remarks>通过逐级比较字节长度与2的幂次方阈值（1024=2^10），选择最合适的单位进行格式化输出，保留两位小数。</remarks>
        public static string GetByteLengthString(long byteLength)
        {
            if (byteLength < 1024L) // 2 ^ 10
            {
                return TextUtility.Format("{0} Bytes", byteLength.ToString());
            }

            if (byteLength < 1048576L) // 2 ^ 20
            {
                return TextUtility.Format("{0} KB", (byteLength / 1024f).ToString("F2"));
            }

            if (byteLength < 1073741824L) // 2 ^ 30
            {
                return TextUtility.Format("{0} MB", (byteLength / 1048576f).ToString("F2"));
            }

            if (byteLength < 1099511627776L) // 2 ^ 40
            {
                return TextUtility.Format("{0} GB", (byteLength / 1073741824f).ToString("F2"));
            }

            if (byteLength < 1125899906842624L) // 2 ^ 50
            {
                return TextUtility.Format("{0} TB", (byteLength / 1099511627776f).ToString("F2"));
            }

            if (byteLength < 1152921504606846976L) // 2 ^ 60
            {
                return TextUtility.Format("{0} PB", (byteLength / 1125899906842624f).ToString("F2"));
            }

            return TextUtility.Format("{0} EB", (byteLength / 1152921504606846976f).ToString("F2"));
        }

        public static string BinToUtf8(byte[] total)
        {
            byte[] result = total;
            if (total[0] == 0xef && total[1] == 0xbb && total[2] == 0xbf)
            {
                // utf8文件的前三个字节为特殊占位符，要跳过
                result = new byte[total.Length - 3];
                System.Array.Copy(total, 3, result, 0, total.Length - 3);
            }

            string utf8string = System.Text.Encoding.UTF8.GetString(result);
            return utf8string;
        }

        /// <summary>
        /// 数据格式转换。
        /// </summary>
        /// <param name="data">数据。</param>
        /// <returns>转换后的数据。</returns>
        public static string FormatData(long data)
        {
            string result = "";
            if (data < 0)
                data = 0;

            if (data > 1024 * 1024)
            {
                result = ((int)(data / (1024 * 1024))).ToString() + "MB";
            }
            else if (data > 1024)
            {
                result = ((int)(data / 1024)).ToString() + "KB";
            }
            else
            {
                result = data + "B";
            }

            return result;
        }

        /// <summary>
        /// 获取文件大小。
        /// </summary>
        /// <param name="path">文件路径。</param>
        /// <returns>文件大小。</returns>
        public static long GetFileSize(string path)
        {
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return file.Length;
            }
        }
    }
}