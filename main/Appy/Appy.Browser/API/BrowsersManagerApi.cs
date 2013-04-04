using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.UI;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;

namespace AppDirect.WindowsClient.Browser.API
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class BrowsersManagerApi : IBrowsersManagerApi
    {
        private readonly IBrowserWindowsManager _browserWindowsManager;
        private readonly IUiHelper _uiHelper;

        public BrowsersManagerApi(IBrowserWindowsManager browserWindowsManager, IUiHelper uiHelper)
        {
            if (browserWindowsManager == null)
            {
                throw new ArgumentNullException("browserWindowsManager");
            }

            if (uiHelper == null)
            {
                throw new ArgumentNullException("uiHelper");
            }

            _browserWindowsManager = browserWindowsManager;
            _uiHelper = uiHelper;
        }

        public void DisplayApplication(IApplication application)
        {
            var browserWindow = _browserWindowsManager.GetOrCreateBrowserWindow(application);

            _uiHelper.PerformInUiThread(() =>
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
            var browserWindow = _browserWindowsManager.GetBrowserWindow(appId);
            if (browserWindow != null)
            {
                _uiHelper.PerformInUiThread(browserWindow.Hide);
            }
        }

        public void UpdateSession(IAppDirectSession newSession)
        {
            _browserWindowsManager.Session = newSession;
        }

        public void UpdateApplications(IEnumerable<IApplication> applications)
        {
            _browserWindowsManager.Applications = applications;
        }

        public void CloaseAllApplicationsAndQuit()
        {
            MessageBox.Show("Should exit");
            _uiHelper.GracefulShutdown();
        }
    }
}