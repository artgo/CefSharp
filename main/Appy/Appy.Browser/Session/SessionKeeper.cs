using System;
using System.Threading;
using AppDirect.WindowsClient.Browser.Control;

namespace AppDirect.WindowsClient.Browser.Session
{
    /// <summary>
    /// Keeping session alive for the duration of object live
    /// </summary>
    public class SessionKeeper : IDisposable
    {
        private static readonly TimeSpan TimeBetweenUpdates = TimeSpan.FromMinutes(10);
        private readonly Thread _updaterThread;
        private readonly WpfCefBrowser _browser = new WpfCefBrowser();
        private readonly string _url = null;

        public SessionKeeper(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            _url = url;
            _updaterThread = new Thread(KeepUpdatingSession);
        }

        public void Start()
        {
            _updaterThread.Start();

        }

        private void KeepUpdatingSession()
        {
            while (true)
            {
                Thread.Sleep(TimeBetweenUpdates);

                ReloadSession();
            }
        }

        private void ReloadSession()
        {
            _browser.NavigateTo(_url);
        }

        public void Stop()
        {
            if (_updaterThread.IsAlive)
            {
                _updaterThread.Abort();
                _updaterThread.Join();
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}