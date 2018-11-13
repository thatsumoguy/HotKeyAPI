using System;
using System.Diagnostics;
using System.Threading;

namespace HotKeyApi
{
    public static class Processes
    {
        private static string _processname;
        private static string _processpath;
        private static bool _commandline;

        /// <summary>
        /// Creates a process with the provided name and path
        /// </summary>
        /// <param name="processname">Name of the process you want to launch</param>
        /// <param name="processpath">Path/Commands you want to launch</param>
        internal static void Process(string processname, string processpath, bool commandline = false)
        {
            _processname = processname;
            _processpath = processpath;
            _commandline = commandline;
        }
        /// <summary>
        /// Starts the process specified with the processname and path/command
        /// </summary>
        internal static void Start()
        {
            var processName = new Process();
            if (_commandline)
            {
                processName.StartInfo.FileName = "cmd.exe";
                processName.StartInfo.Arguments = "/C " + _processpath;
            }
            else
            {
                processName.StartInfo.FileName = _processpath;
            }
            try
            {
                new Thread(() =>
                {
                    try
                    {
                        Thread.CurrentThread.IsBackground = true;
                        processName.Start();
                    }
                    catch (System.ComponentModel.Win32Exception ex)
                    {
                        throw new System.ComponentModel.Win32Exception("Process stopped by user before launching. " + ex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }).Start();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("Your path is not correct, please fix in config.json" + Environment.NewLine +
                    "Process trying to start: " + _processname);
            }
            catch (Exception)
            {
                throw new Exception("The process failed to start");
            }
        }
    }
}
