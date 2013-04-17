using AppDirect.WindowsClient.Browser.API;

namespace AppDirect.WindowsClient.Browser.UI
{
    internal class BrowserWindowsBuilder : IBrowserWindowsBuilder<IBrowserWindow>
    {
        public IBrowserWindow CreateBrowserWindow(BrowserViewModel model)
        {
            return new BrowserWindow(model);
        }
    }
}