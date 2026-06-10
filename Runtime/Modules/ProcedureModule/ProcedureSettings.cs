using System;
using Cysharp.Threading.Tasks;
using Moirai.Atropos.Fsm;
using UnityEngine;

namespace Moirai.Atropos.Procedure
{
    // ReSharper disable once InconsistentNaming
    public sealed class ProcedureSettings : ScriptableObject
    {
        [SerializeField] private string[] m_AvailableProcedureTypeNames = null;
        
        [SerializeField] private string m_EntranceProcedureTypeName = null;

        private IProcedureModule _procedureModule = null;
        private ProcedureBase _entranceProcedure = null;
        
        /// <summary>
        /// 获取当前流程。
        /// </summary>
        public static ProcedureBase CurrentProcedure
        {
            get
            {
                if (Instance._procedureModule == null)
                {
                    return null;
                }

                return Instance._procedureModule.CurrentProcedure;
            }
        }

        /// <summary>
        /// 获取当前流程持续时间。
        /// </summary>
        public static float CurrentProcedureTime
        {
            get
            {
                if (Instance._procedureModule == null)
                {
                    return 0f;
                }

                return Instance._procedureModule.CurrentProcedureTime;
            }
        }

        /// <summary>
        /// 启动流程。
        /// </summary>
        public static async UniTaskVoid StartProcedure()
        {
            if (Instance._procedureModule == null)
            {
                Instance._procedureModule = ModuleSystem.GetModule<IProcedureModule>();
            }

            if (Instance._procedureModule == null)
            {
                Log.Fatal("Procedure manager is invalid.");
                return;
            }

            ProcedureBase[] procedures = new ProcedureBase[Instance.m_AvailableProcedureTypeNames.Length];
            for (int i = 0; i < Instance.m_AvailableProcedureTypeNames.Length; i++)
            {
                Type procedureType = AssemblyUtility.GetType(Instance.m_AvailableProcedureTypeNames[i]);
                if (procedureType == null)
                {
                    Log.Error("Can not find procedure type '{0}'.", Instance.m_AvailableProcedureTypeNames[i]);
                    return;
                }

                procedures[i] = (ProcedureBase)Activator.CreateInstance(procedureType);
                if (procedures[i] == null)
                {
                    Log.Error("Can not create procedure instance '{0}'.", Instance.m_AvailableProcedureTypeNames[i]);
                    return;
                }

                if (Instance.m_EntranceProcedureTypeName == Instance.m_AvailableProcedureTypeNames[i])
                {
                    Instance._entranceProcedure = procedures[i];
                }
            }

            if (Instance._entranceProcedure == null)
            {
                Log.Error("Entrance procedure is invalid.");
                return;
            }

            Instance._procedureModule.Initialize(ModuleSystem.GetModule<IFsmModule>(), procedures);

            await UniTask.Yield();

            Instance._procedureModule.StartProcedure(Instance._entranceProcedure.GetType());
        }

        #region 设置单例

        private const string SETTINGS_DATA_NAME = "ProcedureSettings";
        private const string SETTINGS_DATA_FILE = "Assets/Settings/Framework/Resources/" + SETTINGS_DATA_NAME + ".asset";
        private static ProcedureSettings s_Instance;
        private static ProcedureSettings Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = Resources.Load<ProcedureSettings>(SETTINGS_DATA_NAME);
                    if (s_Instance == null)
                    {
#if UNITY_EDITOR
                        s_Instance = SettingHelper.LoadSettingSO<ProcedureSettings>(SETTINGS_DATA_FILE);
#else
                        Log.Error($"Could not find Settings at path '{SETTINGS_DATA_FILE} - Create using Tools->Settings->{SETTINGS_DATA_NAME}'");
#endif
                    }
                }
                return s_Instance;
            }
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Settings/" + SETTINGS_DATA_NAME, priority = -490)]
        private static void CreateSettings()
        {
            UnityEditor.Selection.activeObject = SettingHelper.LoadSettingSO<ProcedureSettings>(SETTINGS_DATA_FILE);
        }
#endif

        #endregion
    }
}