using System;

namespace AppDirect.WindowsClient.Common.API
{
    public interface IAbstractServiceClient<T> : IAbstractServiceRunner<T> where T: class
    {
        bool TryToStart();

        void StartIfNotStarted();

        void MakeSureExecuteAction(Action action);

        TR MakeSureExecuteAction<TR>(Func<TR> action);
    }
}