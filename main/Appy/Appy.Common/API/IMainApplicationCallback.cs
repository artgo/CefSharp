using System.ServiceModel;

namespace AppDirect.WindowsClient.Common.API
{
    public interface IMainApplicationCallback
    {
        [OperationContract(IsTerminating = true, IsOneWay = true)]
        void CloseWindow();

        [OperationContract(IsOneWay = true)]
        void ActivateWindow();
    }
}
