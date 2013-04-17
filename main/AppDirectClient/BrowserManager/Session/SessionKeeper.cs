using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using System;
using System.Threading;
using Xilium.CefGlue.WPF;

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
        private readonly ThreadStart _sessionUpdator;
        private readonly MainApplicationServiceClient _mainAppClient;
        private readonly ILogger _log;
        private volatile bool _stopFlag = false;

        public SessionKeeper(MainApplicationServiceClient mainAppClient, IBrowserWindowsManager browserWindowsManager, ILogger log)
        {
            if (mainAppClient == null)
            {
                throw new ArgumentNullException("mainAppClient");
            }

            if (browserWindowsManager == null)
            {
                throw new ArgumentNullException("browserWindowsManager");
            }

            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            _mainAppClient = mainAppClient;
            _log = log;
            _sessionUpdator = KeepUpdatingSession;
            _browserWindowsManager = browserWindowsManager;
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
                Thread.Sleep(TimeBetweenUpdates);

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
            _browserWindowsManager.Session = session;

            var apps = _mainAppClient.GetMyApps();
            _browserWindowsManager.Applications = apps;

            foreach (var app in _browserWindowsManager.Applications)
            {
                if ((app != null) && !string.IsNullOrEmpty(app.UrlString))
                {
                    var browser = new WpfCefBrowser();
                    browser.NavigateTo(app.UrlString);
                }
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