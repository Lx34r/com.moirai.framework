using System;
using System.Collections.Generic;
using System.Linq;
using Moirai.Atropos.ConfigTable;
using UnityEngine;
using YooAsset;

namespace Moirai.Atropos.Localization
{
    /// <summary>
    /// 本地化多语言模块。
    /// todo 配置表导出分表，不需要一次加载所有多语言
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class LocalizationModule : Module, ILocalizationModule
    {
        // 本地化器列表
        private readonly List<LocalizerBase> _localizers = new List<LocalizerBase>();
        
        // 语言列表
        private List<Language> LanguageList { get; set; } = new List<Language>();
        // 本地化字符串字典
        private Dictionary<string, List<string>> LocalizedStrings { get; set; } = new Dictionary<string, List<string>>();
        
        // 当前本地化语言
        private Language _currentLanguage;
        // 当前本地化语言设置来自
        private string _settingSource;
        
        public Language CurrentLanguage => _currentLanguage ?? GetCurrentLanguage(true, ref _settingSource);
        public int CurrentLanguageIndex => GetLanguageIndex(_currentLanguage);
        public event ILocalizationModule.OnLanguageChangedDelegate OnLanguageChanged;

        public override void OnInit() { }
        public override void Shutdown() { }
        
        public void InitLanguageSettings()
        {
            if (LocalizedStrings.Count == 0)
            {
                LocalizedStrings.Clear();
                LoadLocalizedStrings();
            }
            
            ChangeLanguage(CurrentLanguage, true);
        }
        
        public Language GetCurrentLanguage(bool onlySupported, ref string settingSource)
        {
            // 获取启动命令中的设置
            string language = CommandLineUtility.GetForceLanguage();
            if (!string.IsNullOrEmpty(language))
            {
                settingSource = "CommandLine";
            }
            else
            {
                // 如果处于编辑器模拟模式下，使用编辑器设置的语言
                if (GameModule.Resource.PlayMode == EPlayMode.EditorSimulateMode
                    && GameModule.Base.EditorLanguage != Language.Unspecified.Name)
                {
                    language = GameModule.Base.EditorLanguage;
                    settingSource = "EditorSetting";
                }
                // 如果已设置语言，则使用设置的语言
                else if (SettingUtility.HasSetting(Constant.Setting.LANGUAGE))
                {
                    language = SettingUtility.GetString(Constant.Setting.LANGUAGE);
                    settingSource = "SavedSetting";
                }
                // 否则，使用系统语言
                else
                {
                    SystemLanguage systemLanguage = Application.systemLanguage;
                    // 未区分简繁时，使用简体中文
                    if (systemLanguage == SystemLanguage.Chinese)
                    {
                        systemLanguage = SystemLanguage.ChineseSimplified;
                    }
                    language = ((Language)systemLanguage).Code;
                    settingSource = "SystemLanguage";
                }
            }
            
            return LocalizationHelper.ToLanguage(language, onlySupported);
        }
        
        /// <summary>
        /// 从配置表加载本地化字符串到内存。
        /// </summary>
        private void LoadLocalizedStrings()
        {
            LocalizedStrings = ConfigMgr.Instance.GetAllLocalizedStrings();
            LanguageList = LocalizationHelper.GetAllAvailableLanguages();
            
            if (LanguageList.Count != 0 && LocalizedStrings != null)
            {
                Log.Info("Load Localized Text Success!");
            }
            else
            {
                Log.Error("Failed to load localized text, generate config first!");
            }
        }

        public void ChangeLanguage(Language language, bool logSource = false)
        {
            if (LanguageList.Count == 0)
            {
                Log.Error("No language available!");
                return;
            }

            if (_currentLanguage == language) return;

            _currentLanguage = LanguageList[GetLanguageIndex(language)];
            OnLanguageChanged?.Invoke(_currentLanguage);

            // 重新注入所有注入器的字符串。
            _localizers.ForEach(_ => _.Localize());

            SettingUtility.SetString(Constant.Setting.LANGUAGE, _currentLanguage.Code);
            Log.Info($"Change the language: {_currentLanguage}{(logSource ? $"(by {_settingSource})" : "")}");
        }

        public void ChangeLanguage(string language) => ChangeLanguage(LocalizationHelper.ToLanguage(language, true));

        public void ChangeLanguage(int index) => ChangeLanguage(LanguageList[index]);
        public string ActivatePreviousLanguage()
        {
            var prevIndex = (int)Mathf.Repeat(CurrentLanguageIndex - 1, LanguageList.Count);
            ChangeLanguage(LanguageList[prevIndex]);
            return LanguageList[prevIndex].Name;
        }

        public string ActivateNextLanguage()
        {
            var nextIndex = (int)Mathf.Repeat(CurrentLanguageIndex + 1, LanguageList.Count);
            ChangeLanguage(LanguageList[nextIndex]);
            return LanguageList[nextIndex].Name;
        }

        /// <summary>
        /// 获取语言索引
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        private int GetLanguageIndex(Language language)
        {
            // 检查语言列表是否为空
            if (LanguageList == null || !LanguageList.Any())
            {
                Log.Error("Language list is empty or null");
                throw new InvalidOperationException("Language list is empty or null");
            }

            // 进行匹配
            var i = LanguageList.FindIndex(s => s == language);

            // 处理语言不存在的情况
            if (i == -1)
            {
                Log.Error($"Language {language} is not available");
                throw new KeyNotFoundException($"Language {language} is not available");
            }
            
            return i;
        }
        
        public void AddLocalizer(LocalizerBase localizer) => _localizers.Add(localizer);
        
        public void RemoveLocalizer(LocalizerBase localizer) => _localizers.Remove(localizer);

        public bool Has(string id) => LocalizedStrings.ContainsKey(id);
        
        public string GetTextFromId(string id, params object[] p) => GetTextFromIdLanguage(id, _currentLanguage, p);
        
        public string GetTextFromIdLanguage(string id, Language language, params object[] p)
        {
            // 不是多语言直接返回
            if (!LocalizedStrings.ContainsKey(id)) return id;
            
            var languageIndex = GetLanguageIndex(language);
            string text = p is { Length: > 0 }
                ? string.Format(LocalizedStrings[id][languageIndex], p)
                : LocalizedStrings[id][languageIndex];
            
            // 如果该文本没有被翻译，返回 ID
            return string.IsNullOrEmpty(text) ? id : text;
        }

        public Dictionary<string, string> GetDictionaryFromId(string id)
        {
            var dict = new Dictionary<string, string>();
            if (!LocalizedStrings.ContainsKey(id)) return dict;
            
            foreach (var language in LanguageList)
            {
                var text = GetTextFromIdLanguage(id, language);
                dict.Add(language.Name, text);
            }

            return dict;
        }

        public List<string> GetAllIds() => LocalizedStrings.Keys.ToList();
    }
}