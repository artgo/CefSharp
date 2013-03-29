namespace AppDirect.WindowsClient.API
{
    public interface ILatch
    {
        bool Wait();
        bool Unlock();
    }
}