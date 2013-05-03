using System;
using AppDirect.WindowsClient.Analytics;
using AppDirect.WindowsClient.Common.UI;
using AppDirect.WindowsClient.Tests.Common.UI;
using NSubstitute;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.Analytics
{
    [TestFixture]
    public class AsyncAnalyticsTest
    {
        private volatile IAnalytics _analytics = null;
        private volatile AsyncAnalytics _asyncAnalytics = null;
        private volatile IUiHelper _uiHelper = null;

        [SetUp]
        public void Init()
        {
            _uiHelper = new TestUiHelper();
            _analytics = Substitute.For<IAnalytics>();
            _asyncAnalytics = new AsyncAnalytics(_analytics, _uiHelper);
        }

        private void InitWithMocks()
        {
            _uiHelper = Substitute.For<IUiHelper>();
            _analytics = Substitute.For<IAnalytics>();
            _asyncAnalytics = new AsyncAnalytics(_analytics, _uiHelper);
        }

        [Test]
        public void TestNotifyCallsNotify()
        {
            _asyncAnalytics.Notify("1", "2", 3);
            _analytics.Received().Notify("1", "2", 3);
        }

        [Test]
        public void TestNotifySimpleActionCallsNotifySimpleAction()
        {
            _asyncAnalytics.NotifySimpleAction("1");
            _analytics.Received().NotifySimpleAction("1");
        }

        [Test]
        public void TestNotifyCallsAsync()
        {
            InitWithMocks();
            _asyncAnalytics.Notify("1", "2", 3);
            _uiHelper.Received().StartAsynchronously(Arg.Any<Action>());
        }

        [Test]
        public void TestNotifySimpleActionCallsAsync()
        {
            InitWithMocks();
            _asyncAnalytics.NotifySimpleAction("1");
            _uiHelper.Received().StartAsynchronously(Arg.Any<Action>());
        }
    }
}