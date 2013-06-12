using System;

namespace AppDirect.WindowsClient.Common.UI
{
    public interface IUiHelper
    {
        string GetPrevLocation(int depth);

        void PerformInUiThread(Action action);

        void StartAsynchronously(Action action);

        void PerformForMinimumTime(Action action, bool requiresUiThread, int minimumMillisecondsBeforeReturn);

        void GracefulShutdown();

        void IgnoreException(Action action);

        void Sleep(int milliseconds);

        void Sleep(TimeSpan sleepTime);
    }
}