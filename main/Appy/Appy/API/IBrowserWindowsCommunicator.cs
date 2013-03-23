using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.API
{
    public interface IBrowserWindowsCommunicator
    {
        void OpenOrActivateApp(IApplication application);
        void CloseApp(IApplication application);
    }
}