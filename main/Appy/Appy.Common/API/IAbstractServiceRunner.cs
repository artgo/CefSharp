namespace AppDirect.WindowsClient.Common.API
{
    public interface IAbstractServiceRunner<T> : IStartStop
    {
        T Service { get; }
    }
}