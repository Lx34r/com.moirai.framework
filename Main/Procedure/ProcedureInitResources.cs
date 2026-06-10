using System.Collections;
using Moirai.Atropos;
using Moirai.Atropos.Fsm;
using Moirai.Atropos.Procedure;
using UnityEngine;
using YooAsset;

namespace Moirai.Main
{
    /// <summary>
    /// 流程 => 初始化资源
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProcedureInitResources : ProcedurePremainBase
    {
        private bool _initResourcesComplete = false;

        public override bool UseNativeDialog => true;
        
        private IFsm<IProcedureModule> _procedureOwner;

        protected override void OnEnter(IFsm<IProcedureModule> procedureOwner)
        {
            base.OnEnter(procedureOwner);

            _procedureOwner = procedureOwner;
            _initResourcesComplete = false;
            
            LauncherMgr.ShowUI<LoadUpdateUI>(LoadText.Instance.Label_Load_Init);
            
            // 注意：使用单机模式并初始化资源前，需要先构建 AssetBundle 并复制到 StreamingAssets 中，否则会产生 HTTP 404 错误
            UnityUtility.StartCoroutine(InitResources(procedureOwner));
        }

        protected override void OnUpdate(IFsm<IProcedureModule> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!_initResourcesComplete)
            {
                // 初始化资源未完成则继续等待
                return;
            }

            if (_resourceModule.PlayMode == EPlayMode.HostPlayMode || _resourceModule.PlayMode == EPlayMode.WebPlayMode)
            {
                // 线上最新版本operation.PackageVersion
                Log.Debug($"Updated package Version : from {_resourceModule.GetPackageVersion()} to {_resourceModule.PackageVersion}");
                // 注意：保存资源版本号作为下次默认启动的版本!
                // 如果当前是WebGL或者是边玩边下载直接进入预加载阶段。
                if (_resourceModule.PlayMode == EPlayMode.WebPlayMode ||
                    _resourceModule.UpdatableWhilePlaying)
                {
                    // 边玩边下载还可以拓展首包支持。
                    ChangeToPreloadState(procedureOwner);
                    return;
                }

                ChangeToCreateDownloaderState(procedureOwner);
                return;
            }

            ChangeToPreloadState(procedureOwner);
        }
        
        private void ChangeToCreateDownloaderState(IFsm<IProcedureModule> procedureOwner)
        {
            ChangeState<ProcedureCreateDownloader>(procedureOwner);
        }

        /// <summary>
        /// 初始化资源流程。
        /// </summary>
        /// <remarks>YooAsset 需要保持编辑器、单机、联机模式流程一致。</remarks>
        private IEnumerator InitResources(IFsm<IProcedureModule> procedureOwner)
        {
            // 更新资源清单
            Log.Info("Update the manifest file...");
            LauncherMgr.ShowUI<LoadUpdateUI>(LoadText.Instance.Label_UpdateManifest);

            // 1. 获取资源清单的版本信息
            var operation1 = _resourceModule.RequestPackageVersionAsync();
            yield return operation1;
            if (operation1.Status != EOperationStatus.Succeed)
            {
                OnInitResourcesError(procedureOwner, operation1.Error);
                yield break;
            }

            var packageVersion = operation1.PackageVersion;
            _resourceModule.PackageVersion = packageVersion;

            SettingUtility.SetString(Constant.GAME_VERSION, _resourceModule.PackageVersion);

            Log.Info($"Init resource package version : {packageVersion}");

            // 2. 传入的版本信息更新资源清单
            var operation2 = _resourceModule.UpdatePackageManifestAsync(packageVersion);
            yield return operation2;
            if (operation2.Status != EOperationStatus.Succeed)
            {
                OnInitResourcesError(procedureOwner, operation2.Error);
                yield break;
            }

            _initResourcesComplete = true;
        }

        private void ChangeToPreloadState(IFsm<IProcedureModule> procedureOwner)
        {
            ChangeState<ProcedurePreload>(procedureOwner);
        }

        private void OnInitResourcesError(IFsm<IProcedureModule> procedureOwner, string message)
        {
            // 检查设备网络连接状态。
            if (_resourceModule.PlayMode == EPlayMode.HostPlayMode)
            {
                if (!IsNeedUpdate())
                {
                    return;
                }
                else
                {
                    Log.Error(message);
                    LauncherMgr.ShowMessageBox($"获取远程版本失败！点击确认重试\n <color=#FF0000>{message}</color>",
                        () => { UnityUtility.StartCoroutine(InitResources(procedureOwner)); }, Application.Quit);
                    return;
                }
            }

            Log.Error(message);
            LauncherMgr.ShowMessageBox($"初始化资源失败！点击确认重试 \n <color=#FF0000>{message}</color>",
                () => { UnityUtility.StartCoroutine(InitResources(procedureOwner)); }, Application.Quit);
        }

        private bool IsNeedUpdate()
        {
            // 如果不能联网且当前游戏非强制(不更新可以进入游戏。)
            if (UpdateSettings.UpdateStyle == EUpdateStyle.Optional && !_resourceModule.UpdatableWhilePlaying)
            {
                // 获取上次成功记录的版本
                string packageVersion = SettingUtility.GetString(Constant.GAME_VERSION, string.Empty);
                if (string.IsNullOrEmpty(packageVersion))
                {
                    LauncherMgr.ShowUI<LoadUpdateUI>(LoadText.Instance.Label_Net_UnReachable);
                    LauncherMgr.ShowMessageBox("没有找到本地版本记录，需要更新资源！",
                        () => { UnityUtility.StartCoroutine(InitResources(_procedureOwner)); },
                        Application.Quit);
                    return false;
                }

                _resourceModule.PackageVersion = packageVersion;

                if (UpdateSettings.UpdateNotice == EUpdateNotice.Notice)
                {
                    LauncherMgr.ShowUI<LoadUpdateUI>(LoadText.Instance.Label_Load_Notice);
                    LauncherMgr.ShowMessageBox($"更新失败，检测到可选资源更新，推荐完成更新提升游戏体验！ \\n \\n 确定再试一次，取消进入游戏",
                        () => { UnityUtility.StartCoroutine(InitResources(_procedureOwner)); },
                        () => { ChangeState<ProcedurePreload>(_procedureOwner); });
                }
                else
                {
                    ChangeState<ProcedurePreload>(_procedureOwner);
                }

                return false;
            }

            return true;
        }
    }
}