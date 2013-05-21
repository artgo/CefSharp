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
}