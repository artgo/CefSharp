using System.Collections.Generic;
using System.ServiceModel;

namespace AppDirect.WindowsClient.Common.API
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IMainApplication : IPingable
    {
        [OperationContract(IsOneWay = true)]
        void Initialized();

        [OperationContract]
        IAppDirectSession GetSession();

        [OperationContract]
        IEnumerable<IApplication> GetMyApps();
    }
}
