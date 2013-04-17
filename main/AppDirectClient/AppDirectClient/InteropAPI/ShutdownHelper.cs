using System;
using System.Threading;

namespace AppDirect.WindowsClient.InteropAPI
{
    public class ShutdownHelper
    {
        private readonly TaskbarApi.ShutdownCallback _shutdownCallback;

        #region Singleton

        private static readonly object SyncObject = new object();
        private static volatile ShutdownHelper _instance = null;

        private ShutdownHelper()
        {
            _shutdownCallback = DoShutdown;
        }

        public static ShutdownHelper Instance
        {
            get
            {
                lock (SyncObject)
                {
                    if (_instance == null)
                    {
                        _instance = new ShutdownHelper();
                    }
                }

                return _instance;
            }
        }

        #endregion Singleton

        public void PerformInUiThread(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var currentApplication = System.Windows.Application.Current;
            if ((currentApplication == null) || (Thread.CurrentThread == currentApplication.Dispatcher.Thread))
            {
                action.Invoke();
            }
            else
            {
                currentApplication.Dispatcher.Invoke(action);
            }
        }

        private void DoShutdown()
        {
            TaskbarApi.Cleanup();

            PerformInUiThread(() => System.Windows.Application.Current.Shutdown());
        }

        public bool Shutdown()
        {
            return TaskbarApi.Instance.RemoveTaskbarWindowAndShutdown(_shutdownCallback);
        }
    }
}