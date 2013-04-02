using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Common.API;
using System.ServiceModel;

namespace AppDirect.WindowsClient.Browser.Tests.API
{
    public class TestIpcMainWindowStarter : IpcMainWindowStarter
    {
        private readonly ICommunicationObject _communicationObjectMock;

        public TestIpcMainWindowStarter(IBrowsersManagerApi service, ICommunicationObject communicationObjectMock)
            : base(service)
        {
            _communicationObjectMock = communicationObjectMock;
        }

        protected override ICommunicationObject CreateServiceHost(IBrowsersManagerApi service)
        {
            return _communicationObjectMock;
        }
    }
}