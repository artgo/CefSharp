using AppDirect.WindowsClient.Common;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;
using NSubstitute;
using NUnit.Framework;
using System;

namespace AppDirect.WindowsClient.Tests.Common.API
{
    [TestFixture]
    public class ProcessWatcherTests
    {
        private IAbstractProcess _mockProcess;
        private ProcessWatcher _processWatcher;
        private IUiHelper _uiHelper;

        private void TestEvent(object o, EventArgs e)
        {
        }

        [SetUp]
        public void Init()
        {
            _mockProcess = Substitute.For<IAbstractProcess>();
            var mockLogger = Substitute.For<NLogLogger>();
            _uiHelper = Substitute.For<IUiHelper>();
            _processWatcher = new ProcessWatcher("testProcess", _mockProcess, mockLogger, _uiHelper);
        }

        [Test]
        public void StartGetsProcess()
        {
            _processWatcher.Start();
            _mockProcess.ReceivedWithAnyArgs().GetProcess();
        }

        [Test]
        public void StartRegistersListener()
        {
            _processWatcher.Start();
            _mockProcess.ReceivedWithAnyArgs().RegisterExitedEvent(TestEvent);
        }

        [Test]
        public void StopRemovesRegisterEvent()
        {
            _processWatcher.Stop();
            _mockProcess.ReceivedWithAnyArgs().RemoveRegisteredEvent(TestEvent);
        }
    }
}