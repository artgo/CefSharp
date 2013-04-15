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
        public virtual void SetSession(IAppDirectSession session)
        {
        }

        public virtual void Close()
        {
        }

        public virtual WindowState WindowState { get; set; }
        public virtual bool Visible
        {
            get { return true; }
        }

        public virtual void Hide()
        {
        }

        public virtual void PreInitializeWindow()
        {
        }

        public virtual void Show()
        {
        }

        public virtual void Display()
        {
        }
    }
}
