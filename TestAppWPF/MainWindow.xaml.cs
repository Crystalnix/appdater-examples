using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
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

        public MainWindow()
        {
            InitializeComponent();

            ShowVersion();

            Assembly assembly = Assembly.GetExecutingAssembly();
            company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            appGUID = assembly.GetCustomAttribute<GuidAttribute>().Value;

            // Always Enable Omaha Log
            System.IO.File.Copy("./Update.ini", logSettingsFilePath, true);
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
            var upd = new AppBundleCMD(appGUID, @"SOFTWARE\Wow6432Node\" + company + @"\Update\");

            // binding
            LblStatus.DataContext = upd.status;
            ProgressDownload.DataContext = upd.status;

            // run check update process
            await Task.Run(() => upd.CheckUpdateProcessAsync());

            ShowVersion();

            CheckBtnCOM.IsEnabled = true;
            CheckBtnCmd.IsEnabled = true;
        }

        private async void ChkBtnCOM_Click(object sender, RoutedEventArgs e)
        {
            CheckBtnCOM.IsEnabled = false;
            CheckBtnCmd.IsEnabled = false;

            // create COM wrapper
            var com = new AppBundleCOM();
            if (!com.Initialize(appGUID))
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
                    await Task.Run(() => com.InstallUpdates());
                }
            }

            com.DeInitialize();

            CheckBtnCOM.IsEnabled = true;
            CheckBtnCmd.IsEnabled = true;
        }
    }
}
