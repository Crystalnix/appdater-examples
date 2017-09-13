using System;
using System.Threading.Tasks;
using GoogleUpdate3Lib;
using System.ComponentModel;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace UpdateLib
{
    public class AppBundleCOM : IDisposable
    {
        private static string logFilePath = @"C:\Users\Public\Documents\UpdateCOM.log";
        private static string appGUID;
        private IAppBundleWeb appBundle;
        public Status status;

        public AppBundleCOM()
        {
            status = new Status();
        }

        public bool Initialize(string appGUID = null)
        {
            var cobj = new GoogleUpdate3WebMachineClass();
            var obj = (IGoogleUpdate3Web)cobj;
            appBundle = (IAppBundleWeb)obj.createAppBundleWeb();
            appBundle.initialize();
            try
            {
                if (!string.IsNullOrEmpty(appGUID))
                {
                    AppBundleCOM.appGUID = appGUID;
                    appBundle.createInstalledApp("{" + appGUID + "}");     // Particulary application
                }
                else
                {
                    appBundle.createAllInstalledApps();     // All installed application
                }
            }
            catch
            {
                DeInitialize();
                return false;
            }

            return true;
        }

        ~AppBundleCOM()
        {
            DeInitialize();
        }

        public void DeInitialize()
        {
            if (appBundle != null)
            {
                while (Marshal.ReleaseComObject(appBundle) > 0) ;
                appBundle = null;
            }
            GC.Collect();
        }

        public async Task<bool> CheckForUpdates()
        {
            ToLog(new[] { "", "Wait while checking updates" });

            appBundle.checkForUpdate();
            await Task.Run(() => GetCurrentStateValues());

            var waitingThread = new Thread(WaitingThread);
            waitingThread.Start(appBundle);
            waitingThread.Join();
            ToLog(new string[] { "Done!" });

            return AppInCurrentStateCount(appBundle, currentState.STATE_UPDATE_AVAILABLE, true) > 0;
        }

        public async Task<bool> DownloadUpdates()
        {
            ToLog(new[] { "", "Wait while downloading updates" });

            appBundle.download();
            await Task.Run(() => GetCurrentStateValues());

            var waitingThread = new Thread(WaitingThread);
            waitingThread.Start(appBundle);
            waitingThread.Join();
            ToLog(new string[] { "Done!" });

            return AppInCurrentStateCount(appBundle, currentState.STATE_READY_TO_INSTALL, true) == appBundle.length;
        }

        public async Task<bool> InstallUpdates()
        {
            ToLog(new[] { "", "Wait while installing" });

            appBundle.install();
            await Task.Run(() => GetCurrentStateValues());

            var waitingThread = new Thread(WaitingThread);
            waitingThread.Start(appBundle);
            waitingThread.Join();
            ToLog(new string[] { "Done!" });

            return AppInCurrentStateCount(appBundle, currentState.STATE_INSTALL_COMPLETE, true) == appBundle.length;
        }

        static private void ToLog(string[] lines)
        {
            try
            {
                System.IO.File.AppendAllLines(logFilePath, lines);
            }
            catch { }
        }

        static private int AppInCurrentStateCount(IAppBundleWeb appBundle, currentState state, bool logStates = false)
        {
            int count = 0;
            for (int appIndex = 0; appIndex < appBundle.length; ++appIndex)
            {
                var currState = (ICurrentState)appBundle[appIndex].currentState;

                if (logStates)
                {
                    ToLog(new[] { ((currentState)currState.stateValue).ToString() });
                }

                if (currState.stateValue == (int)state)
                {
                    ++count;
                }
            }
            return count;
        }

        static private uint AppTotalBytesToDownload(IAppBundleWeb appBundle)
        {
            uint bytesCount = 0;
            for (int appIndex = 0; appIndex < appBundle.length; ++appIndex)
            {
                var currState = (ICurrentState)appBundle[appIndex].currentState;
                bytesCount += currState.totalBytesToDownload;
            }
            return bytesCount;
        }

        static private uint AppTotalBytesDownloaded(IAppBundleWeb appBundle)
        {
            uint bytesCount = 0;
            for (int appIndex = 0; appIndex < appBundle.length; ++appIndex)
            {
                var currState = (ICurrentState)appBundle[appIndex].currentState;
                bytesCount += currState.bytesDownloaded;
            }
            return bytesCount;
        }

        private async Task GetCurrentStateValues()
        {
            await Task.Run(()=> GetValues());
        }

        private void GetValues()
        {
            bool repeat = true;

            while (repeat && appBundle != null)
            {
                repeat = false;
                if (AppInCurrentStateCount(appBundle, currentState.STATE_CHECKING_FOR_UPDATE) != 0)
                {
                    status.StatusString = StatusConst.CHECKING.ToString();
                    repeat = true;
                }
                if (AppInCurrentStateCount(appBundle, currentState.STATE_DOWNLOADING) != 0)
                {
                    status.StatusString = StatusConst.DOWNLOADING.ToString();
                    double downloaded = AppTotalBytesDownloaded(appBundle);
                    double total = AppTotalBytesToDownload(appBundle);
                    status.DownloadPercent = (uint)((downloaded / total) * 100);
                    repeat = true;
                }
                if (AppInCurrentStateCount(appBundle, currentState.STATE_INSTALLING) != 0)
                {
                    status.StatusString = StatusConst.INSTALLING.ToString();
                    repeat = true;
                }
            }
            status.StatusString = StatusConst.READY.ToString();
        }
        
        static void WaitingThread(object infoState)
        {
            IAppBundleWeb appBundle = (IAppBundleWeb)infoState;
            while (true)
            {
                for (int appIndex = 0; appIndex < appBundle.length; ++appIndex)
                {
                    if (AppInCurrentStateCount(appBundle, currentState.STATE_CHECKING_FOR_UPDATE) == 0 &&
                        AppInCurrentStateCount(appBundle, currentState.STATE_WAITING_TO_DOWNLOAD) == 0 &&
                        AppInCurrentStateCount(appBundle, currentState.STATE_DOWNLOADING) == 0 &&
                        AppInCurrentStateCount(appBundle, currentState.STATE_INSTALLING) == 0
                        )
                    {
                        return;
                    }
                }
                Thread.Sleep(500);
            }
        }

        public void Dispose()
        {
            DeInitialize();
        }
    }
}
