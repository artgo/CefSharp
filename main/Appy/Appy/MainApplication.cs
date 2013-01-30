using System.ServiceModel;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MainApplication" in both code and config file together.
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class MainApplication : IMainApplication
    {
        public IApplication GetApplicationById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var apps = LocalApplications.GetLocalApplications();
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

            return null;
        }

        public IAppDirectSession GetCurrentSession()
        {
            return ServiceLocator.CachedAppDirectApi.Session;
        }
    }
}
