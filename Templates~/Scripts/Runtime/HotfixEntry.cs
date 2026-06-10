using System.Collections.Generic;
using System.Reflection;
using Moirai.Atropos;
using Moirai.Atropos.Events;
using Moirai.Atropos.Procedure;
#if OBFUZ_INSTALLED && ENABLE_OBFUZ
using Obfuz;
#endif

namespace GameLogic
{
    /// <summary>
    /// 进入主流程事件，单次流程。
    /// </summary>
    /// <remarks>用于初始化一些初始设定</remarks>
    public class HotfixEntryEvent : EventBase<HotfixEntryEvent>, IProcedureEvent
    {
        public static void Trigger()
        {
            using var evt = GetPooled();
            EventManager.SendEvent(evt);
        }
    }

    /// <summary>
    /// 游戏主程序入口
    /// </summary>
#if OBFUZ_INSTALLED && ENABLE_OBFUZ
    [ObfuzIgnore(ObfuzScope.TypeName | ObfuzScope.MethodName)]
#endif
    public partial class HotfixEntry
    {
        private static List<Assembly> s_HotfixAssembly;
        
        /// <summary>
        /// 热更域App主入口。
        /// </summary>
        /// <param name="objects"></param>
        public static void Entrance(object[] objects)
        {
            s_HotfixAssembly = (List<Assembly>)objects[0];

            Log.Info("<b><color=orange>======= Entrance GameMain =======</color></b>");
            UnityUtility.AddDestroyListener(Release);
            StartGameLogic();
        }
        
        private static void StartGameLogic()
        {
            Log.Info("<b><color=orange>======= StartGameLogic =======</color></b>");

            MainThreadDispatcher.Instance.Dispatch(() => { Log.Info("Init UnityMainThreadDispatcher"); });
            
            // 保证 UIModule 正常初始化
            GameModule.UI.CloseAll();
            
            // 初始化多语言配置
            GameModule.Localization.InitLanguageSettings();
            
            // 事件通知
            HotfixEntryEvent.Trigger();
        }
        
        private static void Release()
        {
            SingletonSystem.Release();
            Log.Warning("======= Release GameApp =======");
        }
    }
}