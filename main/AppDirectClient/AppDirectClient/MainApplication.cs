using System;
using System.Collections.Generic;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Storage;
using System.Linq;
using System.ServiceModel;

namespace AppDirect.WindowsClient
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class MainApplication : IMainApplication
    {
        private readonly LocalStorage _localStorage;
        private readonly IBrowserWindowsCommunicator _browerServiceClient;

        public MainApplication(LocalStorage localStorage, IBrowserWindowsCommunicator browerServiceClient)
        {
            if (localStorage == null)
            {
                throw new ArgumentNullException("localStorage");
            }

            if (browerServiceClient == null)
            {
                throw new ArgumentNullException("browerServiceClient");
            }

            _localStorage = localStorage;
            _browerServiceClient = browerServiceClient;
        }

        public IAppDirectSession GetSession()
        {
            var session = ServiceLocator.CachedAppDirectApi.Session;
            return session;
        }

        public IEnumerable<IApplication> GetMyApps()
        {
            var apps = _localStorage.InstalledAppDirectApps.Cast<IApplication>().ToList();
            return apps;
        }

        public void Initialized()
        {
            _browerServiceClient.StartIfNotStarted();
            var session = ServiceLocator.CachedAppDirectApi.Session;
            if ((session != null) && (session.Cookies.Count > 0))
            {
                _browerServiceClient.UpdateSession(session);
            }
            var apps = _localStorage.InstalledAppDirectApps.Cast<IApplication>().ToList();
            if (apps.Count > 0)
            {
                _browerServiceClient.UpdateApplications(apps);
            }
        }

        public int Ping(int value)
        {
            return value + 1;
        }
    }
}