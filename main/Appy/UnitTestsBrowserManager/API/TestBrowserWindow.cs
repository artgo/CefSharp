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
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public WindowState WindowState { get; set; }
        public bool Visible
        {
            get { throw new NotImplementedException(); }
        }

        public void Hide()
        {
            throw new NotImplementedException();
        }

        public void PreInitializeWindow()
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            throw new NotImplementedException();
        }

        public void Display()
        {
            throw new NotImplementedException();
        }
    }
}
