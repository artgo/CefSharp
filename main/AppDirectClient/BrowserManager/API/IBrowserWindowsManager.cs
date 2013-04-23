using AppDirect.WindowsClient.Common.API;
using System.Collections.Generic;

namespace AppDirect.WindowsClient.Browser.API
{
    public interface IBrowserWindowsManager
    {
        IEnumerable<IApplication> Applications { get; set; }

        IAppDirectSession Session { get; set; }

        IBrowserWindow GetOrCreateBrowserWindow(IApplication application);

        IBrowserWindow GetOrCreateRegistrationWindow(IApplication application);

        IBrowserWindow GetBrowserWindow(string applicationId);

        void CloseAllWindows();

        IEnumerable<IWindowData> GetBrowserWindowDatas();
    }
}