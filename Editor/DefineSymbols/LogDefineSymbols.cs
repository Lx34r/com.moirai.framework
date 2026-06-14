using UnityEditor;

namespace Moirai.Atropos.Editor
{
    /// <summary>
    /// 日志脚本宏定义操作类。
    /// </summary>
    public static class LogDefineSymbols
    {
        private const string LOG_ALL_SYMBOL = "LOG_ALL";
        private const string LOG_DEBUG_AND_ABOVE_SYMBOL = "LOG_DEBUG_AND_ABOVE";
        private const string LOG_INFO_AND_ABOVE_SYMBOL = "LOG_INFO_AND_ABOVE";
        private const string LOG_WARNING_AND_ABOVE_SYMBOL = "LOG_WARNING_AND_ABOVE";
        private const string LOG_ERROR_AND_ABOVE_SYMBOL = "LOG_ERROR_AND_ABOVE";
        private const string LOG_FATAL_AND_ABOVE_SYMBOL = "LOG_FATAL_AND_ABOVE";
        private const string LOG_DEBUG_ENABLE_SYMBOL = "LOG_DEBUG_ENABLE";
        private const string LOG_INFO_ENABLE_SYMBOL = "LOG_INFO_ENABLE";
        private const string LOG_WARNING_ENABLE_SYMBOL = "LOG_WARNING_ENABLE";
        private const string LOG_ERROR_ENABLE_SYMBOL = "LOG_ERROR_ENABLE";
        private const string LOG_FATAL_ENABLE_SYMBOL = "LOG_FATAL_ENABLE";
        
        private const string MENU_ITEM_PATH = "Tools/Log Define/";
        private const int MENU_ITEM_PRIORITY = 50;

        private static readonly string[] s_AboveLogScriptingDefineSymbols = new string[]
        {
            LOG_DEBUG_AND_ABOVE_SYMBOL,
            LOG_INFO_AND_ABOVE_SYMBOL,
            LOG_WARNING_AND_ABOVE_SYMBOL,
            LOG_ERROR_AND_ABOVE_SYMBOL,
            LOG_FATAL_AND_ABOVE_SYMBOL
        };

        private static readonly string[] s_SpecifyLogScriptingDefineSymbols = new string[]
        {
            LOG_DEBUG_ENABLE_SYMBOL,
            LOG_INFO_ENABLE_SYMBOL,
            LOG_WARNING_ENABLE_SYMBOL,
            LOG_ERROR_ENABLE_SYMBOL,
            LOG_FATAL_ENABLE_SYMBOL
        };

        /// <summary>
        /// 禁用所有日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Disable All Logs", false, MENU_ITEM_PRIORITY)]
        public static void DisableAllLogs()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(LOG_ALL_SYMBOL);
        
            foreach (string specifyLogScriptingDefineSymbol in s_SpecifyLogScriptingDefineSymbols)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(specifyLogScriptingDefineSymbol);
            }
        
            foreach (string aboveLogScriptingDefineSymbol in s_AboveLogScriptingDefineSymbols)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(aboveLogScriptingDefineSymbol);
            }
        }
        
        /// <summary>
        /// 开启所有日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Enable All Logs", false, MENU_ITEM_PRIORITY)]
        public static void EnableAllLogs()
        {
            DisableAllLogs();
            ScriptingDefineSymbols.AddScriptingDefineSymbol(LOG_ALL_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Enable All Logs", true)]
        public static bool EnableAllLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL);
        }
        
        #region Debug Logs

#if LOG_DEBUG_ENABLE
        /// <summary>
        /// 禁用调试级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Disable Debug Logs", false, MENU_ITEM_PRIORITY + 11)]
        public static void DisableDebugLogs()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(LOG_DEBUG_ENABLE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Disable Debug Logs", true)]
        public static bool DisableDebugLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_ENABLE_SYMBOL);
        }
#else
        /// <summary>
        /// 开启调试级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Enable Debug Logs", false, MENU_ITEM_PRIORITY + 11)]
        public static void EnableDebugLogs()
        {
            SetSpecifyLogScriptingDefineSymbol(LOG_DEBUG_ENABLE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Enable Debug Logs", true)]
        public static bool EnableDebugLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_ENABLE_SYMBOL);
        }
#endif
        
        #endregion
        
        #region Debug And Above Logs

#if LOG_DEBUG_AND_ABOVE
        /// <summary>
        /// 禁用调试及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Disable Debug And Above Logs", false, MENU_ITEM_PRIORITY + 12)]
        public static void DisableDebugAndAboveLogs()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Disable Debug And Above Logs", true)]
        public static bool DisableDebugAndAboveLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL);
        }
#else
        /// <summary>
        /// 开启调试及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Enable Debug And Above Logs", false, MENU_ITEM_PRIORITY + 12)]
        public static void EnableDebugAndAboveLogs()
        {
            SetAboveLogScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Enable Debug And Above Logs", true)]
        public static bool EnableDebugAndAboveLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL);
        }
#endif
        
        #endregion

        #region Info Logs

#if LOG_INFO_ENABLE
        /// <summary>
        /// 禁用信息级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Disable Info Logs", false, MENU_ITEM_PRIORITY + 13)]
        public static void DisableInfoLogs()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(LOG_INFO_ENABLE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Disable Info Logs", true)]
        public static bool DisableInfoLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL) &&
                   ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_ENABLE_SYMBOL);
        }
#else
        /// <summary>
        /// 开启信息级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Enable Info Logs", false, MENU_ITEM_PRIORITY + 13)]
        public static void EnableInfoLogs()
        {
            SetSpecifyLogScriptingDefineSymbol(LOG_INFO_ENABLE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Enable Info Logs", true)]
        public static bool EnableInfoLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_ENABLE_SYMBOL);
        }
#endif
        
        #endregion
        
        #region Info And Above Logs

#if LOG_INFO_AND_ABOVE
        /// <summary>
        /// 开启调试及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Disable Info And Above Logs", false, MENU_ITEM_PRIORITY + 14)]
        public static void DisableInfoAndAboveLogs()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL);
        }

        [MenuItem(MENU_ITEM_PATH + "Disable Info And Above Logs", true)]
        public static bool DisableInfoAndAboveLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL);
        }
#else
        /// <summary>
        /// 开启信息及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Enable Info And Above Logs", false, MENU_ITEM_PRIORITY + 14)]
        public static void EnableInfoAndAboveLogs()
        {
            SetAboveLogScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Enable Info And Above Logs", true)]
        public static bool EnableInfoAndAboveLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL);
        }

#endif

        #endregion
        
        #region Warning Logs

#if LOG_WARNING_ENABLE
        /// <summary>
        /// 禁用警告级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Disable Warning Logs", false, MENU_ITEM_PRIORITY + 15)]
        public static void DisableWarningLogs()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(LOG_WARNING_ENABLE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Disable Warning Logs", true)]
        public static bool DisableWarningLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_WARNING_AND_ABOVE_SYMBOL) &&
                   ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_WARNING_ENABLE_SYMBOL);
        }
#else
        /// <summary>
        /// 开启警告级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Enable Warning Logs", false, MENU_ITEM_PRIORITY + 15)]
        public static void EnableWarningLogs()
        {
            SetSpecifyLogScriptingDefineSymbol(LOG_WARNING_ENABLE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Enable Warning Logs", true)]
        public static bool EnableWarningLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_WARNING_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_WARNING_ENABLE_SYMBOL);
        }
#endif
        
        #endregion

        #region Warning And Above Logs

#if LOG_WARNING_AND_ABOVE
        /// <summary>
        /// 禁用警告及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Disable Warning And Above Logs", false, MENU_ITEM_PRIORITY + 16)]
        public static void DisableWarningAndAboveLogs()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(LOG_WARNING_AND_ABOVE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Disable Warning And Above Logs", true)]
        public static bool DisableWarningAndAboveLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL) &&
                   ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_WARNING_AND_ABOVE_SYMBOL);
        }
#else
        /// <summary>
        /// 开启警告及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Enable Warning And Above Logs", false, MENU_ITEM_PRIORITY + 16)]
        public static void EnableWarningAndAboveLogs()
        {
            SetAboveLogScriptingDefineSymbol(LOG_WARNING_AND_ABOVE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Enable Warning And Above Logs", true)]
        public static bool EnableWarningAndAboveLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_WARNING_AND_ABOVE_SYMBOL);
        }
#endif
        
        #endregion
        
        #region Error Logs

#if LOG_ERROR_ENABLE
        /// <summary>
        /// 禁用警告级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Disable Error Logs", false, MENU_ITEM_PRIORITY + 17)]
        public static void DisableErrorLogs()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(LOG_ERROR_ENABLE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Disable Error Logs", true)]
        public static bool DisableErrorLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_WARNING_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ERROR_AND_ABOVE_SYMBOL) &&
                   ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ERROR_ENABLE_SYMBOL);
        }
#else
        /// <summary>
        /// 开启警告级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Enable Error Logs", false, MENU_ITEM_PRIORITY + 17)]
        public static void EnableErrorLogs()
        {
            SetSpecifyLogScriptingDefineSymbol(LOG_ERROR_ENABLE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Enable Error Logs", true)]
        public static bool EnableErrorLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_WARNING_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ERROR_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ERROR_ENABLE_SYMBOL);
        }
#endif
        
        #endregion
        
        #region Error And Above Logs

#if LOG_ERROR_AND_ABOVE 
        /// <summary>
        /// 禁用调试及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Disable Error And Above Logs", false, MENU_ITEM_PRIORITY + 18)]
        public static void DisableErrorAndAboveLogs()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(LOG_ERROR_AND_ABOVE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Disable Error And Above Logs", true)]
        public static bool DisableErrorAndAboveLogsValidation()
        {
             return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
               !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
               !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL) &&
               !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_WARNING_AND_ABOVE_SYMBOL) &&
               ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ERROR_AND_ABOVE_SYMBOL);
        }

#else
        /// <summary>
        /// 开启错误及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Enable Error And Above Logs", false, MENU_ITEM_PRIORITY + 18)]
        public static void EnableErrorAndAboveLogs()
        {
            SetAboveLogScriptingDefineSymbol(LOG_ERROR_AND_ABOVE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Enable Error And Above Logs", true)]
        public static bool EnableErrorAndAboveLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_WARNING_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ERROR_AND_ABOVE_SYMBOL);
        }
#endif
        
        #endregion
        
        #region Fatal Logs

#if LOG_FATAL_ENABLE
        /// <summary>
        /// 禁用警告级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Disable Fatal Logs", false, MENU_ITEM_PRIORITY + 19)]
        public static void DisableFatalLogs()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(LOG_FATAL_ENABLE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Disable Fatal Logs", true)]
        public static bool DisableFatalLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_WARNING_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ERROR_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_FATAL_AND_ABOVE_SYMBOL) &&
                   ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_FATAL_ENABLE_SYMBOL);
        }
#else
        /// <summary>
        /// 开启警告级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Enable Fatal Logs", false, MENU_ITEM_PRIORITY + 19)]
        public static void EnableFatalLogs()
        {
            SetSpecifyLogScriptingDefineSymbol(LOG_FATAL_ENABLE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Enable Fatal Logs", true)]
        public static bool EnableFatalLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_WARNING_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ERROR_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_FATAL_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_FATAL_ENABLE_SYMBOL);
        }
#endif
        
        #endregion

        #region Fatal And Above Logs

#if LOG_FATAL_AND_ABOVE
        /// <summary>
        /// 开启严重错误及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Disable Fatal And Above Logs", false, MENU_ITEM_PRIORITY + 20)]
        public static void DisableFatalAndAboveLogs()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(LOG_FATAL_AND_ABOVE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Disable Fatal And Above Logs", true)]
        public static bool DisableFatalAndAboveLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_WARNING_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ERROR_AND_ABOVE_SYMBOL) &&
                   ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_FATAL_AND_ABOVE_SYMBOL);
        }
#else
        /// <summary>
        /// 开启严重错误及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(MENU_ITEM_PATH + "Enable Fatal And Above Logs", false, MENU_ITEM_PRIORITY + 20)]
        public static void EnableFatalAndAboveLogs()
        {
            SetAboveLogScriptingDefineSymbol(LOG_FATAL_AND_ABOVE_SYMBOL);
        }
        
        [MenuItem(MENU_ITEM_PATH + "Enable Fatal And Above Logs", true)]
        public static bool EnableFatalAndAboveLogsValidation()
        {
            return !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ALL_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_DEBUG_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_INFO_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_WARNING_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_ERROR_AND_ABOVE_SYMBOL) &&
                   !ScriptingDefineSymbols.HasScriptingDefineSymbol(LOG_FATAL_AND_ABOVE_SYMBOL);
        }
#endif
        
        #endregion
        
        /// <summary>
        /// 设置日志脚本宏定义。
        /// </summary>
        /// <param name="aboveLogScriptingDefineSymbol">要设置的日志脚本宏定义。</param>
        public static void SetAboveLogScriptingDefineSymbol(string aboveLogScriptingDefineSymbol)
        {
            if (string.IsNullOrEmpty(aboveLogScriptingDefineSymbol))
            {
                return;
            }

            foreach (string i in s_AboveLogScriptingDefineSymbols)
            {
                if (i == aboveLogScriptingDefineSymbol)
                {
                    DisableAllLogs();
                    ScriptingDefineSymbols.AddScriptingDefineSymbol(aboveLogScriptingDefineSymbol);
                    return;
                }
            }
        }
        
        /// <summary>
        /// 设置日志脚本宏定义。
        /// </summary>
        /// <param name="specifyLogScriptingDefineSymbol">要设置的日志脚本宏定义。</param>
        public static void SetSpecifyLogScriptingDefineSymbol(string specifyLogScriptingDefineSymbol)
        {
            if (string.IsNullOrEmpty(specifyLogScriptingDefineSymbol))
            {
                return;
            }

            foreach (string i in s_SpecifyLogScriptingDefineSymbols)
            {
                if (i == specifyLogScriptingDefineSymbol)
                {
                    ScriptingDefineSymbols.AddScriptingDefineSymbol(specifyLogScriptingDefineSymbol);
                    return;
                }
            }
        }

        /// <summary>
        /// 设置日志脚本宏定义。
        /// </summary>
        /// <param name="specifyLogScriptingDefineSymbols">要设置的日志脚本宏定义。</param>
        public static void SetSpecifyLogScriptingDefineSymbols(string[] specifyLogScriptingDefineSymbols)
        {
            if (specifyLogScriptingDefineSymbols == null || specifyLogScriptingDefineSymbols.Length <= 0)
            {
                return;
            }

            bool removed = false;
            foreach (string specifyLogScriptingDefineSymbol in specifyLogScriptingDefineSymbols)
            {
                if (string.IsNullOrEmpty(specifyLogScriptingDefineSymbol))
                {
                    continue;
                }

                foreach (string i in s_SpecifyLogScriptingDefineSymbols)
                {
                    if (i == specifyLogScriptingDefineSymbol)
                    {
                        if (!removed)
                        {
                            removed = true;
                            DisableAllLogs();
                        }

                        ScriptingDefineSymbols.AddScriptingDefineSymbol(specifyLogScriptingDefineSymbol);
                        break;
                    }
                }
            }
        }
    }
}
