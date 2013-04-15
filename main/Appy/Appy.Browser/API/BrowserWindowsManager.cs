using System.Linq;
using AppDirect.WindowsClient.Browser.Interaction;
using AppDirect.WindowsClient.Browser.UI;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AppDirect.WindowsClient.Browser.API
{
    public class BrowserWindowsManager : IBrowserWindowsManager
    {
        private readonly object _lockObject = new object();
        private readonly Dictionary<string, IBrowserWindow> _browserWindows = new Dictionary<string, IBrowserWindow>();
        private readonly IBrowserObject _browserObject;
        private readonly IUiHelper _uiHelper;
        private readonly IBrowserWindowsBuilder<IBrowserWindow> _browserWindowsBuilder;
        private volatile IAppDirectSession _session = null;
        private volatile IEnumerable<IApplication> _applications = null;

        public BrowserWindowsManager(IBrowserObject browserObject, IUiHelper uiHelper, IBrowserWindowsBuilder<IBrowserWindow> browserWindowsBuilder )
        {
            if (browserObject == null)
            {
                throw new ArgumentNullException("browserObject");
            }

            if (uiHelper == null)
            {
                throw new ArgumentNullException("uiHelper");
            }

            _browserObject = browserObject;
            _uiHelper = uiHelper;
            _browserWindowsBuilder = browserWindowsBuilder;
        }

        public virtual IEnumerable<IApplication> Applications
        {
            get
            {
                return _applications;
            }
            set
            {
                _applications = value;
                InitializeWindows();
            }
        }

        protected virtual void InitializeWindows()
        {
            lock (_lockObject)
            {
                foreach (var application in _applications)
                {
                    if (!_browserWindows.ContainsKey(application.Id) && !string.IsNullOrEmpty(application.UrlString))
                    {
                        var window = GetOrCreateBrowserWindow(application);
                        window.PreInitializeWindow();
                    }
                }
            }
        }

        public virtual IAppDirectSession Session
        {
            get
            {
                return _session;
            }
            set
            {
                _session = value;

                if ((_session != null) && (_session.Cookies.Count > 0))
                {
                    _browserObject.SetCookies(_session.Cookies);

                    IEnumerable<IBrowserWindow> windows;
                    lock (_lockObject)
                    {
                        windows = new List<IBrowserWindow>(_browserWindows.Values);
                    }

                    foreach (var browserWindow in windows)
                    {
                        browserWindow.SetSession(_session);
                    }
                }
            }
        }

        public virtual IBrowserWindow GetOrCreateBrowserWindow(IApplication application)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            var applicationId = application.Id;

            if (string.IsNullOrEmpty(applicationId))
            {
                throw new ArgumentNullException("application.Id");
            }

            lock (_lockObject)
            {
                if (!_browserWindows.ContainsKey(applicationId))
                {
                    var eventHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                    var model = new BrowserViewModel() { Application = application, Session = Session };

                    _uiHelper.PerformInUiThread(() =>
                        {
                            var browserWindow = _browserWindowsBuilder.CreateBrowserWindow(model);
                            _browserWindows[applicationId] = browserWindow;
                            eventHandle.Set();
                        });

                    eventHandle.WaitOne();
                }

                return _browserWindows[applicationId];
            }
        }

        public virtual IBrowserWindow GetBrowserWindow(string applicationId)
        {
            if (string.IsNullOrEmpty(applicationId))
            {
                throw new ArgumentNullException("applicationId");
            }

            lock (_lockObject)
            {
                if (!_browserWindows.ContainsKey(applicationId))
                {
                    return null;
                }

                return _browserWindows[applicationId];
            }
        }

        public void CloseAllWindows()
        {
            IEnumerable<IBrowserWindow> windows;
            lock (_lockObject)
            {
                windows = new List<IBrowserWindow>(_browserWindows.Values);
                _browserWindows.Clear();
            }

            _uiHelper.PerformInUiThread(() =>
                {
                    foreach (var browserWindow in windows)
                    {
                        browserWindow.Close();
                    }
                });
        }

        public virtual IEnumerable<IWindowData> GetBrowserWindowDatas()
        {
            var openWindows = new List<IWindowData>();
            _uiHelper.PerformInUiThread(() =>
                {
                    foreach (var window in _browserWindows)
                    {
                        if (window.Value.Visible)
                        {
                            openWindows.Add(new WindowData{
                                ApplicationId = window.Key,
                                WindowState = window.Value.WindowState
                            });
                        }
                    }
                });

            return openWindows;
        }
    }
}