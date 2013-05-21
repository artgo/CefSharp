using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppDirect.WindowsClient.Common;
using AppDirect.WindowsClient.Common.Log;
using NSubstitute;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.Common.API
{
    [TestFixture]
    public class ProcessWatcherTests
    {
        private IAbstractProcess _mockProcess;
        private ProcessWatcher _processWatcher;

        private void TestEvent(object o, EventArgs e)
        {
        }

        [SetUp]
        public void Init()
        {
            _mockProcess = Substitute.For<IAbstractProcess>();
            _processWatcher = new ProcessWatcher("testProcess", _mockProcess,
                                                    new NLogLogger("testLogger"));
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
