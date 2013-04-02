using AppDirect.WindowsClient.BrowsersApi;
using AppDirect.WindowsClient.Common.API;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace AppDirect.WindowsClient.API
{
    public class BrowserWindowsCommunicator : IBrowserWindowsCommunicator
    {
        private volatile IBrowsersManagerApi _browserApi;
        private volatile ICommunicationObject _communicationObject;
        private readonly ILatch _latch;

        public BrowserWindowsCommunicator(ILatch latch)
        {
            _latch = latch;
        }

        private void WaitAndExecuteWithRetry(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            _latch.Wait();

            try
            {
                action.Invoke();
            }
            catch (CommunicationException)
            {
                // Restart the communicator and retry
                Start();
                action.Invoke();
            }
        }

        public void DisplayApplication(IApplication application)
        {
            WaitAndExecuteWithRetry(() => _browserApi.DisplayApplication(application));
        }

        public void CloseApplication(string appId)
        {
            WaitAndExecuteWithRetry(() => _browserApi.CloseApplication(appId));
        }

        public void UpdateSession(IAppDirectSession newSession)
        {
            WaitAndExecuteWithRetry(() => _browserApi.UpdateSession(newSession));
        }

        public void UpdateApplications(IEnumerable<IApplication> applications)
        {
            WaitAndExecuteWithRetry(() => _browserApi.UpdateApplications(applications));
        }

        public void CloaseAllApplicationsAndQuit()
        {
            WaitAndExecuteWithRetry(() => _browserApi.CloaseAllApplicationsAndQuit());
        }

        public virtual void Start()
        {
            // We can't instantiate this object in advance, since it tries to connect in constructor.
            _browserApi = CreateBrowsersManagerApiClient();
            _communicationObject = GetCommunicationObject();
        }

        protected internal virtual IBrowsersManagerApi CreateBrowsersManagerApiClient()
        {
            return new BrowsersManagerApiClient();
        }

        protected internal virtual ICommunicationObject GetCommunicationObject()
        {
            return (ICommunicationObject) _browserApi;
        }

        public void Stop()
        {
            if ((_browserApi == null) || (_communicationObject == null))
            {
                throw new InvalidOperationException("Service was not started");
            }

            _browserApi = null;
            _communicationObject = null;
        }
    }
}