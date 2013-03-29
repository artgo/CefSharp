using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Browser.Control;
using AppDirect.WindowsClient.Common.API;
using System;
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
        private readonly BrowserWindowsManager _browserWindowsManager;
        private volatile bool _stopFlag = false;
        private readonly ThreadStart _sessionUpdator;

        public SessionKeeper(BrowserWindowsManager browserWindowsManager)
        {
            if (browserWindowsManager == null)
            {
                throw new ArgumentNullException("browserWindowsManager");
            }

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
                catch (Exception)
                {
                    // Ignore all the rest
                }
            }
        }

        private void ReloadSessions()
        {
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
                catch (Exception)
                {
                    // Ignore termination errors
                }
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}