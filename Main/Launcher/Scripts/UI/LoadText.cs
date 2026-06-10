using System;
using Moirai.Atropos;
using Moirai.Atropos.Localization;
using UnityEngine;

namespace Moirai.Main
{
    /// <summary>
    /// 游戏内置的多语言文本
    /// </summary>
    public class TextMode
    {
        public string Label_Load_Progress = "正在下载资源文件，请耐心等待\n当前下载速度：{0}/s 资源文件大小：{1}";
        public string Label_Load_FirstUnpack = "首次进入游戏，正在初始化游戏资源...（此过程不消耗网络流量）";
        /// <summary>正在更新本地资源版本</summary>
        public string Label_Load_Unpacking = "正在更新本地资源版本，请耐心等待...（此过程不消耗网络流量）";
        /// <summary>检测更新设置{0}...</summary>
        public string Label_Load_Checking = "检测更新设置{0}...";
        /// <summary>最新版本检测完成</summary>
        public string Label_Load_Checked = "最新版本检测完成";
        /// <summary>当前使用的版本过低，请下载安装最新版本</summary>
        public string Label_Load_Package = "当前使用的版本过低，请下载安装最新版本";
        public string Label_Load_Platform = "当前使用的版本过低，请前往应用商店安装最新版本";
        /// <summary>检测到可选资源更新</summary>
        public string Label_Load_Notice = "检测到可选资源更新,更新包大小<color=#BA3026>{0}</color>，\n推荐完成更新提升游戏体验";
        public string Label_Load_Force = "检测到版本更新，取消更新将导致无法进入游戏";

        /// <summary>检测到有新的游戏内容需要更新</summary>
        public string Label_Load_Force_WIFI =
            "检测到有新的游戏内容需要更新，\n更新包大小<color=#BA3026>{0}</color>, \n取消更新将导致无法进入游戏，您当前已为<color=#BA3026>wifi网络</color>，请开始更新";

        /// <summary>检测到有新的游戏内容需要更新</summary>
        public string Label_Load_Force_NO_WIFI =
            "检测到有新的游戏内容需要更新，\n更新包大小<color=#BA3026>{0}</color>, \n取消更新将导致无法进入游戏，请开始更新";

        public string Label_Load_Error = "更新参数错误{0}，请点击确定重新启动游戏";
        public string Label_Load_FirstEnterGame_Error = "首次进入游戏资源异常";
        /// <summary>正在加载最新资源文件</summary>
        public string Label_Load_UnpackComplete = "正在加载最新资源文件...（此过程不消耗网络流量）";
        public string Label_Load_UnPackError = "资源解压失败，请点击确定重新启动游戏";
        /// <summary>正在载入...{0}%</summary>
        public string Label_Load_Load_Progress = "正在载入...{0}%";
        /// <summary>载入完成</summary>
        public string Label_Load_Load_Complete = "载入完成";
        /// <summary>初始化资源中...</summary>
        public string Label_Load_Init = "初始化资源中...";
        /// <summary>资源初始化失败！</summary>
        public string Label_Load_InitFailed = "资源初始化失败！";
        /// <summary>重新初始化资源中...</summary>
        public string Label_Load_RetryInit = "重新初始化资源中...";
        /// <summary>当前网络不可用</summary>
        public string Label_Net_UnReachable = "当前网络不可用，请检查本地网络设置后点击确认进行重试";
        public string Label_Net_ReachableViaCarrierDataNetwork = "当前是移动网络，是否继续下载";
        /// <summary>网络异常，请重试</summary>
        public string Label_Net_Error = "网络异常，请重试";
        public string Label_Net_Changed = "网络切换,正在尝试重连,{0}次";
        public string Label_Data_Empty = "数据异常";
        public string Label_Memory_Low = "初始化资源加载失败，请检查本地内存是否充足";
        public string Label_Memory_Low_Load = "内存是否充足,无法更新";
        public string Label_Memory_UnZip_Low = "内存不足，无法解压";
        /// <summary>游戏版本号:{0}</summary>
        public string Label_App_id = "APPVer {0}";
        /// <summary>资源版本号:{0}</summary>
        public string Label_Res_id = "ResVer {0}";
        /// <summary>是否清理本地资源</summary>
        public string Label_Clear_Confirm = "是否清理本地资源?(清理完成后会关闭游戏且重新下载最新资源)";
        public string Label_RestartApp = "本次更新需要重启应用，请点击确定重新启动游戏";
        public string Label_DownLoadFailed = "网络太慢，是否继续下载";
        public string Label_ClearConfig = "清除环境配置，需要重启应用";
        
        public string Label_RegionInfoIllegal = "区服信息为空";
        /// <summary>热更地址为空</summary>
        public string Label_RemoteUrlIsNull = "热更地址为空";
        public string Label_FirstPackageNotFound = "首包资源加载失败";
        public string Label_RequestReginInfo = "正在请求区服信息{0}次";
        public string Label_RequestTimeOut = "请求区服信息超时,是否重试？";
        public string Label_Region_ArgumentError = "参数错误";
        public string Label_Region_IndexOutOfRange = "索引越界";
        public string Label_Region_NonConfigApplication = "未配置此应用";
        public string Label_Region_SystemError = "系统异常";

        public string Label_PreventionOfAddiction = "著作人权：XX市XXX有限公司 软著登记号：2022SR0000000\n抵制不良游戏，拒绝盗版游戏。注意自我保护，谨防受骗上当。适度游戏益脑，" +
                                                    "沉迷游戏伤身。合理安排时间，享受健康生活。";

        /// <summary>确定</summary>
        public string Label_Btn_Update = "确定";
        /// <summary>取消</summary>
        public string Label_Btn_Ignore = "取消";
        /// <summary>更新</summary>
        public string Label_Btn_Package = "更新";

        /// <summary>配置检测中...</summary>
        public string Label_Dlc_ConfigDetectionStage = "配置检测中...";
        public string Label_Dlc_ConfigVerificationStage = "配置校验中...";
        public string Label_Dlc_ConfigLoadingStage = "下载配置中...";

        public string Label_Dlc_Load_Force_WIFI =
            "检测到有新的游戏内容需要更新, 取消更新将导致无法进入游戏，您当前已为<color=#BA3026>wifi网络</color>，请开始更新";

        public string Label_Dlc_Load_Force_NO_WIFI =
            "检测到有新的游戏内容需要更新, 取消更新将导致无法进入游戏，请开始更新";

        public string Label_Had_Update = "检测到有版本更新...";
        /// <summary>更新静态版本文件</summary>
        public string Label_RequestVersionIng = "正在向服务器请求版本信息中...";
        public string Label_RequestVersionInfo = "正在向服务器请求版本信息{0}次";
        
        /// <summary>创建补丁下载器</summary>
        public string Label_CreateDownloader = "创建补丁下载器...";
        /// <summary>开始下载更新文件...</summary>
        public string Label_Download_Start = "开始下载更新文件...";
        /// <summary>正在下载...{0}%</summary>
        public string Label_Download_Progress = "正在下载...{0}%";
        /// <summary>下载完成</summary>
        public string Label_Download_Complete = "下载完成";
        /// <summary>正在更新，已更新 {0}/{1} ({2:F2}%)</summary>
        public string Label_Download_Detail1 = "正在更新，已更新 {0}/{1} ({2:F2}%)";
        /// <summary>已更新大小 {0}MB/{1}MB</summary>
        public string Label_Download_Detail2 = "已更新大小 {0}MB/{1}MB";
        /// <summary>当前网速 {0}/s，剩余时间 {1}</summary>
        public string Label_Download_Detail3 = "当前网速 {0}/s，剩余时间 {1}";
        
        /// <summary>清理未使用的缓存文件</summary>
        public string Label_ClearCache = "清理未使用的缓存文件...";
        /// <summary>清理完成</summary>
        public string Label_ClearCache_Completed = "清理完成，即将进入游戏...";
        
        /// <summary>更新清单文件</summary>
        public string Label_UpdateManifest = "更新清单文件...";

    }

    public class LoadText : TextMode
    {
        private static LoadText _instance;

        public static LoadText Instance => _instance ??= new LoadText();

        /// <summary>
        /// 加载内置多语言
        /// <remarks>json 位于 Resources 下，后缀以 BuildInText_Code</remarks>
        /// </summary>
        public void InitConfigData()
        {
            string buildInTextName = "BuildInText_"; // 默认前缀
            string suffix = ""; // 语言 code 后缀
            string settingSource = null;
            
            Language settingLanguage = GameModule.Localization.GetCurrentLanguage(false, ref settingSource);
            suffix = settingLanguage.Code;
            TextAsset textAsset = Resources.Load<TextAsset>(buildInTextName + suffix);
            if (textAsset == null)
            {
                suffix = LocalizationHelper.DefaultLanguage.Code;
                textAsset = Resources.Load<TextAsset>(buildInTextName + suffix);
            }

            if (textAsset == null)
            {
                Log.Error($"LoadText Failed: {buildInTextName}{suffix}");
                return;
            }

            Log.Info($"LoadText: {buildInTextName}{suffix}");
            try
            {
                TextMode loadConfig = JSONUtility.ToObject<TextMode>(textAsset.text);
                if (loadConfig == null) return;
                
                // 利用反射赋值
                var fields = typeof(TextMode).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                foreach (var field in fields)
                {
                    var fieldName = field.Name;
                    var configValue = field.GetValue(loadConfig);
                    var targetField = this.GetType().GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (targetField != null)
                    {
                        targetField.SetValue(this, configValue);
                    }
                    else
                    {
                        Log.Error($"LoadText: Can not find field: {fieldName}, please check BuildInText.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error parsing built-in text JSON ({buildInTextName}{suffix}): {ex}");
            }
        }
    }
}