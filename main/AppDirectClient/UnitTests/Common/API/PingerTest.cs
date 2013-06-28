using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading;

namespace AppDirect.WindowsClient.Tests.Common.API
{
    [TestFixture]
    public class PingerTest
    {
        private IUiHelper _uiHelper;
        private ILogger _logger;
        private IPingable _pingable;
        private IProcessRestarter _processRestarter;
        private Pinger _pinger;

        [SetUp]
        public void Init()
        {
            _uiHelper = Substitute.For<IUiHelper>();
            _logger = Substitute.For<ILogger>();
            _pingable = Substitute.For<IPingable>();
            _processRestarter = Substitute.For<IProcessRestarter>();
            _pinger = new Pinger(_uiHelper, _logger, _pingable, _processRestarter);
        }

        [Test]
        public void TestStopThrowsIfNeverStarted()
        {
            Assert.Throws<InvalidOperationException>(() => _pinger.Stop());
        }

        [Test]
        public void TestStartThrowsIfAlreadyStarted()
        {
            _uiHelper.StartAsynchronously(Arg.Any<Action>()).Returns(new Thread(() => { }));
            _pinger.Start();
            Assert.Throws<InvalidOperationException>(() => _pinger.Start());
        }

        [Test]
        public void ThirdFailedAttemptTriggersRetry()
        {
            _uiHelper.GetCurrentMilliseconds().Returns(1);
            _pingable.Ping(Arg.Any<int>()).Returns(x => { throw new Exception(); });
            _pinger.TestPing(2);
            _processRestarter.Received().RestartProcess();
        }

        [Test]
        public void ThirdSuccessAttemptDoesNotTriggerRetry()
        {
            _uiHelper.GetCurrentMilliseconds().Returns(1);
            _pingable.Ping(Arg.Any<int>()).Returns(x => 2);
            _pinger.TestPing(2);
            _processRestarter.DidNotReceive().RestartProcess();
        }
    }
}