using AppDirect.WindowsClient.Common;
using AppDirect.WindowsClient.Common.API;
using System.Windows;

namespace AppDirect.WindowsClient.Browser.API
{
    public class MainApplicationCallback : IMainApplicationCallback
    {
        public Window BrowserWindow { get; set; }

        public void CloseWindow()
        {
            CommonHelper.PerformInUiThread(() => System.Windows.Application.Current.Shutdown());
        }

        public void ActivateWindow()
        {
            CommonHelper.PerformInUiThread(() =>
                {
                    if (!BrowserWindow.IsVisible)
                    {
                        BrowserWindow.Show();
                    }

                    if (BrowserWindow.WindowState == WindowState.Minimized)
                    {
                        BrowserWindow.WindowState = WindowState.Normal;
                    }

                    BrowserWindow.Activate();
                    BrowserWindow.Topmost = true;
                    BrowserWindow.Topmost = false;
                    BrowserWindow.Focus();
                });
        }
    }
}