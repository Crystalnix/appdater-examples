using System.ComponentModel;

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

        public int UpdProcessId;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
