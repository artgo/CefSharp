using System;
using System.Diagnostics;
using System.Linq;
using AppDirect.WindowsClient.Common.Log;

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
                _process = new Process();

                _process.StartInfo.FileName = _processName;
                _process.StartInfo.UseShellExecute = false;
                _process.StartInfo.CreateNoWindow = true;

                _log.Info("Process " + _processName + " is not found and need to start");

                _needToStart = true;
            }
        }

        public Process Process
        {
            get { return _process; }
        }
        
        public void Start()
        {
            if (_needToStart)
            {
                Process.Start();
                _log.Info("Process " + _processName + " was started");
            }

            _needToStart = false;
        }

        public void RegisterExitedEvent(EventHandler exitedEvent)
        {
            Process.EnableRaisingEvents = true;
            Process.Exited += exitedEvent;
        }

        public void RemoveRegisteredEvent(EventHandler registeredEvent)
        {
            Process.Exited -= registeredEvent;
        }
    }
}