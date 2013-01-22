using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Models;
using AppDirect.WindowsClient.Storage;
using AppDirect.WindowsClient.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ninject;

namespace AppDirect.WindowsClient.Tests
{
    [TestClass]
    public class MainViewModelUnitTest
    {
        private MainViewModel _mainViewModel;

        private const string FileName = @"\AppDirect\LocalStorage";
        private FileInfo File = new FileInfo(Environment.SpecialFolder.ApplicationData + FileName);

        private const string Username = "appdqa+t75adsa@gmail.com";
        private const string Password = "origo2010";
        private const string BadPassword = "BadPassword";
        private List<Application> _myApplications = new List<Application>(){new Application{IsLocalApp = false, Id = "AppDirectApplicationId", Name = "FakeApp"}}; 

        private LocalStorage _localStorage;

        IAppDirectApi _appDirectApiMock;

        ICachedAppDirectApi _cachedAppDirectApiMock;

        [TestInitialize]
        public void Initialize()
        {
            _appDirectApiMock = Substitute.For<IAppDirectApi>();
            _cachedAppDirectApiMock = Substitute.For<ICachedAppDirectApi>();

            _localStorage = new LocalStorage();

            _cachedAppDirectApiMock.Authenticate(Username, Password).Returns(true);

            IKernel Kernel = new StandardKernel();
            Kernel.Bind<IAppDirectApi>().ToConstant(_appDirectApiMock);
            Kernel.Bind<ICachedAppDirectApi>().ToConstant(_cachedAppDirectApiMock);
            Kernel.Bind<LocalStorage>().ToConstant(_localStorage);

            ServiceLocator.Kernel = Kernel;
            
            File.Delete();
            _mainViewModel = new MainViewModel();
        }

        [TestMethod]
        public void SuggestedApplicationsCollectionIsNotNull()
        {
            Assert.IsTrue(_mainViewModel.SuggestedApplications.Count > 0);
        }

        [TestMethod]
        public void MyApplicationsCollectionIsPopulated()
        {
            Assert.IsNotNull(_mainViewModel.MyApplications);
            _cachedAppDirectApiMock.MyApps.Received();
        }

        [TestMethod]
        public void LoginReturnsTrueForValidLogin()
        {
            Assert.IsTrue(_mainViewModel.Login(Username, Password));
            _cachedAppDirectApiMock.Received().Authenticate(Username, Password);
        }

        [TestMethod]
        public void LoginReturnsFalseForInvalidLogin()
        {
            Assert.IsFalse(_mainViewModel.Login(Username, BadPassword));
            _cachedAppDirectApiMock.Received().Authenticate(Username, BadPassword);
        }

        [TestMethod]
        public void ValidUsernameIsStored()
        {
            _mainViewModel.Login(Username, Password);
            Assert.AreEqual(Username, ServiceLocator.LocalStorage.LoginInfo.Username);

            _cachedAppDirectApiMock.Received().Authenticate(Username, Password);
        }

        [TestMethod]
        public void ValidPasswordIsStored()
        {
            _mainViewModel.Login(Username, Password);
            Assert.AreEqual(Password, ServiceLocator.LocalStorage.LoginInfo.Password);

            _cachedAppDirectApiMock.Received().Authenticate(Username, Password);
        }

        [TestMethod]
        public void PasswordSetDateIsStored()
        {
            _mainViewModel.Login(Username, Password);
            Assert.AreEqual(DateTime.Now.Date, ServiceLocator.LocalStorage.LoginInfo.PasswordSetDate.Date);

            _cachedAppDirectApiMock.Received().Authenticate(Username, Password);
        }


        [TestMethod]
        public void IncorrectLoginIsNotStored()
        {
            _mainViewModel.Login(Username, BadPassword);
            Assert.IsNull(ServiceLocator.LocalStorage.LoginInfo);

            _cachedAppDirectApiMock.Received().Authenticate(Username, BadPassword);
        }

        [TestMethod]
        public void LogOutRemovesLoginInfo()
        {
            _mainViewModel.Login(Username, Password);
            _mainViewModel.Logout();

            Assert.IsNull(ServiceLocator.LocalStorage.LoginInfo);
            _cachedAppDirectApiMock.ReceivedWithAnyArgs().UnAuthenticate();
        }

        [TestMethod]
        public void InstallApplicationLocalIncrementsMyApplication()
        {
            _mainViewModel.MyApplications.Clear();
            var app = _mainViewModel.SuggestedApplications.First(a => a.IsLocalApp);
            _mainViewModel.Install(app);

            Assert.IsTrue(_mainViewModel.MyApplications.Contains(app));
        }

        [TestMethod]
        public void InstallApplicationLocalDecrementsSuggested()
        {
            _mainViewModel.MyApplications.Clear();
            var app = _mainViewModel.SuggestedApplications.First(a => a.IsLocalApp);
            _mainViewModel.Install(app);

            Assert.IsFalse(_mainViewModel.SuggestedApplications.Contains(app));
        }

        [TestMethod]
        public void MyAppsContainsCachedAppDirectMyApps()
        {
            SetMyAppsList(_myApplications);
            _mainViewModel.MyApplications.Clear();

            _mainViewModel.Login(Username, Password);

            _mainViewModel.RefreshAppsLists();

            Assert.IsTrue(_mainViewModel.MyApplications.Contains(_myApplications[0]));
        }

        [TestMethod]
        public void LocalStorageContainsCachedAppDirectMyApps()
        {
            SetMyAppsList(_myApplications);
            _mainViewModel.MyApplications.Clear();
            _mainViewModel.Login(Username, Password);
            _mainViewModel.RefreshAppsLists();

            Assert.IsTrue(_localStorage.InstalledApiApps.Contains(_myApplications[0]));
        }

        [TestMethod]
        public void MyAppsDoesNotContainHiddenMyApps()
        {
            SetMyAppsList(_myApplications);
            _mainViewModel.MyApplications.Clear();
            _mainViewModel.Login(Username, Password);
            _mainViewModel.RefreshAppsLists();

            Assert.IsFalse(_mainViewModel.MyApplications.Contains(_myApplications[0]));
        }

        [TestMethod]
        public void UninstallLocalApplicationIncrementsSuggestedApplications()
        {
            _mainViewModel.MyApplications.Clear();
            var app = _mainViewModel.SuggestedApplications.First(a => a.IsLocalApp);
            _mainViewModel.Install(app);
            _mainViewModel.Uninstall(app);

            Assert.IsTrue(_mainViewModel.SuggestedApplications.Contains(app));
        }

        [TestMethod]
        public void UninstallLocalApplicationDecrementsMyApplications()
        {
            _mainViewModel.MyApplications.Clear();
            var app = _mainViewModel.SuggestedApplications.First(a => a.IsLocalApp);
            _mainViewModel.Install(app);
            _mainViewModel.Uninstall(app);

            Assert.IsFalse(_mainViewModel.MyApplications.Contains(app));
        }
        
        [TestMethod]
        public void LocalStorageInitializedByConstructor()
        {
            Assert.IsNotNull(ServiceLocator.LocalStorage.InstalledLocalApps);
        }

        private void SetMyAppsList(List<Application> myApps)
        {
            _cachedAppDirectApiMock.MyApps.Returns(myApps);
        }
    }
}
