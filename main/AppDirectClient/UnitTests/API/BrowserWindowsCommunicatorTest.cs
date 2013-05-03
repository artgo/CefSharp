using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace AppDirect.WindowsClient.Tests.API
{
    [TestFixture]
    public class BrowserWindowsCommunicatorTest
    {
        private volatile IServiceBuilder<IBrowsersManagerApi> _serviceStarter;
        private volatile IUiHelper _uiHelper;
        private volatile ILogger _log;
        private volatile ICommunicationObject _communicationObject;
        private volatile IBrowsersManagerApi _browserApi;
        private volatile BrowserWindowsCommunicator _browserWindowsCommunicator;

        [SetUp]
        public void Init()
        {
            _browserApi = Substitute.For<IBrowsersManagerApi, ICommunicationObject>();
            _serviceStarter = Substitute.For<IServiceBuilder<IBrowsersManagerApi>>();
            _serviceStarter.CreateServiceAndTryToConnect().Returns(_browserApi);
            _uiHelper = Substitute.For<IUiHelper>();
            _log = Substitute.For<ILogger>();
            _communicationObject = (ICommunicationObject)_browserApi;
            _browserWindowsCommunicator = new BrowserWindowsCommunicator(_serviceStarter, _uiHelper, _log);
        }

        [Test]
        public void TestStopThrowsIfNeverStarted()
        {
            Assert.Throws<InvalidOperationException>(() => _browserWindowsCommunicator.Stop());
        }

        [Test]
        public void TestIfRemoteThrowsWeCallItAgain()
        {
            var thrownAlready = false;
            _browserApi
                .When(x => x.DisplayApplication(Arg.Any<IApplication>()))
                .Do(x =>
                {
                    if (!thrownAlready)
                    {
                        thrownAlready = true;
                        throw new CommunicationException();
                    }
                });
            _browserWindowsCommunicator.Start();
            _browserWindowsCommunicator.DisplayApplication(new Application());
            _browserApi.ReceivedWithAnyArgs(2).DisplayApplication(null);
        }

        [Test]
        public void TestDisplayApplicationCallsDisplayApplication()
        {
            var app = new Application();
            _browserWindowsCommunicator.Start();
            _browserWindowsCommunicator.DisplayApplication(app);
            _browserApi.Received().DisplayApplication(app);
        }

        [Test]
        public void TestCloseAllApplicationsAndQuitCallsCloseAllApplicationsAndQuit()
        {
            _browserWindowsCommunicator.Start();
            _browserWindowsCommunicator.CloseAllApplicationsAndRemoveSessionInfo();
            _browserApi.Received().CloseAllApplicationsAndRemoveSessionInfo();
        }

        [Test]
        public void TestUpdateSessionCallsUpdateSession()
        {
            var session = new AppDirectSession();
            _browserWindowsCommunicator.Start();
            _browserWindowsCommunicator.UpdateSession(session);
            _browserApi.Received().UpdateSession(session);
        }

        [Test]
        public void TestUpdateApplicationsCallsUpdateApplications()
        {
            var applications = new List<IApplication>();
            _browserWindowsCommunicator.Start();
            _browserWindowsCommunicator.UpdateApplications(applications);
            _browserApi.Received().UpdateApplications(applications);
        }
    }
}