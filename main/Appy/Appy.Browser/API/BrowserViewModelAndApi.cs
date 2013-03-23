using AppDirect.WindowsClient.Browser.MainApp;
using AppDirect.WindowsClient.Browser.UI;

namespace AppDirect.WindowsClient.Browser.API
{
    public class BrowserViewModelAndApi
    {
        public BrowserViewModel BrowserViewModel { get; set; }

        public MainApplicationCallback MainApplicationCallback { get; set; }

        public MainApplicationClient MainApplicationClient { get; set; }
    }
}