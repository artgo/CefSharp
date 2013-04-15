using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppDirect.WindowsClient.Browser.API;

namespace AppDirect.WindowsClient.Browser.UI
{
    public interface IBrowserWindowsBuilder<T>
    {
        T CreateBrowserWindow(BrowserViewModel model);
    }

    class BrowserWindowsBuilder : IBrowserWindowsBuilder<IBrowserWindow>
    {
        public IBrowserWindow CreateBrowserWindow(BrowserViewModel model)
        {
            return new BrowserWindow(model);
        }
    }
}
