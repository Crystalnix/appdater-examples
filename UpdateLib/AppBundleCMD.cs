using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace UpdateLib
{
    public class AppBundleCMD
    {
        public Status status;
        public string registryBasePath { get; private set; }
        public string GUID { get; private set; }

        public AppBundleCMD(string appGUID, string regBasePath)
        {
            status = new Status();

            GUID = appGUID;
            registryBasePath = regBasePath;
        }

        public async Task GetValuesFromRegistry()
        {
            await Task.Run(() => GetValues());
        }

        public async Task CheckUpdateProcessAsync()
        {
            string path = @"C:\Program Files (x86)\Crystalnix\Update\Update.exe";
            var proc = new Process();
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.FileName = path;
            proc.StartInfo.Arguments = "/machine /ua /installsource ondemand"; // No Omaha Update GUI
            //proc.StartInfo.Arguments = "/machine /ua";                       // With Omaha Update GUI
            proc.Start();

            await Task.Run(() => GetValuesFromRegistry());
        }

        private void GetDownloadPercentsFromRegistry()
        {
            var tmp = RegistryUtil.GetUpdateRegistryKeyValue(registryBasePath + @"ClientState\{" + GUID + @"}\CurrentState", "DownloadProgressPercent");
            if (!string.IsNullOrEmpty(tmp))
            {
                status.DownloadPercent = uint.Parse(tmp);
            }
        }

        private void GetValues()
        {
            status.StatusString = StatusConst.CHECKING.ToString();
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
            status.StatusString = StatusConst.READY.ToString();
        }
    }
}
