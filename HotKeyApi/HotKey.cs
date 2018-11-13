using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace HotKeyApi
{
    public static class HotKey
    {
        private static KeyboardHook KeyboardHook = new KeyboardHook();


        /// <summary>
        /// Determines whether the provided hotkey action is a powershell script or not. 
        /// Use this to then work the PowerShellOut string to handle the output from powershell.
        /// </summary>
        public static bool PowerShell { get; private set; }


        /// <summary>
        /// The output from any powershell command ran.
        /// This will output in a format similar to how powershell would output to std output.
        /// </summary>
        public static string PowerShellOut { get; private set; }

        /// <summary>
        /// Event handler when Window detects a WM_Hotkey message
        /// </summary>
        public static event EventHandler<KeyboardHookCallEventArgs> KeyPressed;

        /// <summary>
        /// Reads the JSON file supplied in <code>Config.json</code> and supplied back a <code>List<App>(App)</App></code> where you can extract the info.
        /// </summary>
        /// <returns>List<App>(App)</App>></returns>
        public static List<App> Keys() => ReadJSON.GetKeys();

        /// <summary>
        /// Converts the created App and adds it to the json structure and overwrites config.json
        /// </summary>
        /// <param name="jsonObj">App</param>
        public static void ConvertAppToJSON(App app) => ReadJSON.ConvertToJson(app);

        /// <summary>
        /// Determines if a key combo exists in the existing json file.
        /// </summary>
        /// <param name="jsonObj">App object</param>
        /// <returns>string</returns>
        public static string KeyExists(App app) => ReadJSON.KeyExists(app);

        /// <summary>
        /// Replaces the existing key combo with the one you provide
        /// </summary>
        /// <param name="jsonObj">App</param>
        public static void ReplaceKey(App app) => ReadJSON.Replace(app);

        /// <summary>
        /// Allows you to change the path of the config.json
        /// </summary>
        /// <param name="jsonPath">string</param>
        public static void ChangeJSON(string jsonPath)
        {
            if (File.Exists(jsonPath))
            {
                SettingsRegKey.SetRegKey(jsonPath);
            }
            else
            {
                throw new JSONFileDoesNotExistException("The provided path is not a valid path, reverting to the config.json file in your application executable folder");
            }
        }

        /// <summary>
        /// Takes in two strings and tries to register those into a hotkey.
        /// </summary>
        /// <param name="modkey">A string with Modifier Key names</param>
        /// <param name="key">A string with Keys name</param>
        public static void RegisterKey(string modkey, string key) => KeyboardHook.RegisterHotKey(modkey, key);

        /// <summary>
        /// Registers the HotKeys. Subscribes the event to the HotKey pressed event.
        /// </summary>
        public static void CreateKeys()
        {
            KeyboardHook.KeyPressed += (s, e) => KeyPressed?.Invoke(s, e);
            var keyPressed = ReadJSON.GetKeys();
            try
            {
                foreach (var appname in keyPressed)
                {
                    KeyboardHook.RegisterHotKey(appname.SysKey, appname.Key);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Resets HotKey registration
        /// </summary>
        public static void Reset()
        {
            KeyboardHook.ClearHotKeys();
            CreateKeys();
        }

        /// <summary>
        /// Gets the current number of registered keys
        /// </summary>
        /// <returns>int</returns>
        public static int RegisteredKeys() => KeyboardHook.currentID;

        /// <summary>
        /// Checks which key combo was pressed and passes the info to Processes. If a key combo that is registered is pressed, it returns true.
        /// <para></para>
        /// Once you get the results from here you can then handle the Powershell bool to take in the PowershellExecute output, or start the process with Processes.Start.
        /// </summary>
        /// <param name="e">KeyboardHookCallEventArgs</param>
        /// <returns>bool</returns>
        public static bool HotKeyPress(KeyboardHookCallEventArgs e)
        {
            var keyPressed = ReadJSON.GetKeys();
            foreach (var key in keyPressed)
            {
                var modify = KeyboardHook.GetModKeys(key.SysKey);
                modify -= ModifierKeys.NoRepeat;
                var pressedKey = (Keys)Enum.Parse(typeof(Keys), key.Key, ignoreCase: true);
                if (e.Modifier == modify && e.Key == pressedKey)
                {
                    var powershell = KeyboardHook.GetComBool(key.Powershell ?? "false");
                    PowerShell = powershell;
                    if (powershell)
                    {
                        var execute = new PowershellExecute(key.Exec);
                        PowerShellOut = execute.OutScript();
                    }
                    else
                    {
                        var command = KeyboardHook.GetComBool(key.Command ?? "false");
                        Processes.Process(key.Name, key.Exec, command);
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates the variable for the process such as the process name, path, if it is a command rather than an application.
        /// <para></para>
        /// This will then hand that info to the StartProcess method, which will run the method.
        /// </summary>
        /// <param name="processname">The name of the process</param>
        /// <param name="processpath">The path to the process</param>
        /// <param name="commandline">Whether this is an action to be ran on the command line or not</param>
        public static void ProcessCreate(string processname, string processpath, bool commandline = false) => Processes.Process(processname, processpath, commandline);

        /// <summary>
        /// Starts the process specified with the processname and path/command
        /// </summary>
        public static void StartProcess() => Processes.Start();

    }
}
