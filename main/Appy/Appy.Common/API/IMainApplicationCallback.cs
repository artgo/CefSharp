using System.ServiceModel;

namespace AppDirect.WindowsClient.Common.API
{
    public interface IMainApplicationCallback
    {
        [OperationContract]
        void CloseWindow();
    }
}
