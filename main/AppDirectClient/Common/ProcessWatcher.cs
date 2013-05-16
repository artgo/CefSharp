using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AppDirect.WindowsClient.Common.Log;
using NLog;

namespace AppDirect.WindowsClient.Common
{
    public class ProcessWatcher
    {
        private readonly string _processName;
        private int _restartLimit = 10;
        private ILogger _logger;

        public Process Process;

        public ProcessWatcher(string processName, ILogger logger)
        {
            _processName = processName;
            _logger = logger;
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
                _logger.Info(String.Format("{0} process started by ProcessWatcher", _processName));
            }
        }

        private void LaunchIfCrashed(object o, EventArgs e)
        {
            Process process = (Process)o;
            _logger.Info(String.Format("The {0} process was exited with ExitCode {1}.", _processName, process.ExitCode));
            if (process.ExitCode != 0)
            {
                if (_restartLimit-- > 0)//limit the number of restarts to prevent infinite crashing
                {
                    Watch();
                }
            }
        }
    }
}