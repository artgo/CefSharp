using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.Browser.API
{
    public class IpcMainWindowStarter : AbstractServiceRunner<IBrowsersManagerApi>
    {
        public IpcMainWindowStarter(IBrowsersManagerApi service)
            : base(service)
        {
        }
    }
}