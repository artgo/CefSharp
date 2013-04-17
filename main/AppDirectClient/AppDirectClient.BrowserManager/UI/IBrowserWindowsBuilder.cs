using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppDirect.WindowsClient.Browser.UI
{
    public interface IBrowserWindowsBuilder<T>
    {
        T CreateBrowserWindow(BrowserViewModel model);
    }
}
