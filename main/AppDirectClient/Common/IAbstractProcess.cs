using AppDirect.WindowsClient.Common.API;
using System;

namespace AppDirect.WindowsClient.Common
{
    public interface IAbstractProcess : IProcessRestarter
    {
        void Start();

        void RegisterExitedEvent(EventHandler exitedAction);

        void RemoveRegisteredEvent(EventHandler registeredEvent);

        void GetProcess();
    }
}