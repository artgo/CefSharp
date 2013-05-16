using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AppDirect.WindowsClient.Common
{
    public class ProcessWatcher
    {
        private static string _processName;
        private static int count = 10;
        private bool _gracefulShutdown;

        public Process Process;

        public ProcessWatcher(string processName)
        {
            _processName = processName;
        }

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

            if (processesByName.Any())
            {
                Process = processesByName[0];
                Process.EnableRaisingEvents = true;
                Process.Exited += LaunchIfCrashed;

                if (!Process.Responding)
                {
                    Process.Kill();
                }
            }
            else
            {
                Process = new Process();

                Process.StartInfo.FileName = _processName;
                Process.StartInfo.UseShellExecute = false;
                Process.StartInfo.CreateNoWindow = true;
                Process.EnableRaisingEvents = true;
                Process.Exited += LaunchIfCrashed;

                Process.Start();
            }
        }

        private void LaunchIfCrashed(object o, EventArgs e)
        {
            Process process = (Process)o;
            if (process.ExitCode != 0 && !_gracefulShutdown)
            {
                if (count-- > 0) // restart at max count times
                {
                    Launch();
                }
                else
                {
                    Environment.Exit(process.ExitCode);
                }
            }
        }
    }
}