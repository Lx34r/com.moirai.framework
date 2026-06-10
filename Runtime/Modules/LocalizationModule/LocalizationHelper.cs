using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Moirai.Atropos.Localization
{
    /// <summary>
    /// 默认本地化辅助器。
    /// </summary>
    public static class LocalizationHelper
    {
        /// <summary>不存在时的默认语言</summary>
        public static readonly Language DefaultLanguage = Language.English;
        
        // 已加载的语言
        private static readonly HashSet<Language> s_LoadedLanguage = new HashSet<Language>();

        // 所有内置语言
        private static readonly Dictionary<string, Language> s_AllBuildInLanguageMap =
            Language.BuiltinLanguages.ToDictionary(_ => _.Name.ToLower(), _ => _);
        // 所有内置语言代码
        private static readonly Dictionary<string, Language> s_AllBuildInLanguageCodeMap =
            Language.BuiltinLanguages.ToDictionary(_ => _.Code.ToLower(), _ => _);

        private static bool s_HasLoggedWarning;

        #region Version 3

        // 预编译正则表达式
        // 使用正则表达式匹配 {l10n:...} 或 {i18n:...} 或 {g11n:...}
        // (l10n|i18n|g11n) 是第一个捕获组，匹配标签类型。
        // (.*?) 是第二个捕获组，匹配文本 ID。
        private static readonly Regex s_LocalizedRegex = new Regex(@"\{(l10n|i18n|g11n):(.*?)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary>
        /// 返回一个本地化字符串，将 <b>{l10n:ID}</b>/<b>{i18n:ID}</b>/<b>{g11n:ID}</b> 替换为本地化条目
        /// </summary>
        /// <param name="format">使用格式更新的字符串</param>
        /// <returns></returns>
        /// <list type="tabel">
        /// <item><term>l10n</term><description>本地化，Localization 缩写</description></item>
        /// <item><term>i18n</term><description>国际化，Internationalization 缩写</description></item>
        /// <item><term>g11n</term><description>全球化，Globalization 缩写</description></item>
        /// </list>
        public static string ResolveLocalizedStrings(string format)
        {
            // todo 编辑器预览
            if (!Application.isPlaying) return format;

            if (string.IsNullOrEmpty(format)) return format;

            if (GameModule.Localization == null)
            {
                if (!s_HasLoggedWarning) Log.Warning($"{nameof(LocalizationModule)} not initialized!");
                s_HasLoggedWarning = true;
                return format;
            }

            var matches = s_LocalizedRegex.Matches(format);
            if (matches.Count == 0) return format;

            StringBuilder result = new StringBuilder(format);
            foreach (Match match in matches)
            {
                string textId = match.Groups[2].Value.Trim(); // LocalizedRegex 的第二个捕获组专门用于匹配文本 ID。

                try
                {
                    if (!GameModule.Localization.Has(textId))
                    {
                        if (Application.isPlaying) Log.Warning($"Text ID: {textId}({match.Groups[1].Value}) not available.");
                        continue;
                    }

                    string replacement = GameModule.Localization.GetTextFromId(textId);
                    // Log.Info($"Resolving localization for ID: {textId}({replacement})");
                    result.Replace(match.Value, replacement);
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to resolve localization for ID: {textId}. Error: {ex}");
                }
            }

            return result.ToString();
        }

        #endregion

        #region Version 2

        private static readonly Regex s_L10NRegex = new Regex(@"\{l10n:(.*?)\}", RegexOptions.IgnoreCase);
        /// <summary>
        /// 返回一个本地化字符串，将 <b>{l10n:ID}</b> 替换为本地化条目
        /// </summary>
        /// <param name="format">使用格式更新的字符串</param>
        /// <returns></returns>
        [Obsolete("Use " + nameof(ResolveLocalizedStrings))]
        public static string ResolveL10NStrings(string format)
        {
            // todo 编辑器预览
            if (!Application.isPlaying) return format;

            if (string.IsNullOrEmpty(format)) return format;

            if (GameModule.Localization == null)
            {
                Log.Error($"{nameof(LocalizationModule)} not initialized!");
                return format;
            }

            // 使用正则表达式匹配 {l10n:...}
            var matches = s_L10NRegex.Matches(format);
            if (matches.Count == 0) return format;

            foreach (Match match in matches)
            {
                string textId = match.Groups[1].Value; // s_L10NRegex 只有一个捕获组用于匹配文本 ID。，

                if (!GameModule.Localization.Has(textId))
                {
                    if (Application.isPlaying) Log.Warning($"Text ID: {textId} not available.");
                    continue;
                }

                string replacement = GameModule.Localization.GetTextFromId(textId);
                format = format.Replace(match.Value, replacement);
            }

            return format;
        }

        #endregion

        #region Version 1

        /// <summary>
        /// 将字符串中的 {l10n:ID} 替换为本地化文本
        /// </summary>
        /// <remarks>如果遇到不存在的 ID，则会中断直接返回</remarks>
        /// <param name="format"></param>
        /// <returns></returns>
        [Obsolete("Use " + nameof(ResolveLocalizedStrings))]
        public static string ResolveL10NString(string format)
        {
            if (string.IsNullOrEmpty(format)) return format;

            if (GameModule.Localization == null)
            {
                Log.Error("LocalizationModule not initialized!");
                return format;
            }

            int i = format.IndexOf("{l10n:", System.StringComparison.OrdinalIgnoreCase);
            if (format.Contains("{l10n:") && format.Substring(i + 1).Contains("}"))
            {
                while (i >= 0)
                {
                    var e = format.IndexOf('}', i + 1);
                    if (e < 0) e = format.Length;

                    var entry = format.Substring(i, e - i + 1);
                    string textId = entry.Substring(6, entry.Length -7);

                    if (!GameModule.Localization.Has(textId))
                    {
                        if (Application.isPlaying) Log.Warning($"Text ID: {textId} not available.");
                        break;
                    }

                    format = format.Replace(entry, GameModule.Localization.GetTextFromId(textId));

                    i = format.IndexOf("{l10n:", System.StringComparison.OrdinalIgnoreCase);
                }
            }

            return format;
        }

        #endregion

        /// <summary>
        /// 注册可用的多语言
        /// </summary>
        /// <param name="str"></param>
        public static void RegisterLanguageMap(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                str = DefaultLanguage.Name.ToLower();
            }

            str = str.ToLower();
            var language = Language.Unspecified;
            if (s_AllBuildInLanguageMap.TryGetValue(str, out var foundByName))
            {
                language = foundByName;
            }
            else if (s_AllBuildInLanguageCodeMap.TryGetValue(str, out var foundByCode))
            {
                language = foundByCode;
            }
            
            if (language != Language.Unspecified && s_LoadedLanguage.Add(language))
            {
                Log.Info($"Registered language[{s_LoadedLanguage.Count}]: {language}");
            }
        }

        /// <summary>
        /// 获取所用可用的多语言
        /// </summary>
        /// <returns></returns>
        public static List<Language> GetAllAvailableLanguages() => s_LoadedLanguage.ToList();
        
        /// <summary>
        /// 根据 名称/Code 获取语言。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="onlySupported">是否只获取支持的语言，<c>false</c>表示仅根据设置获取语言，不关心本地化是否支持</param>
        /// <returns></returns>
        public static Language ToLanguage(string str, bool onlySupported)
        {
            // 处理边界条件：str 为空或 null
            if (string.IsNullOrEmpty(str))
            {
                return DefaultLanguage;
            }
            
            str = str.ToLower();
            Language target = DefaultLanguage;
            // 尝试从语言代码映射中获取语言
            if (s_AllBuildInLanguageCodeMap.TryGetValue(str, out var langFromCode))
            {
                target = langFromCode;
            }
            
            // 尝试从语言名称映射中获取语言
            if (s_AllBuildInLanguageMap.TryGetValue(str, out var langFromName))
            {
                target = langFromName;
            }

            if (!onlySupported) return target;
            
            return s_LoadedLanguage.Contains(target) ? target : DefaultLanguage;
        }
    }
}