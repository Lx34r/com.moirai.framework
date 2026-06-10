using Moirai.Atropos;

namespace Moirai.Main
{
    /// <summary>
    /// APP更新类型。
    /// </summary>
    public enum UpdateType
    {
        None = 0,
    
        // 资源更新
        ResourceUpdate = 1,
    
        // 底包更新
        PackageUpdate = 2,
    }
    
    /// <summary>
    /// 版本更新数据。
    /// </summary>
    public class UpdateData
    {
        /// <summary>
        /// 当前版本信息。
        /// </summary>
        public string CurrentVersion;
    
        /// <summary>
        /// 是否底包更新。
        /// </summary>
        public UpdateType UpdateType;
    
        /// <summary>
        /// 是否强制更新。
        /// </summary>
        public EUpdateStyle UpdateStyle;
    
        /// <summary>
        /// 是否提示。
        /// </summary>
        public EUpdateNotice UpdateNotice;
    
        /// <summary>
        /// 热更资源地址。
        /// </summary>
        public string HostServerURL;
    
        /// <summary>
        /// 备用热更资源地址。
        /// </summary>
        public string FallbackHostServerURL;
    }
}