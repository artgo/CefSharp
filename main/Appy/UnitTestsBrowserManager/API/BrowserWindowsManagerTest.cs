using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Browser.Interaction;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.UI;
using AppDirect.WindowsClient.Tests.Common.UI;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using Application = AppDirect.WindowsClient.Common.API.Application;

namespace AppDirect.WindowsClient.Browser.Tests.API
{
    [TestFixture]
    public class BrowserWindowsManagerTest
    {
        private volatile IBrowserObject _browserObject = null;
        private volatile IUiHelper _uiHelper = null;
        private volatile BrowserWindowsManager _browserWindowsManager = null;
        private TestBrowserWindowsBuilder _browserWindowBuilder;
        private IBrowserWindow _browserWindowMock;

        [SetUp]
        public void Init()
        {
            _uiHelper = new TestUiHelper();
            _browserObject = Substitute.For<IBrowserObject>();
            _browserWindowMock = Substitute.For<IBrowserWindow>();
            _browserWindowBuilder = new TestBrowserWindowsBuilder(_browserWindowMock);

            _browserWindowsManager = new BrowserWindowsManager(_browserObject, _uiHelper, _browserWindowBuilder);
        }

        [Test]
        public void TestSettingNotNullSessionCallsBrowserObjectSetCookies()
        {
            var cookies = new List<Cookie>();
            cookies.Add(new Cookie("1", "2"));
            var session = new AppDirectSession() { Cookies = cookies };
            _browserWindowsManager.Session = session;
            _browserObject.Received().SetCookies(cookies);
        }

        [Test]
        public void TestSettingNullSessionDoesNotCallsBrowserObjectSetCookies()
        {
            _browserWindowsManager.Session = null;
            _browserObject.DidNotReceiveWithAnyArgs().SetCookies(null);
        }

        [Test]
        [STAThread]
        public void TestGetOrCreateBrowserWindowReturnsNotNull()
        {
            var app = new Application() { Id = "1", UrlString = "url" };
            var result = _browserWindowsManager.GetOrCreateBrowserWindow(app);
            Assert.IsNotNull(result);
        }

        [Test]
        [STAThread]
        public void TestGetOrCreateBrowserWindowReturnsTheSameObjectSecondTime()
        {
            var app1 = new Application() { Id = "1", UrlString = "url" };
            var result1 = _browserWindowsManager.GetOrCreateBrowserWindow(app1);
            var app2 = new Application() { Id = "1", UrlString = "url" };
            var result2 = _browserWindowsManager.GetOrCreateBrowserWindow(app2);
            Assert.IsTrue(result1 == result2);
        }

        [Test]
        public void TestGetOrCreateThrowsOnNullId()
        {
            Assert.Throws<ArgumentNullException>(() => _browserWindowsManager.GetOrCreateBrowserWindow(new Application()));
        }

        [Test]
        public void TestGetOrCreateThrowsOnNullInput()
        {
            Assert.Throws<ArgumentNullException>(() => _browserWindowsManager.GetOrCreateBrowserWindow(null));
        }

        [Test]
        public void TestGetBrowserWindowThrowsOnNullInput()
        {
            Assert.Throws<ArgumentNullException>(() => _browserWindowsManager.GetBrowserWindow(null));
        }

        [Test]
        public void TestGetBrowserWindowReturnsNullIfNeverSpecifiedBefore()
        {
            Assert.IsNull(_browserWindowsManager.GetBrowserWindow("1"));
        }

        [Test]
        [STAThread]
        public void TestGetBrowserWindowReturnsTheSameObjectAsWasCreatedOriginally()
        {
            var app1 = new Application() { Id = "1", UrlString = "url" };
            var result1 = _browserWindowsManager.GetOrCreateBrowserWindow(app1);
            var result2 = _browserWindowsManager.GetBrowserWindow("1");
            Assert.IsTrue(result1 == result2);
        }

        [Test]
        [STAThread]
        public void TestSettingApplicationsPreInitializesAllWindows()
        {
            var apps = new List<IApplication>()
            {
                new Application() { Id = "1", UrlString = "http://google.com" },
                new Application() { Id = "2", UrlString = "http://google.com" },
                new Application() { Id = "3", UrlString = "http://google.com" }
            };

            var browserWindowMock = Substitute.For<IBrowserWindow>();
            var browserWindowsManager = new BrowserWindowsManager(_browserObject, _uiHelper, new TestBrowserWindowsBuilder(browserWindowMock));
            browserWindowsManager.Applications = apps;
            browserWindowMock.Received(apps.Count).PreInitializeWindow();
        }
    }
}