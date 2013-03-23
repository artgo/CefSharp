using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.Browser.UI
{
    public class BrowserViewModel
    {
        public IApplication Application { get; set; }
        public IAppDirectSession Session { get; set; }
    }
}
