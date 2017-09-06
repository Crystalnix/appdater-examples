using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TestAppWPF
{
    public class UpdateStatus: INotifyPropertyChanged
    {
        private string _ststring;
        public string StatusString
        {
            get { return _ststring; }
            set { _ststring = value; OnPropertyChanged(nameof(StatusString)); }
        }

        private int _downloadpercentage;
        public int DownloadPercent
        {
            get { return _downloadpercentage; }
            set { _downloadpercentage = value; OnPropertyChanged(nameof(DownloadPercent)); }
        }

        public string registryBasePath;
        public string GUID;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }


        private void GetDownloadPercentsFromRegistry()
        {
            var tmp = RegistryUtil.GetUpdateRegistryKeyValue(registryBasePath + @"ClientState\{" + GUID + @"}\CurrentState", "DownloadProgressPercent");
            if (!string.IsNullOrEmpty(tmp))
            {
                DownloadPercent = int.Parse(tmp);
            }
        }

        public async Task GetValuesFromRegistry()
        {
            await Task.Run(() => GetValues(false));
        }

        public async Task GetValuesFromCOMObject()
        {
            await Task.Run(() => GetValues(true));
        }

        private async Task GetValues(bool isCOM)
        {
            StatusString = "CHECKING...";
            try
            {
                var updproc = new Process[] { };
                do
                {
                    updproc = Process.GetProcessesByName("Update");
                    GetDownloadPercentsFromRegistry();
                }
                while (updproc.Length != 0);
            }
            catch
            {
            }

            GetDownloadPercentsFromRegistry();
            StatusString = "READY";
        }
    }
}
