using Microsoft.Win32;

namespace TestAppWPF
{
    static public class RegistryUtil
    {
        static public string GetUpdateRegistryKeyValue(string fullPath, string name)
        {
            var reg = Registry.LocalMachine.OpenSubKey(fullPath);

            if (reg!=null)
            {
                return reg.GetValue(name, string.Empty).ToString();
            }

            return string.Empty;
        }
    }
}
