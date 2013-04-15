using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.Browser.Tests.API
{
    public class TestBrowserWindow : IBrowserWindow
    {
        public void SetSession(IAppDirectSession session)
        {
        }

        public void Close()
        {
        }

        public WindowState WindowState { get; set; }
        public bool Visible
        {
            get { return true; }
        }

        public void Hide()
        {
        }

        public void PreInitializeWindow()
        {
        }

        public void Show()
        {
        }

        public void Display()
        {
        }
    }
}
