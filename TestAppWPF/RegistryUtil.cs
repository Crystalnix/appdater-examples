using Microsoft.Win32;

namespace TestAppWPF
{
    static public class RegistryUtil
    {
        static public string GetUpdateRegistryKeyValue(string fullPath, string name)
        {
            var reg = Registry.LocalMachine.OpenSubKey(fullPath);
            var value = string.Empty;

            try
            {
                value = reg.GetValue(name).ToString();
            }
            catch { }
            
            return value;
        }
    }
}
