using System.ComponentModel;

namespace UpdateLib
{
    enum StatusConst
    {
        READY,
        CHECKING,
        DOWNLOADING,
        INSTALLING,
        NO_UPDATE,
        HAVE_UPDATE,
        ERROR,
    }

    public class Status : INotifyPropertyChanged
    {
        public Status()
        {
            StatusString = StatusConst.READY.ToString();
            DownloadPercent = 0;
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
