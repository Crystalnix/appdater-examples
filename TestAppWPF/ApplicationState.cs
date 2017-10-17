using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppWPF
{
    class ApplicationState : INotifyPropertyChanged
    {
        public ApplicationState()
        {
            IsReadyToCheck = true;
            NewVersionInstalled = false;
        }

        private bool _isReadyToCheck;
        public bool IsReadyToCheck
        {
            get { return _isReadyToCheck; }
            set { _isReadyToCheck = value; OnPropertyChanged(nameof(IsReadyToCheck)); }
        }

        private bool _newVersionInstalled;
        public bool NewVersionInstalled
        {
            get { return _newVersionInstalled; }
            set { _newVersionInstalled = value; OnPropertyChanged(nameof(NewVersionInstalled)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
