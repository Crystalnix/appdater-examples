using System;
using System.Threading.Tasks;
using GoogleUpdate3Lib;
using System.ComponentModel;
using System.Threading;
using System.Runtime.InteropServices;

namespace TestAppWPF
{
    public class AppBundleCOM : INotifyPropertyChanged
    {
        private string _ststring;
        public string StatusString
        {
            get { return _ststring; }
            set { _ststring = value; OnPropertyChanged(nameof(StatusString)); }
        }

        private uint _downloadpercentage;
        public uint DownloadPercent
        {
            get { return _downloadpercentage; }
            set { _downloadpercentage = value; OnPropertyChanged(nameof(DownloadPercent)); }
        }

        IAppBundleWeb appBundle;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
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

        public void DeInitialize()
        {
            while (Marshal.ReleaseComObject(appBundle) > 0) ;
            appBundle = null;
            GC.Collect();
        }

        public async Task<bool> CheckForUpdates()
        {
            appBundle.checkForUpdate();
            //await Task.Run(() => GetCurrentStateValues());

            var waitingThread = new Thread(WaitingThread);
            waitingThread.Start(appBundle);
            waitingThread.Join();

            return AppInCurrentStateCount(appBundle, currentState.STATE_UPDATE_AVAILABLE) > 0;
        }

        public async Task<bool> DownloadUpdates()
        {
            appBundle.download();
            //await Task.Run(() => GetCurrentStateValues());

            var waitingThread = new Thread(WaitingThread);
            waitingThread.Start(appBundle);
            waitingThread.Join();

            return AppInCurrentStateCount(appBundle, currentState.STATE_READY_TO_INSTALL) == appBundle.length;
        }

        public async Task<bool> InstallUpdates()
        {
            appBundle.install();
            //await Task.Run(() => GetCurrentStateValues());

            return true;
        }

        static private int AppInCurrentStateCount(IAppBundleWeb appBundle, currentState state)
        {
            int count = 0;
            for (int appIndex = 0; appIndex < appBundle.length; ++appIndex)
            {
                var currState = (ICurrentState)appBundle[appIndex].currentState;
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
            bool repeat = true;
            while (repeat && appBundle != null)
            {
                repeat = false;
                if (AppInCurrentStateCount(appBundle, currentState.STATE_CHECKING_FOR_UPDATE) != 0)
                {
                    StatusString = "CHECKING...";
                    repeat = true;
                }
                if (AppInCurrentStateCount(appBundle, currentState.STATE_DOWNLOADING) != 0)
                {
                    StatusString = "DOWNLOADING...";
                    double downloaded = AppTotalBytesDownloaded(appBundle);
                    double total = AppTotalBytesToDownload(appBundle);
                    DownloadPercent = (uint)((downloaded / total)*100);
                    repeat = true;
                }
                if (AppInCurrentStateCount(appBundle, currentState.STATE_INSTALLING) != 0)
                {
                    StatusString = "INSTALLING...";
                    repeat = true;
                }
            }
            StatusString = "READY";
        }

        static void WaitingThread(object infoState)
        {
            IAppBundleWeb appBundle = (IAppBundleWeb)infoState;

            while (true)
            {
                for (int appIndex = 0; appIndex < appBundle.length; ++appIndex)
                {
                    if (AppInCurrentStateCount(appBundle, currentState.STATE_CHECKING_FOR_UPDATE) == 0 &&
                        AppInCurrentStateCount(appBundle, currentState.STATE_DOWNLOADING) == 0)
                    {
                        return;
                    }
                }
                Thread.Sleep(500);
            }
        }
    }
}
