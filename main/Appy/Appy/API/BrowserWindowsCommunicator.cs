using AppDirect.WindowsClient.BrowsersApi;
using AppDirect.WindowsClient.Common.API;
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

        public void DisplayApplication(IApplication application)
        {
            _latch.Wait();
            _browserApi.DisplayApplication(application);
        }

        public void CloseApplication(string appId)
        {
            _latch.Wait();
            _browserApi.CloseApplication(appId);
        }

        public void UpdateSession(IAppDirectSession newSession)
        {
            _latch.Wait();
            _browserApi.UpdateSession(newSession);
        }

        public void UpdateApplications(IEnumerable<IApplication> applications)
        {
            _latch.Wait();
            _browserApi.UpdateApplications(applications);
        }

        public void CloaseAllApplicationsAndQuit()
        {
            _latch.Wait();
            _browserApi.CloaseAllApplicationsAndQuit();
        }

        public virtual void Start()
        {
            // We can't instantiate this object in advance, since it tries to connect in constructor.
            var client = new BrowsersManagerApiClient();
            _browserApi = client;
            _communicationObject = client;
        }

        public void Stop()
        {
        }
    }
}