using System.ServiceModel;

namespace AppDirect.WindowsClient.Common.API
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IMainApplication
    {
        [OperationContract]
        IInitData Initialized();
    }
}
