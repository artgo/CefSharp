using System.ServiceModel;

namespace AppDirect.WindowsClient.Common.API
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMainApplication" in both code and config file together.
    [ServiceContract(CallbackContract = typeof(IMainApplicationCallback), SessionMode = SessionMode.Required)]
    public interface IMainApplication
    {
        [OperationContract]
        IApplication GetApplicationById(string id);

        [OperationContract]
        IAppDirectSession GetCurrentSession();

        [OperationContract(IsOneWay = true, IsTerminating = true)]
        void BrowserWasClosed();
    }
}
