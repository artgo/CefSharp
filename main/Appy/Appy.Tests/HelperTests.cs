using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Storage;
using NSubstitute;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests
{
    [TestFixture]
    public class HelperTests
    {
        [Test]
        [STAThread]
        public void GetClickedAppFromContextMenuReturnsApp()
        {
            MenuItem testMenuItem = new MenuItem();
            var testApp = LocalApplications.AppStoreApp;
            testMenuItem.DataContext = testApp;

            Assert.AreEqual(testApp, Helper.GetClickedAppFromContextMenuClick(testMenuItem));
        }

        [Test]
        public void AuthenticateReturnsTrueForValidCredentials()
        {
            var username = "Username";
            var password = "Password";

            var localStorage = new LocalStorage();
            localStorage.SetCredentials(username, password);

            var cachedAppDirectApiMock = Substitute.For<ICachedAppDirectApi>();
            cachedAppDirectApiMock.Authenticate(username, password).Returns(true);

            var kernel = ServiceLocator.Kernel;
            kernel.Bind<ICachedAppDirectApi>().ToConstant(cachedAppDirectApiMock);
            kernel.Bind<LocalStorage>().ToConstant(localStorage);

            Assert.IsTrue(Helper.Authenticate());
        }


        [Test]
        public void AuthenticateReturnsFalseForInvalidCredentials()
        {
            var username = "Username";
            var password = "Password";
            
            var localStorage = new LocalStorage();
            localStorage.SetCredentials(username, password);

            var cachedAppDirectApiMock = Substitute.For<ICachedAppDirectApi>();
            cachedAppDirectApiMock.Authenticate(username, password).Returns(false);

            var kernel = ServiceLocator.Kernel;
            kernel.Bind<ICachedAppDirectApi>().ToConstant(cachedAppDirectApiMock);
            kernel.Bind<LocalStorage>().ToConstant(localStorage);

            Assert.IsFalse(Helper.Authenticate());
        }
    }
}
