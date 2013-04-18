using AppDirect.WindowsClient.BrowsersApi;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.API
{
    public class BrowsersManagerApiServiceBuilder : IServiceBuilder<IBrowsersManagerApi>
    {
        public IBrowsersManagerApi CreateServiceAndTryToConnect()
        {
            return new BrowsersManagerApiClient();
        }
    }
}