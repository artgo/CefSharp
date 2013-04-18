using NLog;

namespace AppDirect.WindowsClient.Common.Log
{
    public interface ILogInitializer
    {
        Logger CreateLogger(); 
    }
}