using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Browser.UI;

namespace AppDirect.WindowsClient.Browser.Tests.API
{
    class TestBrowserWindowsBuilder : IBrowserWindowsBuilder<IBrowserWindow>
    {
        public IBrowserWindow CreateBrowserWindow(BrowserViewModel model)
        {
            return new TestBrowserWindow();
        }
    }
}
