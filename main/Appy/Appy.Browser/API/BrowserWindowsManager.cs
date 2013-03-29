using AppDirect.WindowsClient.Browser.Interaction;
using AppDirect.WindowsClient.Browser.UI;
using AppDirect.WindowsClient.Common;
using AppDirect.WindowsClient.Common.API;
using System;
using System.Collections.Generic;
using System.Windows;

namespace AppDirect.WindowsClient.Browser.API
{
    public class BrowserWindowsManager
    {
        private readonly object _lockObject = new object();
        private readonly IDictionary<string, Window> _browserWindows = new Dictionary<string, Window>();
        private readonly BrowserObject _browserObject;
        private volatile IAppDirectSession _session = null;

        public BrowserWindowsManager(BrowserObject browserObject)
        {
            _browserObject = browserObject;
        }

        public IEnumerable<IApplication> Applications { get; set; }

        public IAppDirectSession Session
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

        public Window GetOrCreateWindow(IApplication application)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            var applicationId = application.Id;

            lock (_lockObject)
            {
                if (!_browserWindows.ContainsKey(applicationId))
                {
                    var model = new BrowserViewModel() { Application = application, Session = Session };
                    CommonHelper.PerformInUiThread(() =>
                        {
                            var browserWindow = new BrowserWindow(model);
                            _browserWindows[applicationId] = browserWindow;
                        });
                }

                return _browserWindows[applicationId];
            }
        }

        public Window GetWindow(string applicationId)
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