using System;
using System.Threading;
using System.Windows;
using AppDirect.WindowsClient.UI;

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

            var eventHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            PerformInUiThread(() => { 
                Application.Current.Shutdown();
                eventHandle.Set();
            });

            eventHandle.WaitOne(TimeSpan.FromMinutes(5.0));

            Environment.Exit(0);
        }

        public bool Shutdown()
        {
            return TaskbarApi.Instance.RemoveTaskbarWindowAndShutdown(_shutdownCallback);
        }
    }
}