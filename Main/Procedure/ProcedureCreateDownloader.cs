using System;
using Cysharp.Threading.Tasks;
using Moirai.Atropos;
using Moirai.Atropos.Fsm;
using Moirai.Atropos.Procedure;
using UnityEngine;
using YooAsset;

namespace Moirai.Main
{
    /// <summary>
    /// 流程 => 创建补丁下载器
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProcedureCreateDownloader : ProcedurePremainBase
    {
        private int _curTryCount;
        
        private const int MAX_TRY_COUNT = 3;
        
        public override bool UseNativeDialog { get; }

        private IFsm<IProcedureModule> _procedureOwner;

        private ResourceDownloaderOperation _downloader;

        private int _totalDownloadCount;

        private string _totalSizeMb;

        protected override void OnEnter(IFsm<IProcedureModule> procedureOwner)
        {
            _procedureOwner = procedureOwner;
            
            Log.Info("Create a patch downloader...");
            
            LauncherMgr.ShowUI<LoadUpdateUI>(LoadText.Instance.Label_CreateDownloader);
            
            CreateDownloader().Forget();
        }

        private async UniTaskVoid CreateDownloader()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            _downloader = _resourceModule.CreateResourceDownloader();

            if (_downloader.TotalDownloadCount == 0)
            {
                Log.Info("Not found any download files !");
                ChangeState<ProcedureDownloadOver>(_procedureOwner);
            }
            else
            {
                // 一共发现 n 个文件需要下载
                Log.Info($"Found total {_downloader.TotalDownloadCount} files that need download ！");

                // 发现新更新文件后，挂起流程系统
                // 注意：开发者需要在下载前检测磁盘空间不足
                _totalDownloadCount = _downloader.TotalDownloadCount;
                long totalDownloadBytes = _downloader.TotalDownloadBytes;

                float sizeMb = totalDownloadBytes / 1048576f;
                sizeMb = Mathf.Clamp(sizeMb, 0.1f, float.MaxValue);
                _totalSizeMb = sizeMb.ToString("f1");

                // if (!SettingsUtils.EnableUpdateData())
                {
                    LauncherMgr.ShowMessageBox($"Found update patch files, Total count {_totalDownloadCount} Total size {_totalSizeMb}MB",
                        StartDownFile, Application.Quit);
                }
                // else
                // {
                //     RequestUpdateData().Forget();
                // }
            }
        }

        void StartDownFile()
        {
            ChangeState<ProcedureDownloadFile>(_procedureOwner);
        }
        

        /// <summary>
        /// 请求更新配置数据。
        /// </summary>
        private async UniTaskVoid RequestUpdateData()
        {
            Log.Warning("On RequestVersion");
            _curTryCount++;

            if (_curTryCount > MAX_TRY_COUNT)
            {
                LauncherMgr.ShowMessageBox(LoadText.Instance.Label_Net_Error,
                    () =>
                    {
                        _curTryCount = 0;
                        RequestUpdateData().Forget();
                    }, Application.Quit);
                return;
            }

            var checkVersionUrl = UpdateSettings.GetResDownLoadPath();

            LauncherMgr.ShowUI<LoadUpdateUI>(string.Format(LoadText.Instance.Label_Load_Checking, _curTryCount));
            if (string.IsNullOrEmpty(checkVersionUrl))
            {
                Log.Error("LoadMgr.RequestVersion, remote url is empty or null");
                LauncherMgr.ShowMessageBox(LoadText.Instance.Label_RemoteUrlIsNull,
                    Application.Quit);
                return;
            }
            Log.Info("RequestUpdateData, proxy:" + checkVersionUrl);

            var updateDataStr = await HttpUtility.Get(checkVersionUrl);

            try
            {
                UpdateData updateData = JSONUtility.ToObject<UpdateData>(updateDataStr);
                ShowUpdateType(updateData);
            }
            catch (Exception e)
            {
                Log.Fatal(e);
                throw;
            }
        }

        /// <summary>
        /// 显示更新方式
        /// </summary>
        /// <returns></returns>
        private void ShowUpdateType(UpdateData data)
        {
            LauncherMgr.ShowUI<LoadUpdateUI>(LoadText.Instance.Label_Load_Checked);
            // 底包更新
            if (data.UpdateType == UpdateType.PackageUpdate)
            {
                LauncherMgr.ShowMessageBox(LoadText.Instance.Label_Load_Package,
                    () =>
                    {
                        // 自定义下载APK
                        Log.Info("Customize the download APK");
                        Application.Quit();
                    });
            }
            // 资源更新
            else if (data.UpdateType == UpdateType.ResourceUpdate)
            {
                // 强制
                if (data.UpdateStyle == EUpdateStyle.Force)
                {
                    // 提示
                    if (data.UpdateNotice == EUpdateNotice.Notice)
                    {
                        NetworkReachability networkReachability = Application.internetReachability;
                        string desc = LoadText.Instance.Label_Load_Force_WIFI;
                        if (networkReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                        {
                            desc = LoadText.Instance.Label_Load_Force_NO_WIFI;
                        }
                        desc = string.Format(desc, $"{_totalSizeMb}MB");
                        LauncherMgr.ShowMessageBox(desc,
                            StartDownFile, Application.Quit);
                    }
                    // 不提示
                    else if (data.UpdateNotice == EUpdateNotice.NoNotice)
                    {
                        StartDownFile();
                    }
                }
                // 非强制
                else if (data.UpdateStyle == EUpdateStyle.Optional)
                {
                    // 提示
                    if (data.UpdateNotice == EUpdateNotice.Notice)
                    {
                        LauncherMgr.ShowMessageBox(string.Format(LoadText.Instance.Label_Load_Notice,$"{_totalSizeMb}MB"),
                            StartDownFile, () =>
                            {
                                ChangeState<ProcedureLoadAssembly>(_procedureOwner);
                            });
                    }
                    // 不提示
                    else if (data.UpdateNotice == EUpdateNotice.NoNotice)
                    {
                        StartDownFile();
                    }
                }
                else
                {
                    Log.Error("LoadMgr._CheckUpdate, style is error,code:" + data.UpdateStyle);
                }
            }
        }
    }
}