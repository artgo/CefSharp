using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;
using System.Collections.Generic;

namespace AppDirect.WindowsClient.Browser.API
{
    public class MainApplicationServiceClient : AbstractServiceClient<IMainApplication>, IMainApplicationServiceClient
    {
        public MainApplicationServiceClient(IServiceBuilder<IMainApplication> serviceStarter, IUiHelper uiHelper, ILogger log)
            : base(serviceStarter, uiHelper, log)
        {
        }

        public void Initialized()
        {
            MakeSureExecuteAction(() => Service.Initialized());
        }

        public IAppDirectSession GetSession()
        {
            return MakeSureExecuteAction<IAppDirectSession>(() => Service.GetSession());
        }

        public IEnumerable<IApplication> GetMyApps()
        {
            return MakeSureExecuteAction<IEnumerable<IApplication>>(() => Service.GetMyApps());
        }

        public int Ping(int value)
        {
            return MakeSureExecuteAction<int>(() => Service.Ping(value));
        }
    }
}