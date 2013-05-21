using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.UI;
using System;
using System.Collections.Generic;
using System.ServiceModel;

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
                browserWindow.Display();
            });
        }

        public void DisplayApplicationWithoutSession(IApplication application)
        {
            var browserWindow = _browserWindowsManager.GetOrCreateSessionlessWindow(application);

            _uiHelper.PerformInUiThread(() =>
            {
                browserWindow.Navigate();
                browserWindow.Display();
            });
        }

        public void DisplayApplications(IEnumerable<IApplicationWithState> applications)
        {
            foreach (var applicationWithState in applications)
            {
                var browserWindow = _browserWindowsManager.GetOrCreateBrowserWindow(applicationWithState.Application);

                var state = applicationWithState.WindowState;

                _uiHelper.PerformInUiThread(() =>
                {
                    if (!browserWindow.Visible)
                    {
                        browserWindow.Show();
                    }

                    browserWindow.WindowState = state;
                });
            }
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

        public void CloseAllApplicationsAndRemoveSessionInfo()
        {
            _browserWindowsManager.CloseAllWindows();
            _browserWindowsManager.Session = new AppDirectSession();
            _browserWindowsManager.ClearAllCookies();
        }

        public void CloseBrowserProcess()
        {
            _browserWindowsManager.CloseAllWindows();
            _uiHelper.GracefulShutdown();
        }

        public IEnumerable<IWindowData> GetOpenWindowDatas()
        {
            return _browserWindowsManager.GetBrowserWindowDatas();
        }
    }
}