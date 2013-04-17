namespace AppDirect.WindowsClient.Common.API
{
    public interface IServiceBuilder<T>
    {
        T CreateServiceAndTryToConnect();
    }
}