using AppDirect.WindowsClient.Browser.UI;
using AppDirect.WindowsClient.Common.API;
using System.Collections.Generic;

namespace AppDirect.WindowsClient.Browser.API
{
    public interface IBrowserWindowsManager
    {
        IEnumerable<IApplication> Applications { get; set; }

        IAppDirectSession Session { get; set; }

        BrowserWindow GetOrCreateBrowserWindow(IApplication application);

        BrowserWindow GetBrowserWindow(string applicationId);
    }
}