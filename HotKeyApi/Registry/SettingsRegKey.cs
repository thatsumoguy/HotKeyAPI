using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;

namespace HotKeyApi
{
    public static class SettingsRegKey
    {
        public static string _appName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
        /// <summary>
        /// Sets registry value for Config path. This must be given the Application Path and Application Name.
        /// </summary>
        /// <param name="path">Path to application</param>
        /// <param name="appName">Application name</param>
        public static void SetRegKey(string path)
        {
            using (var key = Registry.CurrentUser.OpenSubKey("Software", writable: true))
            {
                if (!key.GetSubKeyNames().Contains(_appName))
                {
                    key.CreateSubKey(_appName);
                }
                using (var hotKey = key.OpenSubKey(_appName, writable: true))
                {
                    try
                    {
                        hotKey.SetValue("ConfigPath", path);
                    }
                    catch (ArgumentNullException)
                    {
                        throw new ArgumentNullException("The provided path is null");
                    }
                    catch (SecurityException)
                    {
                        throw new SecurityException("You are not authorized to write to the registry");
                    }
                }
            }
        }
        /// <summary>
        /// Get the current Registry key value for you Application key
        /// </summary>
        /// <returns>string</returns>
        public static string GetRegKey()
        {
            return (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + _appName, "ConfigPath", "Value has not been set");
        }
        /// <summary>
        /// Unregistered the key used for Config path.
        /// </summary>
        /// <param name="appName">Application Name</param>
        public static void UnregisterKey(string appName)
        {
            using (var key = Registry.CurrentUser.OpenSubKey("Software", writable: true))
            {
                if (key.GetSubKeyNames().Contains(appName))
                {
                    key.DeleteSubKey(appName);
                }
            }
        }
    }
}
