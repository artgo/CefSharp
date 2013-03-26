using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.API;
using NSubstitute;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.API
{
    [TestFixture]
    public class BrowserWindowsCommunicatorTest
    {
        [Test]
        public void BrowserWindowsCommunicatorCloseAppCallsIpcCloseBrowser()
        {
            var ipcCommunicatorMock = Substitute.For<IIpcCommunicator>();
            var communicator = new BrowserWindowsCommunicator(ipcCommunicatorMock);
            const string id = "1";
            var app = new Application() { Id = id };
            communicator.CloseApp(app);
            ipcCommunicatorMock.Received().CloseBrowser(id);
        }

        [Test]
        public void BrowserWindowsCommunicatorOpenOrActivateAppCallsIpcActivateBrowserIfExists()
        {
            var ipcCommunicatorMock = Substitute.For<IIpcCommunicator>();
            ipcCommunicatorMock.ActivateBrowserIfExists(Arg.Any<string>()).Returns(true);
            var communicator = new BrowserWindowsCommunicator(ipcCommunicatorMock);
            const string id = "1";
            var app = new Application() { Id = id };
            communicator.OpenOrActivateApp(app);
            ipcCommunicatorMock.Received().ActivateBrowserIfExists(id);
        }
    }
}