using System;
using System.Threading;

namespace AppDirect.WindowsClient.Common.UI
{
    public class UiHelper : IUiHelper
    {
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
            PerformInUiThread(() => System.Windows.Application.Current.Shutdown());
        }
    }
}
