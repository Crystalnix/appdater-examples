using System.Diagnostics;
using System.Threading.Tasks;

namespace UpdateLib
{
    public class AppBundleCMD
    {
        public Status status;
        public string registryBasePath { get; private set; }
        public string GUID { get; private set; }
        private string quitFlagPath;
        private string quitFlagName;
        private string updatePath;

        public AppBundleCMD(string appGUID, string regBasePath)
        {
            status = new Status();

            GUID = appGUID;
            registryBasePath = regBasePath;
            updatePath = RegistryUtil.GetRegistryKeyValue(registryBasePath, "path");
            quitFlagPath = registryBasePath + @"ClientState\{" + GUID + @"}\CurrentState";
            quitFlagName = "DownloadProgressPercent";
        }

        public async Task GetValuesFromRegistry()
        {
            await Task.Run(() => GetValues());
        }

        public async Task CheckUpdateProcessAsync()
        {
            if (string.IsNullOrEmpty(updatePath)) { return; }
            string path = updatePath;
            var proc = new Process();
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.FileName = path;
            proc.StartInfo.Arguments = "/machine /ua /installsource ondemand"; // No Omaha Update GUI
            //proc.StartInfo.Arguments = "/machine /ua";                       // With Omaha Update GUI
            proc.Start();

            await Task.Run(() => GetValuesFromRegistry());
        }

        internal void InstallUpdate()
        {
            if (string.IsNullOrEmpty(updatePath)) { return; }
            string path = updatePath;
            var proc = new Process();
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.FileName = path;
            proc.StartInfo.Arguments = "/silent /handoff \"appguid={" + GUID + "}\"";
            proc.Start();
        }

        private void GetDownloadPercentsFromRegistry()
        {
            var tmp = RegistryUtil.GetRegistryKeyValue(quitFlagPath, quitFlagName);
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
