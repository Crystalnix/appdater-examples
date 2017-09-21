using System;
using System.Threading.Tasks;
using GoogleUpdate3Lib;
using System.Threading;
using System.Runtime.InteropServices;

namespace UpdateLib
{
    public class AppBundleCOM : IDisposable
    {
        private static string logFilePath = @"C:\Users\Public\Documents\UpdateCOM.log";
        private IAppBundleWeb appBundle;
        public Status status;

        public string registryBasePath { get; private set; }
        public string GUID { get; private set; }

        public AppBundleCOM(string appGUID, string regBasePath)
        {
            status = new Status();
            GUID = appGUID;
            registryBasePath = regBasePath;
        }

        ~AppBundleCOM()
        {
            DeInitialize();
        }

        public void Dispose()
        {
            DeInitialize();
        }

        public bool Initialize()
        {
            var COM_obj = new GoogleUpdate3WebMachineClass();
            var class_obj = (IGoogleUpdate3Web)COM_obj;
            appBundle = (IAppBundleWeb)class_obj.createAppBundleWeb();
            appBundle.initialize();
            try
            {
                if (!string.IsNullOrEmpty(GUID))
                {
                    appBundle.createInstalledApp("{" + GUID + "}");     // Particulary application
                    var appweb = (IAppWeb)appBundle[0];
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

        public async Task InstallUpdates()
        {
            DeInitialize();

            var cmd = new AppBundleCMD(GUID, registryBasePath);
            await Task.Run(() => cmd.InstallUpdate());
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

            status.StatusString = string.Empty;
            if (AppInCurrentStateCount(appBundle, currentState.STATE_ERROR) != 0)
            {
                status.StatusString += " " + StatusConst.ERROR.ToString();
            }
            else if (AppInCurrentStateCount(appBundle, currentState.STATE_NO_UPDATE) != 0)
            {
                status.StatusString += " " + StatusConst.NO_UPDATE.ToString();
            }
            else if (AppInCurrentStateCount(appBundle, currentState.STATE_UPDATE_AVAILABLE) != 0)
            {
                status.StatusString += " " + StatusConst.HAVE_UPDATE.ToString();
            }
            else
            {
                status.StatusString = StatusConst.READY.ToString();
            }
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
                        AppInCurrentStateCount(appBundle, currentState.STATE_INSTALLING) == 0 &&
                        AppInCurrentStateCount(appBundle, currentState.STATE_WAITING_TO_INSTALL) == 0
                        )
                    {
                        AppInCurrentStateCount(appBundle, currentState.STATE_WAITING_TO_INSTALL);
                        return;
                    }
                }
                Thread.Sleep(500);
            }
        }
    }
}
