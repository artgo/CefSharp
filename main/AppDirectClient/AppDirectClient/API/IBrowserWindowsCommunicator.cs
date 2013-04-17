using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.API
{
    public interface IBrowserWindowsCommunicator : IAbstractServiceClient<IBrowsersManagerApi>, IBrowsersManagerApi
    {
    }
}