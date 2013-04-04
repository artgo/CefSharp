using AppDirect.WindowsClient.Common.UI;
using System;

namespace AppDirect.WindowsClient.Tests.Common.UI
{
    public class TestUiHelper : IUiHelper
    {
        public virtual bool WasShutdown { get; private set; }

        public virtual void PerformInUiThread(Action action)
        {
            action.Invoke();
        }

        public virtual void PerformForMinimumTime(Action action, bool requiresUiThread, int minimumMillisecondsBeforeReturn)
        {
            action.Invoke();
        }

        public virtual void GracefulShutdown()
        {
            WasShutdown = true;
        }

        public void IgnoreException(Action action)
        {
            action.Invoke();
        }
    }
}