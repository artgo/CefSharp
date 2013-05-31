using System.Threading;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using System;
using System.Diagnostics;

namespace AppDirect.WindowsClient.Common
{
    public class ProcessWatcher : IProcessWatcher
    {
        private readonly string _processName;
        private volatile int _restartsRemaining = 10;
        private volatile ILogger _logger;

        private volatile IAbstractProcess _process;

        public ProcessWatcher(string processName, IAbstractProcess abstractProcess, ILogger logger)
        {
            _processName = processName;
            _process = abstractProcess;
            _logger = logger;
        }

        public void Start()
        {
            Watch();
        }

        public void Stop()
        {
            _process.RemoveRegisteredEvent(LaunchIfCrashed);
            _restartsRemaining = 10;
        }

        private void Watch()
        {
            _process.GetProcess();
            _process.RegisterExitedEvent(LaunchIfCrashed);
        }

        private void LaunchIfCrashed(object o, EventArgs e)
        {
            Process process = (Process)o;
            _logger.Info(String.Format("The {0} process was exited with ExitCode {1}.", _processName, process.ExitCode));
            if (process.ExitCode != 0)
            {
                if (_restartsRemaining-- > 0)//limit the number of restarts to prevent infinite crashing
                {
                    _process.GetProcess();
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