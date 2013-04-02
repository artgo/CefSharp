using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.API;
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
        private volatile ILatch _latch = null;
        private volatile IBrowsersManagerApi _browserApi = null;
        private volatile ICommunicationObject _communicationObject = null;
        private volatile TestBrowserWindowsCommunicator _browserWindowsCommunicator = null;

        private class TestBrowserWindowsCommunicator : BrowserWindowsCommunicator
        {
            private readonly IBrowsersManagerApi _browserApi = null;
            private readonly ICommunicationObject _communicationObject = null;

            public int CreateBrowsersManagerApiClientTimesCalled = 0;
            public int GetCommunicationObjectTimesCalled = 0;

            public TestBrowserWindowsCommunicator(ILatch latch, IBrowsersManagerApi browserApi, ICommunicationObject communicationObject)
                : base(latch)
            {
                _browserApi = browserApi;
                _communicationObject = communicationObject;
            }

            protected internal override IBrowsersManagerApi CreateBrowsersManagerApiClient()
            {
                CreateBrowsersManagerApiClientTimesCalled++;
                return _browserApi;
            }

            protected internal override ICommunicationObject GetCommunicationObject()
            {
                GetCommunicationObjectTimesCalled++;
                return _communicationObject;
            }
        }

        [SetUp]
        public void Init()
        {
            _latch = Substitute.For<ILatch>();
            _browserApi = Substitute.For<IBrowsersManagerApi, ICommunicationObject>();
            _communicationObject = (ICommunicationObject)_browserApi;
            _browserWindowsCommunicator = new TestBrowserWindowsCommunicator(_latch, _browserApi, _communicationObject);
        }

        [Test]
        public void TestStopThrowsIfNeverStarted()
        {
            Assert.Throws<InvalidOperationException>(() => _browserWindowsCommunicator.Stop());
        }

        [Test]
        public void TestDisplayApplicationCallsLatchWait()
        {
            _browserWindowsCommunicator.Start();
            _browserWindowsCommunicator.DisplayApplication(new Application());
            _latch.Received().Wait();
        }

        [Test]
        public void TestDisplayApplicationThrowsIfNotStarted()
        {
            Assert.Throws<NullReferenceException>(() => _browserWindowsCommunicator.DisplayApplication(new Application()));
        }

        [Test]
        public void TestDisplayApplicationThrowsIfStopped()
        {
            _browserWindowsCommunicator.Start();
            _browserWindowsCommunicator.Stop();
            Assert.Throws<NullReferenceException>(() => _browserWindowsCommunicator.DisplayApplication(new Application()));
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
        public void TestIfRemoteThrowsWeCallCreateBrowsersManagerApiClient()
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
            Assert.AreEqual(2, _browserWindowsCommunicator.CreateBrowsersManagerApiClientTimesCalled);
        }

        [Test]
        public void TestIfRemoteThrowsWeCallGetCommunicationObject()
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
            Assert.AreEqual(2, _browserWindowsCommunicator.GetCommunicationObjectTimesCalled);
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
        public void TestCloaseAllApplicationsAndQuitCallsCloaseAllApplicationsAndQuit()
        {
            _browserWindowsCommunicator.Start();
            _browserWindowsCommunicator.CloaseAllApplicationsAndQuit();
            _browserApi.Received().CloaseAllApplicationsAndQuit();
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