using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace AppDirect.WindowsClient.Common
{
    public class ProcessWatcher
    {
        public static string _processName;

        public ProcessWatcher(string processName)
        {
            _processName = processName; 
        }

        static int count = 10;
        private bool _gracefulShutdown;

        public void Watch()
        {
            Launch();
            Thread.Sleep(Timeout.Infinite);
        }

        public void Stop()
        {
            _gracefulShutdown = true;
        }

        private void Launch()
        {
            var processesByName = Process.GetProcessesByName(_processName);

            Process process;

            if (processesByName.Any())
            {
                process = processesByName[0];
                process.EnableRaisingEvents = true;
                process.Exited += LaunchIfCrashed;

                if (!process.Responding)
                {
                    process.Kill();
                }
            }
            else
            {
                process = new Process();

                process.StartInfo.FileName = _processName;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.EnableRaisingEvents = true;
                process.Exited += LaunchIfCrashed;

                process.Start();
            }
        }

        private void LaunchIfCrashed(object o, EventArgs e)
        {
            Process process = (Process)o;
            if (process.ExitCode != 0 && !_gracefulShutdown)
            {
                if (count-- > 0) // restart at max count times
                    Launch();
                else
                    Environment.Exit(process.ExitCode);
            }
        }
    }
}
