using System;
using System.Diagnostics;
using System.Linq;

namespace AppDirect.WindowsClient.Common
{
    public class AbstractProcess : IAbstractProcess
    {
        private readonly string _processName;
        private volatile Process _process;

        public AbstractProcess(string processName)
        {
            _processName = processName;
        }

        public void GetProcess()
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
        }

        public Process Process
        {
            get { return _process; }
        }
        
        public void Start()
        {
            Process.Start();
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