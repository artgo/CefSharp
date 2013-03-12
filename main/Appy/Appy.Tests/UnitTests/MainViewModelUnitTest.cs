using System;
using System.Collections.Generic;
using System.Linq;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Storage;
using AppDirect.WindowsClient.UI;
using AppDirect.WindowsClient.Updates;
using NUnit.Framework;
using NSubstitute;

namespace AppDirect.WindowsClient.Tests.UnitTests
{
    [TestFixture]
    public class MainViewModelUnitTest
    {
        private volatile MainViewModel _mainViewModel;
        
        private const string Username = TestData.TestUsername;
        private const string Password = TestData.TestPassword;
        private const string BadPassword = "BadPassword";

        private readonly List<Application> _myApplications = new List<Application>()
            {
                new Application {IsLocalApp = false, Id = "AppDirectApplicationId", Name = "FakeApp"},
                new Application {IsLocalApp = false, Id = "AppDirectApplicationId2", Name = "FakeApp2"},
                new Application {IsLocalApp = false, Id = "AppDirectApplicationId3", Name = "FakeApp3"}
            };
        
        [TestFixtureSetUp]
        public void SetUpForTests()
        {
            var cachedAppDirectApiMock = Substitute.For<ICachedAppDirectApi>();

            var localStorage = new LocalStorage();

            cachedAppDirectApiMock.Authenticate(Username, Password).Returns(true);

            var kernel = ServiceLocator.Kernel;
            kernel.Rebind<ICachedAppDirectApi>().ToConstant(cachedAppDirectApiMock);
            kernel.Rebind<LocalStorage>().ToConstant(localStorage);

        }
        
        private void InitializeTests()
        {
            ServiceLocator.LocalStorage.ClearAllStoredData();
            _mainViewModel = new MainViewModel();
            _mainViewModel.InitializeAppsLists();
        }
        
        #region Constructor Tests

        [Test]
        public void SuggestedApplicationsContainsAllLocalApps()
        {
            InitializeTests();
            Assert.AreEqual(LocalApplications.LocalApplicationsList, _mainViewModel.SuggestedApplications.Select(a => a.Application));
        }

        [Test]
        public void MyApplicationsCollectionContainsAppStoreApp()
        {
            InitializeTests();
            Assert.AreEqual(LocalApplications.AppStoreApp, _mainViewModel.MyApplications[0].Application);
        }

        #endregion
        #region Install Test

        [Test]
        public void InstallAppMarksAppPinned()
        {
            var app = CallInstallApp();
            Assert.IsTrue(app.Application.PinnedToTaskbar);
        }

        [Test]
        public void InstallApplicationAddsAppToMyApps()
        {
            var app = CallInstallApp();
            Assert.IsTrue(_mainViewModel.MyApplications.Contains(app));
        }

        [Test]
        public void InstallApplicationAddsAppToStoredInstalledList()
        {
            var app = CallInstallApp();
            Assert.IsTrue(ServiceLocator.LocalStorage.InstalledLocalApps.Contains(app.Application));
        }

        [Test]
        public void InstallApplicationRemovesAppFromSuggestedApps()
        {
            var app = CallInstallApp();
            Assert.IsFalse(_mainViewModel.SuggestedApplications.Contains(app));
        }

        [Test]
        public void InstallApplicationRemovesAppFromStoredSuggestedApps()
        {
            var app = CallInstallApp();
            Assert.IsFalse(ServiceLocator.LocalStorage.LastSuggestedApps.Contains(app.Application));
        }

        #endregion
        #region Uninstall Tests

        [Test]
        public void UninstallLocalApplicationAddsAppToSuggestedApplications()
        {
            var app = CallUninstallApp();
            Assert.IsTrue(_mainViewModel.SuggestedApplications.Contains(app));
        }

        [Test]
        public void UninstallLocalApplicationAddsAppToStoredSuggestedApplications()
        {
            var app = CallUninstallApp();
            Assert.IsTrue(ServiceLocator.LocalStorage.LastSuggestedApps.Contains(app.Application));
        }

        [Test]
        public void UninstallLocalApplicationRemovesAppFromMyApplications()
        {
            var app = CallUninstallApp();
            Assert.IsFalse(_mainViewModel.MyApplications.Contains(app));
        }

        [Test]
        public void UninstallLocalApplicationRemovesAppFromStoredInstalledApplications()
        {
            var app = CallUninstallApp();
            Assert.IsFalse(ServiceLocator.LocalStorage.InstalledLocalApps.Contains(app.Application));
        }

        #endregion
        #region Login Tests

        [Test]
        public void LoginReturnsTrueForValidLogin()
        {
            InitializeTests();
            Assert.IsTrue(_mainViewModel.Login(Username, Password));
            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, Password);
        }

        [Test]
        public void LoginReturnsFalseForInvalidLogin()
        {
            InitializeTests();
            Assert.IsFalse(_mainViewModel.Login(Username, BadPassword));
            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, BadPassword);
        }

        [Test]
        public void ValidUsernameIsStored()
        {
            InitializeTests();
            _mainViewModel.Login(Username, Password);
            Assert.AreEqual(Username, ServiceLocator.LocalStorage.LoginInfo.Username);

            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, Password);
        }

        [Test]
        public void ValidPasswordIsStored()
        {
            InitializeTests();
            _mainViewModel.Login(Username, Password);
            Assert.AreEqual(Password, ServiceLocator.LocalStorage.LoginInfo.Password);

            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, Password);
        }

        [Test]
        public void PasswordSetDateIsStored()
        {
            InitializeTests();
            _mainViewModel.Login(Username, Password);
            Assert.AreEqual(DateTime.Now.Date, ServiceLocator.LocalStorage.LoginInfo.PasswordSetDate.Date);

            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, Password);
        }

        [Test]
        public void IncorrectLoginIsNotStored()
        {
            InitializeTests();
            _mainViewModel.Login(Username, BadPassword);
            Assert.IsNull(ServiceLocator.LocalStorage.LoginInfo);

            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, BadPassword);
        }

        [Test]
        public void MyAppsContainsCachedAppDirectMyApps()
        {
            InitializeTests();
            SetMyAppsAndLogin(_myApplications);
            Assert.IsTrue(ServiceLocator.LocalStorage.InstalledAppDirectApps.Contains(_myApplications[0]));
        }

        #endregion
        #region Log Out Tests

        [Test]
        public void LogOutRemovesLoginInfo()
        {
            InitializeTests();
            _mainViewModel.Login(Username, Password);
            _mainViewModel.Logout();

            Assert.IsNull(ServiceLocator.LocalStorage.LoginInfo);
            ServiceLocator.CachedAppDirectApi.ReceivedWithAnyArgs().UnAuthenticate();
        }

        [Test]
        public void LogOutRemovesApiApps()
        {
            InitializeTests();
            SetMyAppsAndLogin(_myApplications);
            _mainViewModel.Logout();

            Assert.IsEmpty(ServiceLocator.LocalStorage.InstalledAppDirectApps);
        }

        #endregion
        #region SyncAppsWithApi Tests

        [Test]
        public void SyncAppsWithApiAddsMissingApiApps()
        {
            SetMyAppsAndLogin(_myApplications);
            var apiApp = _mainViewModel.MyApplications.First(a => !a.Application.IsLocalApp);
            _mainViewModel.MyApplications.Remove(apiApp);
            _mainViewModel.SyncAppsWithApi();

            Assert.IsNotNull(_mainViewModel.MyApplications.FirstOrDefault(a => Equals(a.Application, apiApp.Application)));
        }

        [Test]
        public void SyncAppsWithApiRemovesExpiredApiApps()
        {
            SetMyAppsAndLogin(_myApplications);

            var expiredApp = _myApplications[0];
            _myApplications.Remove(expiredApp);

            SetMyAppsAndLogin(_myApplications);

            _mainViewModel.SyncAppsWithApi();

            Assert.IsNull(_mainViewModel.MyApplications.FirstOrDefault(a => a.Application == expiredApp));
        }

        #endregion
        #region Checking For Updates Tests

        [Test]
        public void ResetUpdateTextUpdateAvailableTest()
        {
            ServiceLocator.LocalStorage.UpdateDownloaded = true;

            _mainViewModel = new MainViewModel();
            _mainViewModel.ResetUpdateText();

            Assert.AreEqual("Install updates now", _mainViewModel.UpdateString);
        }

        [Test]
        public void ResetUpdateTextNoUpdateTest()
        {
            ServiceLocator.LocalStorage.UpdateDownloaded = false;

            _mainViewModel = new MainViewModel();
            _mainViewModel.ResetUpdateText();
            Assert.AreEqual("Check for updates", _mainViewModel.UpdateString);
        }

        [Test]
        public void UpdateClickInstallsAvailableUpdateTest()
        {
            ServiceLocator.LocalStorage.UpdateDownloaded = true;
            var mockUpdater = Substitute.For<Updater>();

            ServiceLocator.Kernel.Rebind<Updater>().ToConstant(mockUpdater);

            _mainViewModel = new MainViewModel();
            _mainViewModel.UpdateClick();

            mockUpdater.Received().InstallUpdates();
        }
        
        #endregion
        
        private void SetMyAppsAndLogin(List<Application> myApps)
        {
            InitializeTests();
            ServiceLocator.CachedAppDirectApi.MyApps.Returns(myApps);
            _mainViewModel.Login(Username, Password);
        }

        private ApplicationViewModel CallInstallApp()
        {
            InitializeTests();
            var app = _mainViewModel.SuggestedApplications.First(a => a.Application.IsLocalApp);
            _mainViewModel.Install(app);
            return app;
        }

        private ApplicationViewModel CallUninstallApp()
        {
            InitializeTests();
            var app = _mainViewModel.SuggestedApplications.First(a => a.Application.IsLocalApp);
            _mainViewModel.Install(app);
            _mainViewModel.Uninstall(app);
            return app;
        }
    }
}
