using System;
using System.Windows.Controls;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Storage;
using NSubstitute;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.UnitTests
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
            kernel.Rebind<ICachedAppDirectApi>().ToConstant(cachedAppDirectApiMock);
            kernel.Rebind<LocalStorage>().ToConstant(localStorage);
        }

        [Test]
        public void PerformInUiThreadDoesPerformAnAction()
        {
            var test = false;
            Helper.PerformInUiThread(() => { test = true; });
            Assert.IsTrue(test);
        }

        [Test]
        public void PerformForMinimumTimeDoesPerformAnAction()
        {
            var test = false;
            Helper.PerformForMinimumTime(() => { test = true; }, false, 0);
            Assert.IsTrue(test);
        }


        //Should be moved to Integration Tests
        [Test]
        public void PerformForMinimumTimeDoesNotReturnBeforeTimeIsElapsed()
        {
            var millisecondsToSleep = 3000;

            var start = Environment.TickCount;
            var test = false;
            Helper.PerformForMinimumTime(() => { test = true; }, false, millisecondsToSleep);

            var stop = Environment.TickCount;

            var elapsedTime = stop - start;

            Assert.IsTrue(elapsedTime > millisecondsToSleep);
        }
    }
}
