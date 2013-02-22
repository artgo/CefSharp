using System;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class RegistryChangeEventArgs : EventArgs
    {
        private readonly RegistryChangeMonitor _monitor;

        public RegistryChangeEventArgs(RegistryChangeMonitor monitor)
        {
            _monitor = monitor;
            Stop = false;
        }

        public RegistryChangeMonitor Monitor
        {
            get { return _monitor; }
        }

        public Exception Exception { get; set; }

        public bool Stop { get; set; }
    }
}