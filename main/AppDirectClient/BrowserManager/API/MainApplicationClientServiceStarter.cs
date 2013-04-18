using AppDirect.WindowsClient.Browser.MainApp;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.Browser.API
{
    public class MainApplicationClientServiceStarter : IServiceBuilder<IMainApplication>
    {
        public IMainApplication CreateServiceAndTryToConnect()
        {
            return new MainApplicationClient();
        }
    }
}