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
        private string company;
        private string appGUID;
        private UpdateStatus upd;
        private AppBundleCOM com;
        
        public MainWindow()
        {
            InitializeComponent();

            ShowVersion();

            Assembly assembly = Assembly.GetExecutingAssembly();
            company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            appGUID = assembly.GetCustomAttribute<GuidAttribute>().Value;
        }

        private async void ChkBtnCmd_Click(object sender, RoutedEventArgs e)
        {
            CheckBtnCOM.IsEnabled = false;
            CheckBtnCmd.IsEnabled = false;

            // create object
            upd = new UpdateStatus();
            upd.registryBasePath = @"SOFTWARE\Wow6432Node\" + company + @"\Update\";
            upd.GUID = appGUID;

            // binding
            LblStatus.DataContext = upd;
            ProgressDownload.DataContext = upd;
            upd.StatusString = "READY";
            upd.DownloadPercent = 0;

            // run new process
            await ChechUpdateProcessAsync();

            ShowVersion();

            CheckBtnCOM.IsEnabled = true;
            CheckBtnCmd.IsEnabled = true;
        }

        private async void ChkBtnCOM_Click(object sender, RoutedEventArgs e)
        {
            CheckBtnCOM.IsEnabled = false;
            CheckBtnCmd.IsEnabled = false;

            // create COM wrapper
            com = new AppBundleCOM();
            if (!com.Initialize(appGUID))
            {
                CheckBtnCOM.IsEnabled = true;
                CheckBtnCmd.IsEnabled = true;

                return;
            }

            // binding
            LblStatus.DataContext = com;
            ProgressDownload.DataContext = com;
            com.StatusString = "READY";
            com.DownloadPercent = 0;

            // call COM wrapper methods
            if (await Task.Run(() => com.CheckForUpdates()))
            {
                if (await Task.Run(() => com.DownloadUpdates()))
                {
                    await Task.Run(() => com.InstallUpdates());
                }
            }

            com.DeInitialize();

            CheckBtnCOM.IsEnabled = true;
            CheckBtnCmd.IsEnabled = true;
        }

        private async Task ChechUpdateProcessAsync()
        {
            string path = @"C:\Program Files (x86)\Crystalnix\Update\Update.exe";

            var proc = new Process();
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.FileName = path;
            proc.StartInfo.Arguments = "/machine /ua /installsource ondemand"; // No Omaha Update GUI
            //proc.StartInfo.Arguments = "/machine /ua";                         // With Omaha Update GUI
            proc.Start();

            await Task.Run(() => upd.GetValuesFromRegistry());

            proc.WaitForExit();
        }

        private void ShowVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            LblVersion.Content = "CurrentVersion: " + assembly.GetName().Version.ToString();
        }
    }
}
