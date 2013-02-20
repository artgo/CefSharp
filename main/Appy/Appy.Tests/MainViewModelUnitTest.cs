using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Storage;
using AppDirect.WindowsClient.UI;
using NUnit.Framework;
using NSubstitute;

namespace AppDirect.WindowsClient.Tests
{
    [TestFixture]
    public class MainViewModelUnitTest
    {
        private volatile MainViewModel _mainViewModel;

        private readonly FileInfo File = new FileInfo(TestData.FileName);

        private const string Username = TestData.TestUsername;
        private const string Password = TestData.TestPassword;
        private const string BadPassword = "BadPassword";

        private readonly List<Application> _myApplications = new List<Application>()
            {
                new Application {IsLocalApp = false, Id = "AppDirectApplicationId", Name = "FakeApp"},
                new Application {IsLocalApp = false, Id = "AppDirectApplicationId2", Name = "FakeApp2"},
                new Application {IsLocalApp = false, Id = "AppDirectApplicationId3", Name = "FakeApp3"}
            };

        private volatile LocalStorage _localStorage;
        private volatile IAppDirectApi _appDirectApiMock;
        private volatile ICachedAppDirectApi _cachedAppDirectApiMock;

        [TestFixtureSetUp]
        public void Initialize()
        {
            _appDirectApiMock = Substitute.For<IAppDirectApi>();
            _cachedAppDirectApiMock = Substitute.For<ICachedAppDirectApi>();

            _localStorage = new LocalStorage();

            _cachedAppDirectApiMock.Authenticate(Username, Password).Returns(true);
            
            var kernel = ServiceLocator.Kernel;
            kernel.Bind<IAppDirectApi>().ToConstant(_appDirectApiMock);
            kernel.Bind<ICachedAppDirectApi>().ToConstant(_cachedAppDirectApiMock);
            kernel.Bind<LocalStorage>().ToConstant(_localStorage);

            _mainViewModel = new MainViewModel();
        }

        [Test]
        public void InstallAppMarksAppPinned()
        {
            _mainViewModel.MyApplications.Clear();
            var app = _mainViewModel.SuggestedApplications.First(a => a.IsLocalApp);
            _mainViewModel.Install(app);

            Assert.IsTrue(app.PinnedToTaskbar);
        }

        [Test]
        public void SuggestedApplicationsCollectionIsNotNull()
        {
            Assert.IsTrue(_mainViewModel.SuggestedApplications.Count > 0);
        }

        [Test]
        public void MyApplicationsCollectionIsPopulated()
        {
            Assert.IsTrue(_mainViewModel.MyApplications.Count > 0);
        }

        [Test]
        public void LoginReturnsTrueForValidLogin()
        {
            Assert.IsTrue(_mainViewModel.Login(Username, Password));
            _cachedAppDirectApiMock.Received().Authenticate(Username, Password);
        }

        [Test]
        public void LoginReturnsFalseForInvalidLogin()
        {
            Assert.IsFalse(_mainViewModel.Login(Username, BadPassword));
            _cachedAppDirectApiMock.Received().Authenticate(Username, BadPassword);
        }

        [Test]
        public void ValidUsernameIsStored()
        {
            _mainViewModel.Login(Username, Password);
            Assert.AreEqual(Username, ServiceLocator.LocalStorage.LoginInfo.Username);

            _cachedAppDirectApiMock.Received().Authenticate(Username, Password);
        }

        [Test]
        public void ValidPasswordIsStored()
        {
            _mainViewModel.Login(Username, Password);
            Assert.AreEqual(Password, ServiceLocator.LocalStorage.LoginInfo.Password);

            _cachedAppDirectApiMock.Received().Authenticate(Username, Password);
        }

        [Test]
        public void PasswordSetDateIsStored()
        {
            _mainViewModel.Login(Username, Password);
            Assert.AreEqual(DateTime.Now.Date, ServiceLocator.LocalStorage.LoginInfo.PasswordSetDate.Date);

            _cachedAppDirectApiMock.Received().Authenticate(Username, Password);
        }


        [Test]
        public void IncorrectLoginIsNotStored()
        {
            _mainViewModel.Login(Username, BadPassword);
            Assert.IsNull(ServiceLocator.LocalStorage.LoginInfo);

            _cachedAppDirectApiMock.Received().Authenticate(Username, BadPassword);
        }


        [Test]
        public void LogOutRemovesLoginInfo()
        {
            _mainViewModel.Login(Username, Password);
            _mainViewModel.Logout();

            Assert.IsNull(ServiceLocator.LocalStorage.LoginInfo);
            _cachedAppDirectApiMock.ReceivedWithAnyArgs().UnAuthenticate();
        }

        [Test]
        public void LogOutRemovesApiApps()
        {
            SetMyAppsAndLogin(_myApplications);
            _mainViewModel.Logout();

            Assert.IsEmpty(ServiceLocator.LocalStorage.InstalledAppDirectApps);
        }

        [Test]
        public void InstallApplicationLocalIncrementsMyApplication()
        {
            _mainViewModel.MyApplications.Clear();
            var app = _mainViewModel.SuggestedApplications.First(a => a.IsLocalApp);
            _mainViewModel.Install(app);

            Assert.IsTrue(ServiceLocator.LocalStorage.InstalledLocalApps.Contains(app));
        }

        [Test]
        public void MyAppsContainsCachedAppDirectMyApps()
        {
            SetMyAppsAndLogin(_myApplications);
            Assert.IsTrue(ServiceLocator.LocalStorage.InstalledAppDirectApps.Contains(_myApplications[0]));
        }

        [Test]
        public void MyAppsDoesNotContainHiddenMyApps()
        {
            SetMyAppsAndLogin(_myApplications);

            Assert.IsFalse(_mainViewModel.MyApplications.Contains(_myApplications[0]));
        }

        [Test]
        public void UninstallLocalApplicationIncrementsSuggestedApplications()
        {
            _mainViewModel.MyApplications.Clear();
            var app = _mainViewModel.SuggestedApplications.First(a => a.IsLocalApp);
            _mainViewModel.Install(app);
            _mainViewModel.Uninstall(app);

            Assert.IsTrue(_localStorage.LastSuggestedApps.Contains(app));
        }

        [Test]
        public void UninstallLocalApplicationDecrementsMyApplications()
        {
            _mainViewModel.MyApplications.Clear();
            var app = _mainViewModel.SuggestedApplications.First(a => a.IsLocalApp);
            _mainViewModel.Install(app);
            _mainViewModel.Uninstall(app);

            Assert.IsFalse(_mainViewModel.MyApplications.Contains(app));
        }

        [Test]
        public void LocalStorageInitializedByConstructor()
        {
            Assert.IsNotNull(ServiceLocator.LocalStorage.InstalledLocalApps);
        }

        private void SetMyAppsAndLogin(List<Application> myApps)
        {
            _cachedAppDirectApiMock.MyApps.Returns(myApps);

            _mainViewModel.MyApplications.Clear();
            _mainViewModel.Login(Username, Password);
        }
    }
}
