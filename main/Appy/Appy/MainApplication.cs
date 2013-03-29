using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Storage;
using System.Linq;
using System.ServiceModel;

namespace AppDirect.WindowsClient
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
    public class MainApplication : IMainApplication
    {
        private readonly IBrowserWindowsCommunicator _browserWindowsCommunicator;
        private readonly LocalStorage _localStorage;
        private readonly ILatch _latch;

        public MainApplication(IBrowserWindowsCommunicator browserWindowsCommunicator, LocalStorage localStorage, ILatch latch)
        {
            _browserWindowsCommunicator = browserWindowsCommunicator;
            _localStorage = localStorage;
            _latch = latch;
        }

        public IInitData Initialized()
        {
            var apps = _localStorage.InstalledAppDirectApps.Cast<IApplication>().ToList();
            var initData = new InitData()
                {
                    Applications = apps,
                    Session = ServiceLocator.CachedAppDirectApi.Session
                };

            _browserWindowsCommunicator.Start();

            _latch.Unlock();

            return initData;
        }
    }
}