using System;
using System.Windows.Controls;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Storage;
using AppDirect.WindowsClient.UI;
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
        public void GetClickedApplicationViewModelFromContextMenuReturnsApplicationViewModel()
        {
            MenuItem testMenuItem = new MenuItem();
            var testAppViewModel = new ApplicationViewModel(LocalApplications.AppStoreApp);
            testMenuItem.DataContext = testAppViewModel;
            Assert.AreEqual(testAppViewModel, Helper.GetApplicationViewModelFromContextMenuClick(testMenuItem));
        }

        [Test]
        [STAThread]
        public void GetClickedApplicationViewModelFromContextMenuReturnsAppViewModel()
        {
            MenuItem testMenuItem = new MenuItem();
            var testApp = LocalApplications.AppStoreApp;
            testMenuItem.DataContext = new ApplicationViewModel(testApp);

            Assert.AreEqual(testApp, Helper.GetApplicationViewModelFromContextMenuClick(testMenuItem).Application);
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

        [Test]
        public void PerformWhenIdlePerformsAction()
        {
            var test = false;
            Helper.PerformWhenIdle(() => { test = true; }, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromHours(1));
            Assert.IsTrue(test);
        }

        [Test]
        public void PerformWhenIdleTimeoutHonored()
        {
            var test = false;
            Helper.PerformWhenIdle(() => { test = true; }, TimeSpan.FromMinutes(1), TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(2));
            Assert.IsFalse(test);
        }
        
        //Should be moved to Integration Tests
        [Test]
        public void PerformForMinimumTimeDoesNotReturnBeforeTimeIsElapsed()
        {
            var millisecondsToSleep = 300;

            var start = Environment.TickCount;
            var test = false;
            Helper.PerformForMinimumTime(() => { test = true; }, false, millisecondsToSleep);

            var stop = Environment.TickCount;

            var elapsedTime = stop - start;

            Assert.IsTrue(elapsedTime >= millisecondsToSleep);
        }
    }
}
