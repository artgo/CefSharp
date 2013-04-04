using System;

namespace AppDirect.WindowsClient.Common.UI
{
    public interface IUiHelper
    {
        void PerformInUiThread(Action action);

        void PerformForMinimumTime(Action action, bool requiresUiThread, int minimumMillisecondsBeforeReturn);

        void GracefulShutdown();

        void IgnoreException(Action action);
    }
}