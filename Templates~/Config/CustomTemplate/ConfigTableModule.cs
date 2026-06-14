using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using Moirai.Atropos;
using Moirai.Atropos.Localization;
using Moirai.GameProto.Config.L10n;
using UnityEngine;
using UnityEngine.U2D;

namespace Moirai.GameProto.ConfigTable
{
    /// <summary>
    /// 游戏配置表助手。
    /// </summary>
    public partial class ConfigTableModule : IConfigTableModule
    {
        #region 初始化 [INITIALIZE]

        private static bool s_Registered = false;
        /// <summary>
        /// 注册配置表实例。
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        private static void Initialize()
        {
            if (s_Registered) return;
            
            ConfigMgr.Instance.ConfigTableModule = new ConfigTableModule();
            s_Registered = true;
        }

        #endregion

        #region 处理多语言 [LOCALIZATION]

        private Dictionary<string, List<string>> _allLocalizedStrings;
        public Dictionary<string, List<string>> GetAllLocalizedStrings()
        {
            if (_allLocalizedStrings == null)
            {
                ResolveLocalization();
            }

            return _allLocalizedStrings;
        }

        /// <summary>
        /// 初始化所有可用的多语言
        /// </summary>
        /// <returns></returns>
        private void ResolveLocalization()
        {
            Log.Info("<color=yellow>\u25bc\u25bc\u25bc\u25bc " +
                     "Start Resolve LocalizationBean~" +
                     " \u25bc\u25bc\u25bc\u25bc</color>");
            
            // 获取类型 -> 多语言Bean
            Type type = typeof(LocalizedStringBean);

            // 获取所有公共实例字段
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            // 注册所有可用的多语言
            foreach (var field in fields)
            {
                LocalizationHelper.RegisterLanguageMap(field.Name);
            }

            // 处理所有多语言数据
            _allLocalizedStrings = new Dictionary<string, List<string>>();
            foreach (var data in Tables.TbLocalizedStrings.DataList)
            {
                foreach (FieldInfo field in fields)
                {
                    // 检查字段是否为 readonly 并且类型为 string
                    if (field.IsInitOnly && field.FieldType == typeof(string))
                    {
                        // 获取多语言的 Key
                        string key = data.Key;
                        // 获取多语言的字段值
                        string fieldValue = (string)field.GetValue(data.FormattedStrings);

                        // 输出字段名称和值
                        // Debug.Log($"[{key}] Field Name: {field.Name}, Value: {fieldValue}");

                        if (_allLocalizedStrings.ContainsKey(key))
                        {
                            _allLocalizedStrings[key].Add(fieldValue);
                        }
                        else
                        {
                            _allLocalizedStrings.Add(key, new List<string> { fieldValue });
                        }
                    }
                }
            }

            Log.Info("<color=yellow>\u25b2\u25b2\u25b2\u25b2 " +
                     "Resolve LocalizationBean Done!" +
                     " \u25b2\u25b2\u25b2\u25b2</color>");

            // string str = "";
            // foreach (var item in _allLocalizedStrings)
            // {
            //     str += $"{item.Key}[{item.Value.Count}]: {string.Join(",", item.Value)}\n";
            // }
            // Log.Info($"AllLocalizedStrings:\n{str}");
        }

        #endregion

        #region 界面 [UI]

        public string GetUIWindowLocation(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            if (!Tables.TbUIWindow.DataMap.TryGetValue(id, out var uiWindowConfig))
            {
                Log.Warning($"UI ID[{id}] is invalid.");
                return string.Empty;
            }
            
            // todo 获取当前主题的配置？
            // UI 当前主题配置在 UIConfigManager
            return uiWindowConfig.DefaultRes;
        }

        /// <summary>
        /// 根据图集名（配置表 id 必须为图集名）获取实际 SpriteAtlas
        /// </summary>
        /// <param name="id">UISprite - SpriteAtlas 配置表的 id</param>
        /// <param name="cancellationToken"></param>
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        public async UniTask<Sprite> LoadSpriteByID(string id, CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            if (string.IsNullOrEmpty(id)) return null;

            if (GameModule.Resource == null)
            {
                Log.Warning("ResourceModule is null.");
                return null;
            }
            
            if (!Tables.TbSprite.DataMap.TryGetValue(id, out var spriteConfig))
            {
                Log.Warning($"Sprite ID[{id}] is invalid.");
                return null;
            }
            
            if (!Tables.TbSpriteAtlas.DataMap.TryGetValue(spriteConfig.SpriteAtlasId, out var atlasConfig))
            {
                Log.Warning($"SpriteAtlasId ID[{id}] is invalid.");
                return null;
            }
            
            // Log.Info($"LoadSpriteByID {id} from {atlasConfig.Location}");

            var atlas =
// #if UNITY_WEBGL
//             await GameModule.Resource.LoadAssetAsync<SpriteAtlas>(atlasConfig.Location, packageName:atlasConfig.PackageName);
// #else
               GameModule.Resource.LoadAsset<SpriteAtlas>(atlasConfig.Location, packageName:atlasConfig.PackageName);
// #endif
            return atlas?.GetSprite(spriteConfig.SpriteName);
        }

        #endregion
    }
}