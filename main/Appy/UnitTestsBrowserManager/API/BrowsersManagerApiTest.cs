using System.Windows;
using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Browser.UI;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.UI;
using AppDirect.WindowsClient.Tests.Common.UI;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Application = AppDirect.WindowsClient.Common.API.Application;

namespace AppDirect.WindowsClient.Browser.Tests.API
{
    [TestFixture]
    public class BrowsersManagerApiTest
    {
        private volatile IUiHelper _uiHelper = null;
        private volatile IBrowserWindowsManager _browserWindowsManager = null;
        private volatile BrowsersManagerApi _browsersManagerApi = null;

        [SetUp]
        public void Init()
        {
            _uiHelper = new TestUiHelper();
            _browserWindowsManager = Substitute.For<IBrowserWindowsManager>();
            _browsersManagerApi = new BrowsersManagerApi(_browserWindowsManager, _uiHelper);
        }

        [Test]
        [STAThread]
        public void TestDisplayApplicationCallGetOrCreateBrowserWindow()
        {
            _uiHelper = Substitute.For<IUiHelper>();
            var windowMock = Substitute.For<IBrowserWindow>();
            _browserWindowsManager.GetOrCreateBrowserWindow(null).ReturnsForAnyArgs(windowMock);
            _browsersManagerApi = new BrowsersManagerApi(_browserWindowsManager, _uiHelper);
            _browsersManagerApi.DisplayApplication(new Application());
            _browserWindowsManager.Received().GetOrCreateBrowserWindow(Arg.Any<IApplication>());
        }

        [Test]
        [STAThread]
        public void TestDisplayApplicationCallsDisplay()
        {
            var windowMock = Substitute.For<IBrowserWindow>();
            _browserWindowsManager.GetOrCreateBrowserWindow(null).ReturnsForAnyArgs(windowMock);
            _browsersManagerApi.DisplayApplication(new Application());
            windowMock.Received().Display();
        }

        [Test]
        [STAThread]
        public void TestCloseApplicationCallsHideOnWindow()
        {
            var windowMock = Substitute.For<IBrowserWindow>();
            _browserWindowsManager.GetBrowserWindow(null).ReturnsForAnyArgs(windowMock);
            _browsersManagerApi.CloseApplication("1");
            windowMock.Received().Hide();
        }

        [Test]
        public void TestUpdateSessionSetsSessionOnWindowsManager()
        {
            var session = new AppDirectSession();
            _browsersManagerApi.UpdateSession(session);
            _browserWindowsManager.Received().Session = session;
        }

        [Test]
        public void TestUpdateApplicationsSetsApplicationsOnWindowsManager()
        {
            var apps = new List<IApplication>();
            _browsersManagerApi.UpdateApplications(apps);
            _browserWindowsManager.Received().Applications = apps;
        }

        [Test]
        public void TestCloseAllApplicationsAndQuitCallsShutdown()
        {
            _uiHelper = Substitute.For<IUiHelper>();
            _browsersManagerApi = new BrowsersManagerApi(_browserWindowsManager, _uiHelper);
            _browsersManagerApi.CloseAllApplicationsAndQuit();
            _uiHelper.Received().GracefulShutdown();
        }

        [Test]
        public void TestGetOpenWindowDatasCallsGetBroswerWindowDatas()
        {
            _browsersManagerApi.GetOpenWindowDatas();
            _browserWindowsManager.Received().GetBrowserWindowDatas();
        }

        [Test]
        [STAThread]
        public void TestDisplayApplicationsSetsWindowState()
        {
            WindowState testWindowState = WindowState.Maximized;
            var windowMock = Substitute.For<IBrowserWindow>();
            _browserWindowsManager.GetOrCreateBrowserWindow(null).ReturnsForAnyArgs(windowMock);
            _browsersManagerApi.DisplayApplications(new List<IApplicationWithState>() { new ApplicationWithState() { Application = new Application(), WindowState = testWindowState } });
            
            Assert.AreEqual(testWindowState, windowMock.WindowState);
        }
    }
}