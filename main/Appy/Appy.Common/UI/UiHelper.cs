using System;
using System.Threading;
using AppDirect.WindowsClient.Common.Log;

namespace AppDirect.WindowsClient.Common.UI
{
    public class UiHelper : IUiHelper
    {
        private readonly ILogger _log;

        public UiHelper(ILogger log)
        {
            _log = log;
        }

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

        public void PerformForMinimumTime(Action action, bool requiresUiThread, int minimumMillisecondsBeforeReturn)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var startTime = Environment.TickCount;

            if (requiresUiThread)
            {
                PerformInUiThread(action);
            }
            else
            {
                action.Invoke();
            }

            var remainingTime = minimumMillisecondsBeforeReturn - (Environment.TickCount - startTime);

            if (remainingTime > 0)
            {
                Thread.Sleep(remainingTime);
            }
        }

        public void GracefulShutdown()
        {
            _log.Info("Started shutdown");
            var eventHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            PerformInUiThread(() =>
                {
                    System.Windows.Application.Current.Shutdown();
                    _log.Info("Application shutdown complete");
                    eventHandle.Set();
                });

            eventHandle.WaitOne(TimeSpan.FromSeconds(5.0));

            _log.Info("Shutdown complete");
            Environment.Exit(0);
        }

        public void IgnoreException(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                _log.ErrorException("Invokation failed", e);
            }
        }

        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }
    }
}
