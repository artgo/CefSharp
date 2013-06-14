using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;
using System.Collections.Generic;

namespace AppDirect.WindowsClient.API
{
    public class BrowserWindowsCommunicator : AbstractServiceClient<IBrowsersManagerApi>, IBrowserWindowsCommunicator
    {
        public BrowserWindowsCommunicator(IServiceBuilder<IBrowsersManagerApi> serviceStarter, IUiHelper uiHelper, ILogger log)
            : base(serviceStarter, uiHelper, log)
        {
        }

        public void DisplayApplication(IApplication application)
        {
            MakeSureExecuteAction(() => Service.DisplayApplication(application));
        }

        public void DisplayApplicationWithoutSession(IApplication application)
        {
            MakeSureExecuteAction(() => Service.DisplayApplicationWithoutSession(application));
        }

        public void DisplayApplications(IEnumerable<IApplicationWithState> applications)
        {
            MakeSureExecuteAction(() => Service.DisplayApplications(applications));
        }

        public void CloseApplication(string appId)
        {
            MakeSureExecuteAction(() => Service.CloseApplication(appId));
        }

        public void UpdateSession(IAppDirectSession newSession)
        {
            MakeSureExecuteAction(() => Service.UpdateSession(newSession));
        }

        public void UpdateApplications(IEnumerable<IApplication> applications)
        {
            MakeSureExecuteAction(() => Service.UpdateApplications(applications));
        }

        public void CloseAllApplicationsAndRemoveSessionInfo()
        {
            MakeSureExecuteAction(() => Service.CloseAllApplicationsAndRemoveSessionInfo());
        }

        public void CloseBrowserProcess()
        {
            MakeSureExecuteAction(() => Service.CloseBrowserProcess());
        }

        public IEnumerable<IWindowData> GetOpenWindowDatas()
        {
            return MakeSureExecuteAction(() => Service.GetOpenWindowDatas());
        }

        public int Ping(int value)
        {
            return MakeSureExecuteAction(() => Service.Ping(value));
        }
    }
}