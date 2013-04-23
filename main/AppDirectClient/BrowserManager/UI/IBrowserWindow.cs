using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.Browser.API
{
    public interface IBrowserWindow
    {
        void SetSession(IAppDirectSession session);
        void Close();
        WindowState WindowState { get; set; }
        bool Visible { get; }
        void Hide();
        void PreInitializeWindow();
        void Show();
        void Display();
        void Navigate();
    }
}
