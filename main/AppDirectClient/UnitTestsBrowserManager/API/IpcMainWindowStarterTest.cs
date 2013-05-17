using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Common.API;
using NSubstitute;
using NUnit.Framework;
using System;
using System.ServiceModel;

namespace AppDirect.WindowsClient.Browser.Tests.API
{
    [TestFixture]
    public class IpcMainWindowStarterTest
    {
        private volatile IBrowsersManagerApi _browsersManagerApi = null;
        private volatile ICommunicationObject _communicationObjectMock = null;
        private volatile IpcMainWindowStarter _ipcMainWindowStarter = null;

        [SetUp]
        public void Init()
        {
            _browsersManagerApi = Substitute.For<IBrowsersManagerApi>();
            _communicationObjectMock = Substitute.For<ICommunicationObject>();
            _ipcMainWindowStarter = new TestIpcMainWindowStarter(_browsersManagerApi, _communicationObjectMock);
        }

        [Test]
        public void TestStartCallsOpenOnCommunicationObject()
        {
            _ipcMainWindowStarter.Start();
            _communicationObjectMock.Received().Open();
        }

        [Test]
        public void TestInCaseIfStateIsOpenedStopCallsClose()
        {
            _ipcMainWindowStarter.Start();
            _communicationObjectMock.State.ReturnsForAnyArgs(CommunicationState.Opened);
            _ipcMainWindowStarter.Stop();
            _communicationObjectMock.Received().Close();
        }

        [Test]
        public void TestInCaseIfStateIsClosedStopDoesNotCallClose()
        {
            _ipcMainWindowStarter.Start();
            _communicationObjectMock.State.ReturnsForAnyArgs(CommunicationState.Closed);
            _ipcMainWindowStarter.Stop();
            _communicationObjectMock.DidNotReceiveWithAnyArgs().Close();
        }

        [Test]
        public void TestServiceReturnProvidedService()
        {
            Assert.AreSame(_browsersManagerApi, _ipcMainWindowStarter.Service);
        }

        [Test]
        public void TestStopThrowsIfNeverStarted()
        {
            Assert.Throws<InvalidOperationException>(() => _ipcMainWindowStarter.Stop());
        }

        [Test]
        public void TestStopStopsWatcher()
        {
            var processWatcher = Substitute.For<IStartStop>();
            _browsersManagerApi = Substitute.For<IBrowsersManagerApi>();
            var ipcMainWindowStarter = new IpcMainWindowStarter(_browsersManagerApi);
            ipcMainWindowStarter.Start(processWatcher);
            processWatcher.Received().Stop();
        }

        [Test]
        public void TestStartStartsWatcher()
        {
            var processWatcher = Substitute.For<IStartStop>();
            _browsersManagerApi = Substitute.For<IBrowsersManagerApi>();
            var ipcMainWindowStarter = new IpcMainWindowStarter(_browsersManagerApi);
            ipcMainWindowStarter.Start(processWatcher);
            processWatcher.Received().Start();
        }

    }
}