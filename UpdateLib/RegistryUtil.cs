using Microsoft.Win32;

namespace UpdateLib
{
    static public class RegistryUtil
    {
        static public string GetRegistryKeyValue(string fullPath, string name)
        {
            var reg = Registry.LocalMachine.OpenSubKey(fullPath);

            if (reg!=null)
            {
                return reg.GetValue(name, string.Empty).ToString();
            }

            return string.Empty;
        }

        static public void SetRegistryKeyValue(string fullPath, string name, object value)
        {
            var reg = Registry.LocalMachine.OpenSubKey(fullPath);

            if (reg != null)
            {
                reg.SetValue(name, value);
            }
        }
    }
}
