using System;
using System.Diagnostics;

namespace AppDirect.WindowsClient.Common
{
    public interface IAbstractProcess
    {
        Process Process { get; }
        void Start();
        void RegisterExitedEvent(EventHandler exitedAction);
        void RemoveRegisteredEvent(EventHandler registeredEvent);
        void GetProcess();
    }

    public class TestAbstractProcess : IAbstractProcess
    {
        public Process Process { get; private set; }

        public void Start()
        {
        }

        public void RegisterExitedEvent(EventHandler exitedAction)
        {
        }

        public void RemoveRegisteredEvent(EventHandler registeredEvent)
        {
        }

        public void GetProcess()
        {
        }
    }
}