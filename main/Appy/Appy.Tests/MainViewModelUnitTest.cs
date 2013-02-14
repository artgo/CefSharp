using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Storage;
using AppDirect.WindowsClient.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AppDirect.WindowsClient.Tests
{
    [TestClass]
    public class MainViewModelUnitTest
    {
        private volatile MainViewModel _mainViewModel;

        private readonly FileInfo File = new FileInfo(TestData.FileName);

        private const string Username = TestData.TestUsername;
        private const string Password = TestData.TestPassword;
        private const string BadPassword = "BadPassword";
        private readonly List<Application> _myApplications = new List<Application>() { new Application { IsLocalApp = false, Id = "AppDirectApplicationId", Name = "FakeApp" } };

        private volatile LocalStorage _localStorage;
        private volatile IAppDirectApi _appDirectApiMock;
        private volatile ICachedAppDirectApi _cachedAppDirectApiMock;

        [TestInitialize]
        public void Initialize()
        {
            lock (this)
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
        }

        [TestMethod]
        public void SuggestedApplicationsCollectionIsNotNull()
        {
            lock (this)
            {
                Assert.IsTrue(_mainViewModel.SuggestedApplications.Count > 0);
            }
        }

        [TestMethod]
        public void MyApplicationsCollectionIsPopulated()
        {
            lock (this)
            {
                Assert.IsNotNull(_mainViewModel.MyApplications);
                _cachedAppDirectApiMock.MyApps.Received();
            }
        }

        [TestMethod]
        public void LoginReturnsTrueForValidLogin()
        {
            lock (this)
            {
                Assert.IsTrue(_mainViewModel.Login(Username, Password));
                _cachedAppDirectApiMock.Received().Authenticate(Username, Password);
            }
        }

        [TestMethod]
        public void LoginReturnsFalseForInvalidLogin()
        {
            lock (this)
            {
                Assert.IsFalse(_mainViewModel.Login(Username, BadPassword));
                _cachedAppDirectApiMock.Received().Authenticate(Username, BadPassword);
            }
        }

        [TestMethod]
        public void ValidUsernameIsStored()
        {
            lock (this)
            {
                _mainViewModel.Login(Username, Password);
                Assert.AreEqual(Username, ServiceLocator.LocalStorage.LoginInfo.Username);

                _cachedAppDirectApiMock.Received().Authenticate(Username, Password);
            }
        }

        [TestMethod]
        public void ValidPasswordIsStored()
        {
            lock (this)
            {
                _mainViewModel.Login(Username, Password);
                Assert.AreEqual(Password, ServiceLocator.LocalStorage.LoginInfo.Password);

                _cachedAppDirectApiMock.Received().Authenticate(Username, Password);
            }
        }

        [TestMethod]
        public void PasswordSetDateIsStored()
        {
            lock (this)
            {
                _mainViewModel.Login(Username, Password);
                Assert.AreEqual(DateTime.Now.Date, ServiceLocator.LocalStorage.LoginInfo.PasswordSetDate.Date);

                _cachedAppDirectApiMock.Received().Authenticate(Username, Password);
            }
        }


        [TestMethod]
        public void IncorrectLoginIsNotStored()
        {
            lock (this)
            {
                _mainViewModel.Login(Username, BadPassword);
                Assert.IsNull(ServiceLocator.LocalStorage.LoginInfo);

                _cachedAppDirectApiMock.Received().Authenticate(Username, BadPassword);
            }
        }

        [TestMethod]
        public void LogOutRemovesLoginInfo()
        {
            lock (this)
            {
                _mainViewModel.Login(Username, Password);
                _mainViewModel.Logout();

                Assert.IsNull(ServiceLocator.LocalStorage.LoginInfo);
                _cachedAppDirectApiMock.ReceivedWithAnyArgs().UnAuthenticate();
            }
        }

        [TestMethod]
        public void InstallApplicationLocalIncrementsMyApplication()
        {
            lock (this)
            {
                _mainViewModel.MyApplications.Clear();
                var app = _mainViewModel.SuggestedApplications.First(a => a.IsLocalApp);
                _mainViewModel.Install(app);

                Assert.IsTrue(ServiceLocator.LocalStorage.InstalledLocalApps.Contains(app));
            }
        }

        [TestMethod]
        public void MyAppsContainsCachedAppDirectMyApps()
        {
            lock (this)
            {
                SetMyAppsAndLogin(_myApplications);
                Assert.IsTrue(ServiceLocator.LocalStorage.InstalledAppDirectApps.Contains(_myApplications[0]));
            }
        }

        [TestMethod]
        public void LocalStorageContainsCachedAppDirectMyApps()
        {
            lock (this)
            {
                SetMyAppsAndLogin(_myApplications);
                Assert.IsTrue(_localStorage.InstalledAppDirectApps.Contains(_myApplications[0]));
            }
        }

        [TestMethod]
        public void MyAppsDoesNotContainHiddenMyApps()
        {
            lock (this)
            {
                SetMyAppsAndLogin(_myApplications);

                Assert.IsFalse(_mainViewModel.MyApplications.Contains(_myApplications[0]));
            }
        }

        [TestMethod]
        public void UninstallLocalApplicationIncrementsSuggestedApplications()
        {
            lock (this)
            {
                _mainViewModel.MyApplications.Clear();
                var app = _mainViewModel.SuggestedApplications.First(a => a.IsLocalApp);
                _mainViewModel.Install(app);
                _mainViewModel.Uninstall(app);

                Assert.IsTrue(_localStorage.LastSuggestedApps.Contains(app));
            }
        }

        [TestMethod]
        public void UninstallLocalApplicationDecrementsMyApplications()
        {
            lock (this)
            {
                _mainViewModel.MyApplications.Clear();
                var app = _mainViewModel.SuggestedApplications.First(a => a.IsLocalApp);
                _mainViewModel.Install(app);
                _mainViewModel.Uninstall(app);

                Assert.IsFalse(_mainViewModel.MyApplications.Contains(app));
            }
        }

        [TestMethod]
        public void LocalStorageInitializedByConstructor()
        {
            lock (this)
            {
                Assert.IsNotNull(ServiceLocator.LocalStorage.InstalledLocalApps);
            }
        }

        private void SetMyAppsAndLogin(List<Application> myApps)
        {
            _cachedAppDirectApiMock.MyApps.Returns(myApps);

            _mainViewModel.MyApplications.Clear();
            _mainViewModel.Login(Username, Password);
        }
    }
}
