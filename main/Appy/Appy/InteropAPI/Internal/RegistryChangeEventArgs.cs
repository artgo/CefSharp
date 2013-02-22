using System;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class RegistryChangeEventArgs : EventArgs
    {
        private readonly RegistryChangeMonitor _monitor;
        private readonly Exception _exception;

        public RegistryChangeEventArgs(RegistryChangeMonitor monitor, Exception exception)
        {
            _monitor = monitor;
            _exception = exception;
            Stop = false;
        }

        public RegistryChangeMonitor Monitor
        {
            get
            {
                return _monitor;
            }
        }

        public Exception Exception
        {
            get
            {
                return _exception;
            }
        }

        public bool Stop { get; set; }
    }
}