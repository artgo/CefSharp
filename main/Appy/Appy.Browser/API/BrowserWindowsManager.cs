using System.Threading;
using AppDirect.WindowsClient.Browser.Interaction;
using AppDirect.WindowsClient.Browser.UI;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.UI;
using System;
using System.Collections.Generic;

namespace AppDirect.WindowsClient.Browser.API
{
    public class BrowserWindowsManager : IBrowserWindowsManager
    {
        private readonly object _lockObject = new object();
        private readonly IDictionary<string, BrowserWindow> _browserWindows = new Dictionary<string, BrowserWindow>();
        private readonly IBrowserObject _browserObject;
        private readonly IUiHelper _uiHelper;
        private volatile IAppDirectSession _session = null;
        private volatile IEnumerable<IApplication> _applications = null;

        public BrowserWindowsManager(IBrowserObject browserObject, IUiHelper uiHelper)
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

                if (_session != null)
                {
                    _browserObject.SetCookies(_session.Cookies);
                }
            }
        }

        public virtual BrowserWindow GetOrCreateBrowserWindow(IApplication application)
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
                            var browserWindow = new BrowserWindow(model);
                            _browserWindows[applicationId] = browserWindow;
                            eventHandle.Set();
                        });

                    eventHandle.WaitOne();
                }

                return _browserWindows[applicationId];
            }
        }

        public virtual BrowserWindow GetBrowserWindow(string applicationId)
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
    }
}