using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AppDirect.WindowsClient.Common
{
    public class ProcessWatcher
    {
        private readonly string _processName;
        private int _restartLimit = 10;
        private volatile bool _gracefulShutdown;

        public Process Process;

        public ProcessWatcher(string processName)
        {
            _processName = processName;
        }

        public void Start()
        {
            Watch();
        }

        public void Stop()
        {
            Process.Exited -= LaunchIfCrashed;
        }

        private void Watch()
        {
            var processesByName = Process.GetProcessesByName(_processName);

            if (processesByName.Any())
            {
                Process = processesByName[0];
                Process.EnableRaisingEvents = true;
                Process.Exited += LaunchIfCrashed;
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
                if (_restartLimit-- > 0)//limit the number of restarts to prevent infinite crashing
                {
                    Watch();
                }
            }
        }
    }
}