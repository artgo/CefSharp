using AppDirect.WindowsClient.Common.Log;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AppDirect.WindowsClient.Common
{
    public class AbstractProcess : IAbstractProcess
    {
        private readonly string _processName;
        private readonly ILogger _log;
        private volatile Process _process;
        private volatile int _currentUserSessionId;
        private volatile bool _needToStart = false;

        private int CurrentUserSessionId
        {
            get
            {
                if (_currentUserSessionId == 0)
                {
                    var currentProcess = Process.GetCurrentProcess();
                    _currentUserSessionId = currentProcess.SessionId;
                }
                return _currentUserSessionId;
            }
        }

        public AbstractProcess(string processName, ILogger log)
        {
            _processName = processName;
            _log = log;
        }

        public void GetProcess()
        {
            var processesByName = Process.GetProcessesByName(_processName);

            if (processesByName.Any())
            {
                _process = processesByName.LastOrDefault(p => p.SessionId == CurrentUserSessionId);
                _needToStart = false;
            }
            else
            {
                ReCreateProcess();
            }
        }

        private void ReCreateProcess()
        {
            _process = new Process
                {
                    StartInfo = { FileName = _processName, UseShellExecute = false, CreateNoWindow = true }
                };

            _log.Info("Process " + _processName + " is not found and need to start");

            _needToStart = true;
        }

        public void Start()
        {
            if (_needToStart)
            {
                _process.Start();
                _log.Info("Process " + _processName + " was started");
            }

            _needToStart = false;
        }

        public void RegisterExitedEvent(EventHandler exitedEvent)
        {
            _process.EnableRaisingEvents = true;
            _process.Exited += exitedEvent;
        }

        public void RemoveRegisteredEvent(EventHandler registeredEvent)
        {
            _process.Exited -= registeredEvent;
        }

        public void RestartProcess()
        {
            if (_process != null)
            {
                _process.Kill();
                _process = null;
            }

            Thread.Sleep(1000);

            ReCreateProcess();
            Start();
        }
    }
}