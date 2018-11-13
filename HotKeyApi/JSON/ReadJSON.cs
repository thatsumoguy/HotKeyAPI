using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using System.Reflection;

namespace HotKeyApi
{
    internal static class ReadJSON
    {
        public static string JSONPath { get; private set; }
        /// <summary>
        /// Reads the JSON file supplied in <code>Config.json</code> and supplied back a <code>List<App>(App)</App></code> where you can extract the info.
        /// </summary>
        /// <returns>List<App>(App)</App>></returns>
        internal static List<App> GetKeys()
        {
            var apps = DeserializeJSON();
            return (from App appnames in apps.App select appnames).ToList();
        }
        /// <summary>
        /// Converts the created App and adds it to the json structure and overwrites config.json
        /// </summary>
        /// <param name="jsonObj">App</param>
        internal static void ConvertToJson(App jsonObj)
        {
            var newApp = DeserializeJSON();
            if (!newApp.App.Contains(jsonObj))
            {
                newApp.App.Add(jsonObj);
            }
            else
            {
                var existing = newApp.App.Where(x => x.Key == jsonObj.Key && x.SysKey == jsonObj.SysKey).Select(x => x.Name);
                var deliminter = ",";
                var existingstring = string.Join(deliminter, existing);
                throw new KeyAlreadyExistsException(existingstring);
            }
            var js = new JavaScriptSerializer();
            var jsNewApp = FormatOutput(js.Serialize(newApp));
            var jsonfile = JSONFile();
            File.WriteAllText(jsonfile, jsNewApp);
        }
        /// <summary>
        /// Replaces the existing key combo with the one you provide
        /// </summary>
        /// <param name="jsonObj">App</param>
        internal static void Replace(App jsonObj)
        {
            var js = new JavaScriptSerializer();
            var newApp = DeserializeJSON();
            var replace = newApp.App.First(x => x.Key == jsonObj.Key && x.SysKey == jsonObj.SysKey);
            var index = newApp.App.IndexOf(replace);
            if (index != -1)
            {
                newApp.App[index] = jsonObj;
            }
            var jsNewApp = FormatOutput(js.Serialize(newApp));
            var jsonfile = JSONFile();
            File.WriteAllText(jsonfile, jsNewApp);
        }
        /// <summary>
        /// Determines if a key exists
        /// </summary>
        /// <param name="jsonObj">App object</param>
        /// <returns>string</returns>
        internal static string KeyExists(App jsonObj)
        {
            var jsonfile = JSONFile();
            var newApp = DeserializeJSON();
            var existing = newApp.App.Where(x => x.Key == jsonObj.Key && x.SysKey == jsonObj.SysKey).Select(x => x.Name);
            if(existing.Any())
            {
                var deliminter = ",";
                var existingstring = string.Join(deliminter, existing);
                return existingstring;
            }
            return null;
        }
        
        private static AppRoot DeserializeJSON()
        {
            var jsonfile = JSONFile();
            var json = File.ReadAllText(jsonfile);
            var js = new JavaScriptSerializer();
            return js.Deserialize<AppRoot>(json);
        }
        /// <summary>
        /// Sets the json file path.
        /// </summary>
        /// <returns>string</returns>
        private static string JSONFile()
        {
            
            if (File.Exists(SettingsRegKey.GetRegKey()))
            {
                JSONPath = SettingsRegKey.GetRegKey();
            }
            else
            {
                var execpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var jsonFile = Path.Combine(execpath, @"Config.json");
                if(File.Exists(jsonFile))
                {
                    JSONPath = jsonFile;
                    SettingsRegKey.SetRegKey(jsonFile);
                }
                else
                {
                    throw new JSONFileDoesNotExistException("There is currently no Config.json file. Please create one and place it in the executable file location. If you want to change the path please run ChangeJSON");
                }
            }
            return JSONPath;
        }
        ///// <summary>
        ///// Keeps your configuration
        ///// </summary>
        //internal static void KeepSettings()
        //{
        //    var assembly = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //    var configFolder = Path.Combine(Path.GetDirectoryName(assembly), @"Config");
        //    if(!Directory.Exists(configFolder))
        //    {
        //        Directory.CreateDirectory(configFolder);
        //    }
        //    var configJson = Path.Combine(configFolder, @"Config.json");
            
        //    File.WriteAllText(JSONFile(), configJson);
        //}

        /// <summary>
        /// Adds indentation and line breaks to output of JavaScriptSerializer
        /// </summary>
        private static string FormatOutput(string jsonString)
        {
            // Stole from https://stackoverflow.com/questions/5881204/how-to-set-formatting-with-javascriptserializer-when-json-serializing/23828858#23828858 


            var stringBuilder = new StringBuilder();

            bool escaping = false;
            bool inQuotes = false;
            int indentation = 0;

            foreach (char character in jsonString)
            {
                if (escaping)
                {
                    escaping = false;
                    stringBuilder.Append(character);
                }
                else
                {
                    if (character == '\\')
                    {
                        escaping = true;
                        stringBuilder.Append(character);
                    }
                    else if (character == '\"')
                    {
                        inQuotes = !inQuotes;
                        stringBuilder.Append(character);
                    }
                    else if (!inQuotes)
                    {
                        if (character == ',')
                        {
                            stringBuilder.Append(character);
                            stringBuilder.Append("\r\n");
                            stringBuilder.Append('\t', indentation);
                        }
                        else if (character == '[' || character == '{')
                        {
                            stringBuilder.Append(character);
                            stringBuilder.Append("\r\n");
                            stringBuilder.Append('\t', ++indentation);
                        }
                        else if (character == ']' || character == '}')
                        {
                            stringBuilder.Append("\r\n");
                            stringBuilder.Append('\t', --indentation);
                            stringBuilder.Append(character);
                        }
                        else if (character == ':')
                        {
                            stringBuilder.Append(character);
                            stringBuilder.Append('\t');
                        }
                        else
                        {
                            stringBuilder.Append(character);
                        }
                    }
                    else
                    {
                        stringBuilder.Append(character);
                    }
                }
            }

            return stringBuilder.ToString();
        }
    }
    /// <summary>
    /// App property structure
    /// </summary>
    public class App : IEquatable<App>
    {
        public string ProcessName { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        public string SysKey { get; set; }

        public string Command { get; set; }

        public string Powershell { get; set; }

        public string Exec { get; set; }
        
        public override string ToString()
        {
            return "Name: " + Name+ "Key: " + Key+ "SysKey: " +  SysKey +  "Exec path: " +  Exec;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var appObj = obj as App;
            if (appObj == null) return false;
            else return Equals(appObj);
        }
        public override int GetHashCode() => base.GetHashCode();
        public bool Equals(App other)
        {
            if (other == null) return false;
            return this.Key.Equals(other.Key) && this.SysKey.Equals(other.SysKey);
        }
    }
    public class AppRoot
    {
        public List<App> App { get; set; }
    }
}
