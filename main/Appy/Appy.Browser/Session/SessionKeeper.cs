using AppDirect.WindowsClient.Browser.Control;
using System;
using System.Threading;

namespace AppDirect.WindowsClient.Browser.Session
{
    /// <summary>
    /// Keeping session alive for the duration of object live
    /// </summary>
    public class SessionKeeper : IDisposable
    {
        private static readonly TimeSpan TimeBetweenUpdates = TimeSpan.FromMinutes(5);
        private readonly Thread _updaterThread;
        private readonly WpfCefBrowser _browser = new WpfCefBrowser();
        private readonly string _url = null;
        private volatile bool _stopFlag = false;
        private readonly ThreadStart _sessionUpdator; 

        public SessionKeeper(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            _sessionUpdator = KeepUpdatingSession;
            _url = url;
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
                    ReloadSession();
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception)
                {
                    // Ignore all the rest
                }
            }
        }

        private void ReloadSession()
        {
            _browser.NavigateTo(_url);
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