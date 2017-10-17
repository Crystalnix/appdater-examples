using System;
using System.Diagnostics;
using System.IO;
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
        private ApplicationState appState;
        private AppBundleCOM comUpd;
        private AppBundleCMD cmdUpd;
        private Status updStatus;

        public MainWindow()
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();
            company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            appGUID = assembly.GetCustomAttribute<GuidAttribute>().Value;
            baseRegKey = @"SOFTWARE\Wow6432Node\" + company + @"\Update\";

            // Always Enable Omaha Log
            File.Copy("./Update.ini", logSettingsFilePath, true);

            appState = new ApplicationState();
            CheckBtnCmd.DataContext = appState;
            CheckBtnCOM.DataContext = appState;
            RestartBtn.DataContext = appState;

            updStatus = new Status();
            LblStatus.DataContext = updStatus;
            ProgressDownload.DataContext = updStatus;

            comUpd = new AppBundleCOM(appGUID, baseRegKey, ref updStatus);
            cmdUpd = new AppBundleCMD(appGUID, baseRegKey, ref updStatus);

            ShowVersion();
        }

        private void ShowVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            LblVersion.Content = assembly.GetName().Version.ToString();

            var registredVersion = RegistryUtil.GetRegistryKeyValue(baseRegKey + @"Clients\{" + appGUID + @"}", "pv");
            if (registredVersion.CompareTo(LblVersion.Content) != 0)
            {
                appState.NewVersionInstalled = true;
            }
        }

        private async void ChkBtnCmd_Click(object sender, RoutedEventArgs e)
        {
            appState.IsReadyToCheck = false;

            // run check update process
            await Task.Run(() => cmdUpd.CheckUpdateProcessAsync());

            appState.IsReadyToCheck = true;

            ShowVersion();
        }

        private async void ChkBtnCOM_Click(object sender, RoutedEventArgs e)
        {
            appState.IsReadyToCheck = false;

            if (!comUpd.Initialize())
            {
                appState.IsReadyToCheck = true;
                return;
            }

            // call COM wrapper methods
            if (await Task.Run(() => comUpd.CheckForUpdates()))
            {
                if (await Task.Run(() => comUpd.DownloadUpdates()))
                {
                    await Task.Run(() => comUpd.InstallUpdates());
                }
            }

            comUpd.DeInitialize();
            appState.IsReadyToCheck = true;

            ShowVersion();
        }

        private void RestartBtn_Click(object sender, RoutedEventArgs e)
        {
            var exe_path = Assembly.GetExecutingAssembly().Location;
            var tokens = exe_path.Split('\\');
            var linkfile = "..\\" + tokens[tokens.Length - 1].Replace(".exe",".lnk");

            if (File.Exists(linkfile))
            {
                var info = new ProcessStartInfo("cmd");
                info.WindowStyle = ProcessWindowStyle.Hidden;
                info.WorkingDirectory = Path.GetDirectoryName(exe_path);
                info.Arguments = "/C " + linkfile;
                Process.Start(info);
            }

            Application.Current.Shutdown();
        }
    }
}
