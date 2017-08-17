using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace TestAppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window 
    {
        static UpdateStatus upd = new UpdateStatus();

        public MainWindow()
        {
            InitializeComponent();

            ShowVersion();

            LblStatus.DataContext = upd;
            ProgressDownload.DataContext = upd;

            upd.StatusString = "READY";
            upd.DownloadPercent = 0;
        }

        private async void ChkBtn_Click(object sender, RoutedEventArgs e)
        {
            await ChechUpdateProcessAsync();

            ShowVersion();
        }

        private async Task ChechUpdateProcessAsync()
        {
            CheckBtn.IsEnabled = false;
            string path = @"C:\Program Files (x86)\TestCo\Update\1.3.99.0\GoogleUpdate.exe";

            var proc = new Process();
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.FileName = path;
            proc.StartInfo.Arguments = "/machine /ua /installsource ondemand";
            proc.Start();
            upd.UpdProcessId = proc.Id;

            await Task.Run(()=>CheckUpdateThreadFunction(upd));

            //var output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            //var exitcode = proc.ExitCode;
            CheckBtn.IsEnabled = true;
        }

        private void ShowVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            LblVersion.Content = "CurrentVersion: " + assembly.GetName().Version.ToString() ;
        }

        static string GetSelfGUID()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetCustomAttribute<GuidAttribute>().Value;
        }

        static string GetUpdateRegistryKeyValue(string subpath, string name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            string regKey = @"SOFTWARE\Wow6432Node\" + company + @"\Update\"+ subpath ;

            var reg = Registry.LocalMachine.OpenSubKey(regKey);
            var value = string.Empty;
            
            try
            {
                value = reg.GetValue(name).ToString();
            }
            catch
            {
            }

            return value;
        }

        static void GetDownloadPercents()
        {
            var tmp = GetUpdateRegistryKeyValue(@"ClientState\{" + GetSelfGUID() + @"}\CurrentState", "DownloadProgressPercent");
            if (!string.IsNullOrEmpty(tmp))
            {
                upd.DownloadPercent = int.Parse(tmp);
            }
        }

        static void CheckUpdateThreadFunction(object stateInfo)
        {
            UpdateStatus updstatus = (UpdateStatus)stateInfo;
            updstatus.StatusString = "CHECKING...";

            try
            {
                var updproc = new Process[] { };
                do
                {
                    updproc = Process.GetProcessesByName("GoogleUpdate");
                    GetDownloadPercents();
                }
                while (updproc.Length != 0);
            }
            catch
            {
            }

            GetDownloadPercents();
            updstatus.StatusString = "READY";
        }
    }
}
