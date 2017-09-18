using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UpdateLib;

namespace TestAppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string company;
        private string appGUID;
        private string logSettingsFilePath = @"C:\Update.ini";
        private string baseRegKey;

        public MainWindow()
        {
            InitializeComponent();

            ShowVersion();

            Assembly assembly = Assembly.GetExecutingAssembly();
            company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            appGUID = assembly.GetCustomAttribute<GuidAttribute>().Value;
            baseRegKey = @"SOFTWARE\Wow6432Node\" + company + @"\Update\";

            // Always Enable Omaha Log
            System.IO.File.Copy("./Update.ini", logSettingsFilePath, true);

            ThreadPool.QueueUserWorkItem(CloseInstanceOnUpdate, baseRegKey + @"Clients\{" + appGUID +@"}");
        }

        private void ShowVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            LblVersion.Content = assembly.GetName().Version.ToString();
        }

        private async void ChkBtnCmd_Click(object sender, RoutedEventArgs e)
        {
            CheckBtnCOM.IsEnabled = false;
            CheckBtnCmd.IsEnabled = false;

            // create object
            var upd = new AppBundleCMD(appGUID, baseRegKey);

            // binding
            LblStatus.DataContext = upd.status;
            ProgressDownload.DataContext = upd.status;

            // run check update process
            if (MessageBox.Show("Close all instances of application for update?", "Warrning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await Task.Run(() => upd.CheckUpdateProcessAsync());
            }

            ShowVersion();

            CheckBtnCOM.IsEnabled = true;
            CheckBtnCmd.IsEnabled = true;
        }

        private async void ChkBtnCOM_Click(object sender, RoutedEventArgs e)
        {
            CheckBtnCOM.IsEnabled = false;
            CheckBtnCmd.IsEnabled = false;

            // create COM wrapper
            var com = new AppBundleCOM(appGUID, baseRegKey);
            if (!com.Initialize())
            {
                CheckBtnCOM.IsEnabled = true;
                CheckBtnCmd.IsEnabled = true;
                return;
            }

            // binding
            LblStatus.DataContext = com.status;
            ProgressDownload.DataContext = com.status;

            // call COM wrapper methods
            if (await Task.Run(() => com.CheckForUpdates()))
            {
                if (await Task.Run(() => com.DownloadUpdates()))
                {
                    if (MessageBox.Show("Close all instances of application for update?", "Warrning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        await Task.Run(() => com.InstallUpdates());
                    }
                }
            }

            com.DeInitialize();

            CheckBtnCOM.IsEnabled = true;
            CheckBtnCmd.IsEnabled = true;
        }

        private static void CloseInstanceOnUpdate(object stateInfo)
        {
            var regKey = (string)stateInfo;
            if (string.IsNullOrEmpty(regKey))
            {
                return;
            }
            Process p = Process.GetCurrentProcess();
            int ival;
            while (true)
            {
                var val = RegistryUtil.GetRegistryKeyValue(regKey, "update");
                if (int.TryParse(val, out ival))
                {
                    if (ival == 1)
                    {
                        p.CloseMainWindow();
                    }
                }
                Thread.Sleep(10);
            }
        }
    }
}
