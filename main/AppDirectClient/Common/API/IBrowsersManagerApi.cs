using System.Collections.Generic;
using System.ServiceModel;

namespace AppDirect.WindowsClient.Common.API
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IBrowsersManagerApi : IPingable
    {
        [OperationContract(IsOneWay = true)]
        void DisplayApplication(IApplication application);

        [OperationContract(IsOneWay = true)]
        void DisplayApplicationWithoutSession(IApplication application);

        [OperationContract(IsOneWay = true)]
        void DisplayApplications(IEnumerable<IApplicationWithState> applications);

        [OperationContract(IsOneWay = true)]
        void CloseApplication(string appId);

        [OperationContract(IsOneWay = true)]
        void UpdateSession(IAppDirectSession newSession);

        [OperationContract(IsOneWay = true)]
        void UpdateApplications(IEnumerable<IApplication> applications);

        [OperationContract(IsOneWay = true)]
        void CloseAllApplicationsAndRemoveSessionInfo();

        [OperationContract(IsOneWay = true)]
        void CloseBrowserProcess();

        [OperationContract(IsOneWay = false)]
        IEnumerable<IWindowData> GetOpenWindowDatas();
    }
}