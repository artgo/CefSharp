namespace AppDirect.WindowsClient.Common.API
{
    public interface IAbstractServiceRunner<T> : IStartStop where T : class 
    {
        T Service { get; }
    }
}