using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using System;
using System.Diagnostics;
using System.Linq;

namespace AppDirect.WindowsClient.Common
{
    public class ProcessWatcher : IStartStop
    {
        private readonly string _processName;
        private volatile int _restartsRemaining = 10;
        private volatile ILogger _logger;

        private volatile Process _process;

        public ProcessWatcher(string processName)
        {
            _processName = processName;
            _logger = new NLogLogger(_processName + "Watcher");
        }

        public void Start()
        {
            Watch();
        }

        public void Stop()
        {
            _process.Exited -= LaunchIfCrashed;
            _restartsRemaining = 10;
        }

        private void Watch()
        {
            var processesByName = Process.GetProcessesByName(_processName);

            if (processesByName.Any())
            {
                _process = processesByName[0];
            }
            else
            {
                _process = new Process();

                _process.StartInfo.FileName = _processName;
                _process.StartInfo.UseShellExecute = false;
                _process.StartInfo.CreateNoWindow = true;
            }

            _process.EnableRaisingEvents = true;
            _process.Exited += LaunchIfCrashed;
        }

        private void LaunchIfCrashed(object o, EventArgs e)
        {
            Process process = (Process)o;
            _logger.Info(String.Format("The {0} process was exited with ExitCode {1}.", _processName, process.ExitCode));
            if (process.ExitCode != 0)
            {
                if (_restartsRemaining-- > 0)//limit the number of restarts to prevent infinite crashing
                {
                    _process = null;
                    Watch();
                    if (_process != null)
                    {
                        _process.Start();
                        _logger.Info(String.Format("{0} process started by ProcessWatcher", _processName));
                    }
                }
                else
                {
                    Stop();
                }
            }
        }
    }
}