using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Browser.UI;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AppDirect.WindowsClient.Browser.Session
{
    /// <summary>
    /// Keeping session alive for the duration of object live
    /// </summary>
    public class SessionKeeper : IDisposable, IStartStop
    {
        private static readonly TimeSpan TimeBetweenUpdates = TimeSpan.FromMinutes(3);
        private readonly Thread _updaterThread;
        private readonly IBrowserWindowsManager _browserWindowsManager;
        private readonly IUiHelper _uiHelper;
        private readonly ThreadStart _sessionUpdator;
        private readonly MainApplicationServiceClient _mainAppClient;
        private readonly IDictionary<string, IBrowserWindow> _browserWindows = new Dictionary<string, IBrowserWindow>();
        private readonly IBrowserWindowsBuilder<IBrowserWindow> _browserWindowsBuilder;
        private readonly ILogger _log;
        private volatile bool _stopFlag = false;

        public SessionKeeper(MainApplicationServiceClient mainAppClient, IBrowserWindowsManager browserWindowsManager,
            IBrowserWindowsBuilder<IBrowserWindow> browserWindowsBuilder, ILogger log, IUiHelper uiHelper)
        {
            if (mainAppClient == null)
            {
                throw new ArgumentNullException("mainAppClient");
            }

            if (browserWindowsManager == null)
            {
                throw new ArgumentNullException("browserWindowsManager");
            }

            if (browserWindowsBuilder == null)
            {
                throw new ArgumentNullException("browserWindowsBuilder");
            }

            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            if (uiHelper == null)
            {
                throw new ArgumentNullException("uiHelper");
            }

            _mainAppClient = mainAppClient;
            _log = log;
            _uiHelper = uiHelper;
            _sessionUpdator = KeepUpdatingSession;
            _browserWindowsManager = browserWindowsManager;
            _browserWindowsBuilder = browserWindowsBuilder;
            _updaterThread = new Thread(_sessionUpdator);
        }

        public void Start()
        {
            _stopFlag = false;

            _updaterThread.Start();
        }

        private void KeepUpdatingSession()
        {
            while (true)
            {
                _uiHelper.Sleep(TimeBetweenUpdates);

                if (_stopFlag)
                {
                    return;
                }

                try
                {
                    ReloadSessions();
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (ThreadInterruptedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    _log.ErrorException("Exception while reloading session", e);
                }
            }
        }

        private void ReloadSessions()
        {
            var session = _mainAppClient.GetSession();

            if ((session == null) || (session.Cookies.Count <= 0))
            {
                return;
            }

            _browserWindowsManager.Session = session;

            var apps = _mainAppClient.GetMyApps();
            _browserWindowsManager.Applications = apps;

            _uiHelper.PerformInUiThread(() =>
                {
                    foreach (var app in apps)
                    {
                        if ((app == null) || string.IsNullOrEmpty(app.UrlString) || _browserWindows.ContainsKey(app.Id))
                            continue;

                        var model = new BrowserViewModel() { Application = app, Session = session };

                        var browserWindow = _browserWindowsBuilder.CreateBrowserWindow(model);
                        _browserWindows[app.Id] = browserWindow;
                    }
                });

            foreach (var browserWindow in _browserWindows.Values)
            {
                browserWindow.SetSession(session);
            }
        }

        public void Stop()
        {
            _stopFlag = true;

            if (_updaterThread.IsAlive)
            {
                try
                {
                    _updaterThread.Abort();
                }
                catch (Exception e)
                {
                    _log.ErrorException("Exception while stopping session keeper", e);
                }
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}