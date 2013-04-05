using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;
using NSubstitute;
using NUnit.Framework;
using System;
using System.ServiceModel;

namespace AppDirect.WindowsClient.Tests.Common.API
{
    [TestFixture]
    public class AbstractServiceClientTest
    {
        private volatile IBrowsersManagerApi _service;
        private volatile IServiceBuilder<IBrowsersManagerApi> _serviceStarter;
        private volatile IUiHelper _uiHelper;
        private volatile ILogger _log;
        private volatile ICommunicationObject _communicationObject;
        private volatile AbstractServiceClient<IBrowsersManagerApi> _serviceClient;

        [SetUp]
        public void Init()
        {
            _service = Substitute.For<IBrowsersManagerApi, ICommunicationObject>();
            _serviceStarter = Substitute.For<IServiceBuilder<IBrowsersManagerApi>>();
            _serviceStarter.CreateServiceAndTryToConnect().Returns(_service);
            _uiHelper = Substitute.For<IUiHelper>();
            _log = Substitute.For<ILogger>();
            _communicationObject = (ICommunicationObject)_service;
            _serviceClient = new AbstractServiceClient<IBrowsersManagerApi>(_serviceStarter, _uiHelper, _log);
        }

        [Test]
        public void TestStopThrowsIfNeverStarted()
        {
            Assert.Throws<InvalidOperationException>(() => _serviceClient.Stop());
        }

        [Test]
        public void TestForMakeSureExecuteActionIfRemoteThrowsWeCallItAgain()
        {
            var thrownAlready = false;
            var timesCalled = 0;
            _serviceClient.Start();
            _serviceClient.MakeSureExecuteAction(() =>
                {
                    timesCalled++;
                    if (!thrownAlready)
                    {
                        thrownAlready = true;
                        throw new CommunicationException();
                    }
                });

            Assert.AreEqual(2, timesCalled);
        }

        [Test]
        public void TestForMakeSureExecuteActionWeCallActionOnlyOnce()
        {
            var timesCalled = 0;
            _serviceClient.Start();
            _serviceClient.MakeSureExecuteAction(() =>
            {
                timesCalled++;
            });

            Assert.AreEqual(1, timesCalled);
        }

        [Test]
        public void TestStartCallsServiceStarter()
        {
            _serviceClient.Start();
            _serviceStarter.ReceivedWithAnyArgs().CreateServiceAndTryToConnect();
        }

        [Test]
        public void TestStopCallsCloseOnCommunicationObjectIsStateIsOpen()
        {
            _communicationObject.State.Returns(CommunicationState.Opened);
            _serviceClient.Start();
            _serviceClient.Stop();
            _communicationObject.Received().Close();
        }

        [Test]
        public void TestStopDoesNotCallCloseOnCommunicationObjectIsStateIsClose()
        {
            _communicationObject.State.Returns(CommunicationState.Closed);
            _serviceClient.Start();
            _serviceClient.Stop();
            _communicationObject.DidNotReceive().Close();
        }
    }
}