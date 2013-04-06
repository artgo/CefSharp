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
            var browserMock = Substitute.For<IBrowserWindowsCommunicator>();
            ServiceLocator.Kernel.Rebind<IBrowserWindowsCommunicator>().ToConstant(browserMock);

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

            var browserMock = Substitute.For<IBrowserWindowsCommunicator>();
         
            var kernel = ServiceLocator.Kernel;
            kernel.Rebind<ICachedAppDirectApi>().ToConstant(cachedAppDirectApiMock);
            kernel.Rebind<LocalStorage>().ToConstant(localStorage);
            kernel.Rebind<IBrowserWindowsCommunicator>().ToConstant(browserMock);
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

        [Test]
        public void LoginReturnsTrueForValidLogin()
        {
            ServiceLocator.LocalStorage.SetCredentials(Username, Password);
            Assert.IsTrue(Helper.Authenticate());
            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, Password);
        }

        [Test]
        public void LoginReturnsFalseForInvalidLogin()
        {
            ServiceLocator.LocalStorage.SetCredentials(Username, PasswordBad);
            Assert.IsFalse(Helper.Authenticate());
            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, PasswordBad);
        }

        [Test]
        public void ValidUsernameIsStored()
        {
            ServiceLocator.LocalStorage.SetCredentials(Username, Password);
            Helper.Authenticate();
            Assert.AreEqual(Username, ServiceLocator.LocalStorage.LoginInfo.Username);

            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, Password);
        }

        [Test]
        public void ValidPasswordIsStored()
        {
            ServiceLocator.LocalStorage.SetCredentials(Username, Password);
            Helper.Authenticate();
            Assert.AreEqual(Password, ServiceLocator.LocalStorage.LoginInfo.Password);

            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, Password);
        }

        [Test]
        public void PasswordSetDateIsStored()
        {
            ServiceLocator.LocalStorage.SetCredentials(Username, Password);
            Helper.Authenticate();
            Assert.AreEqual(DateTime.Now.Date, ServiceLocator.LocalStorage.LoginInfo.PasswordSetDate.Date);

            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, Password);
        }

        [Test]
        public void IncorrectLoginIsNotStored()
        {
            ServiceLocator.LocalStorage.SetCredentials(Username, PasswordBad);

            Helper.Authenticate();
            Assert.IsNull(ServiceLocator.LocalStorage.LoginInfo);

            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, PasswordBad);
        }
        
        //Should be moved to Integration Tests
        [Test]
        public void PerformForMinimumTimeDoesNotReturnBeforeTimeIsElapsed()
        {
            //var millisecondsToSleep = 300;

            //var start = Environment.TickCount;
            //var test = false;
            //Helper.PerformForMinimumTime(() => { test = true; }, false, millisecondsToSleep);

            //var stop = Environment.TickCount;

            //var elapsedTime = stop - start;

            //Assert.IsTrue(elapsedTime >= millisecondsToSleep);
        }
    }
}
