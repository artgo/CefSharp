using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Browser.UI;

namespace AppDirect.WindowsClient.Browser.Tests.API
{
    public class TestBrowserWindowsBuilder : IBrowserWindowsBuilder<IBrowserWindow>
    {
        private readonly IBrowserWindow _browserWindow;

        public TestBrowserWindowsBuilder(IBrowserWindow browserWindow)
        {
            _browserWindow = browserWindow;
        }

        public IBrowserWindow CreateBrowserWindow(BrowserViewModel model)
        {
            return _browserWindow;
        }
    }
}