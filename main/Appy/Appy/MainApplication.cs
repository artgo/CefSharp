using System.ServiceModel;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MainApplication" in both code and config file together.
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.PerSession)]
    public class MainApplication : IMainApplication
    {
        private volatile string _id;
        private readonly IMainApplicationCallback _callback;

        public MainApplication()
        {
            _callback = OperationContext.Current.GetCallbackChannel<IMainApplicationCallback>();
        }

        public IApplication GetApplicationById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            _id = id;

            ServiceLocator.IpcCommunicator.RegisterClient(id, this);

            return ApplicationById(id);
        }

        private IApplication ApplicationById(string id)
        {
            lock (ServiceLocator.LocalStorage.Locker)
            {
                var apps = ServiceLocator.LocalStorage.InstalledLocalApps;

                foreach (var app in apps)
                {
                    if (id.Equals(app.Id))
                    {
                        return app;
                    }
                }

                apps = ServiceLocator.LocalStorage.InstalledAppDirectApps;
                foreach (var app in apps)
                {
                    if (id.Equals(app.Id))
                    {
                        return app;
                    }
                }

                apps = ServiceLocator.LocalStorage.LastSuggestedApps;
                foreach (var app in apps)
                {
                    if (id.Equals(app.Id))
                    {
                        return app;
                    }
                }
            }

            return null;
        }

        public IAppDirectSession GetCurrentSession()
        {
            return ServiceLocator.CachedAppDirectApi.Session;
        }

        public void BrowserWasClosed()
        {
            ServiceLocator.IpcCommunicator.RemoveClient(_id);
        }

        public IMainApplicationCallback Callback
        {
            get { return _callback; }
        }
    }
}
