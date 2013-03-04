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
        private const string Username = "Username";
        private const string Password = "Password";
        private const string UsernameBad = "UsernameBad";
        private const string PasswordBad = "PasswordBad";

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
            ServiceLocator.LocalStorage.SetCredentials(Username, Password);
            Assert.IsTrue(Helper.Authenticate());
        }

        [Test]
        public void AuthenticateReturnsFalseForInvalidCredentials()
        {
            ServiceLocator.LocalStorage.SetCredentials(UsernameBad, PasswordBad);
            Assert.IsFalse(Helper.Authenticate());
        }

        [TestFixtureSetUp]
        public void InitializeAuthenticationTest()
        {
            var localStorage = new LocalStorage();

            var cachedAppDirectApiMock = Substitute.For<ICachedAppDirectApi>();
            cachedAppDirectApiMock.Authenticate(Username, Password).Returns(true);
            cachedAppDirectApiMock.Authenticate(UsernameBad, PasswordBad).Returns(false);

            var kernel = ServiceLocator.Kernel;
            kernel.Bind<ICachedAppDirectApi>().ToConstant(cachedAppDirectApiMock);
            kernel.Bind<LocalStorage>().ToConstant(localStorage);
        }

        [Test]
        public void PerformInUiThreadDoesPerformAnAction()
        {
            var test = false;
            Helper.PerformInUiThread(() => { test = true; });
            Assert.IsTrue(test);
        }
    }
}
