using AppDirect.WindowsClient.Common;
using AppDirect.WindowsClient.Common.API;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;

namespace AppDirect.WindowsClient.Browser.API
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class BrowsersManagerApi : IBrowsersManagerApi
    {
        public BrowserWindowsManager BrowserWindowsManager { get; set; }

        public void DisplayApplication(IApplication application)
        {
            var browserWindow = BrowserWindowsManager.GetOrCreateWindow(application);

            CommonHelper.PerformInUiThread(() =>
            {
                if (!browserWindow.IsVisible)
                {
                    browserWindow.Show();
                }

                if (browserWindow.WindowState == WindowState.Minimized)
                {
                    browserWindow.WindowState = WindowState.Normal;
                }

                browserWindow.Activate();
                browserWindow.Focus();
            });
        }

        public void CloseApplication(string appId)
        {
            var browserWindow = BrowserWindowsManager.GetWindow(appId);
            if (browserWindow != null)
            {
                CommonHelper.PerformInUiThread(browserWindow.Hide);
            }
        }

        public void UpdateSession(IAppDirectSession newSession)
        {
            BrowserWindowsManager.Session = newSession;
        }

        public void UpdateApplications(IEnumerable<IApplication> applications)
        {
            BrowserWindowsManager.Applications = applications;
        }

        public void CloaseAllApplicationsAndQuit()
        {
            CommonHelper.GracefulShutdown();
        }
    }
}