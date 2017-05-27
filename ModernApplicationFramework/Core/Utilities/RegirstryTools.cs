using Microsoft.Win32;

namespace ModernApplicationFramework.Core.Utilities
{
    public static class RegirstryTools
    {
        private const string RegistryKeyHive = "HKEY_CURRENT_USER\\";


        public static bool ExistsCurrentUserRoot(string regKeyPath)
        {
            return Registry.CurrentUser.OpenSubKey(regKeyPath) != null;
        }

        public static void CreateCurrentUserRoot(string regKeyPath)
        {
            Registry.CurrentUser.CreateSubKey(regKeyPath);
        }



        public static object GetValueCurrentUserRoot(string regKeyPath, string regKeyName, object defaultOnError = null)
        {
            return GetValue(RegistryKeyHive, regKeyPath, regKeyName, defaultOnError);
        }

        public static void SetValueCurrentUserRoot(string regKeyPath, string regKeyName, object value)
        {
            SetValue(RegistryKeyHive, regKeyPath, regKeyName, value);
        }

        private static object GetValue(string regKeyRoot, string regKeyPath, string regKeyName, object defaultOnError)
        {
            string keyName = regKeyRoot + regKeyPath;
            object obj = defaultOnError;
            try
            {
                obj = Registry.GetValue(keyName, regKeyName, defaultOnError) ?? defaultOnError;
            }
            catch (System.Exception)
            {
                // ignored
            }
            return obj;
        }

        private static void SetValue(string regKeyRoot, string regKeyPath, string regKeyName, object value)
        {
            string keyName = regKeyRoot + regKeyPath;
            try
            {
                Registry.SetValue(keyName, regKeyName, value);
            }
            catch (System.Exception)
            {
                // ignored
            }
        }
    }
}