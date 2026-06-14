using System;
using UnityEngine;

namespace Moirai.Atropos.Debugger
{
    public sealed partial class DebuggerComp
    {
        private sealed class PathInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Path Information</b>");
                GUILayout.BeginVertical("box");
                {
                    DrawItem("Current Directory", PathUtility.FormatToUnityPath(Environment.CurrentDirectory));
                    DrawItem("Data Path", PathUtility.FormatToUnityPath(Application.dataPath));
                    DrawItem("Persistent Data Path", PathUtility.FormatToUnityPath(Application.persistentDataPath));
                    DrawItem("Streaming Assets Path", PathUtility.FormatToUnityPath(Application.streamingAssetsPath));
                    DrawItem("Temporary Cache Path", PathUtility.FormatToUnityPath(Application.temporaryCachePath));
#if UNITY_2018_3_OR_NEWER
                    DrawItem("Console Log Path", PathUtility.FormatToUnityPath(Application.consoleLogPath));
#endif
                }
                GUILayout.EndVertical();
            }
        }
    }
}
