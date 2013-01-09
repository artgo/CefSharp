using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        LocalStorage _localStorage;

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
        public void ValidLoginIsStored()
        {
            Assert.IsTrue(_mainViewModel.Login(Username, Password));

            Assert.AreEqual(Username, ServiceLocator.LocalStorage.LoginInfo.Username);
            Assert.AreEqual(Password, ServiceLocator.LocalStorage.LoginInfo.Password);

            _cachedAppDirectApiMock.Received().Authenticate(Username, Password);
        }

        [TestMethod]
        public void IncorrectLoginIsNotStored()
        {
            Assert.IsFalse(_mainViewModel.Login(Username, "BadPassword"));
            Assert.IsNull(ServiceLocator.LocalStorage.LoginInfo);

            _cachedAppDirectApiMock.ReceivedWithAnyArgs().Authenticate("", "");
        }

        [TestMethod]
        public void LogOutRemovesLoginInfo()
        {
            Assert.IsTrue(_mainViewModel.Login(Username, Password));
            Assert.IsNotNull(ServiceLocator.LocalStorage.LoginInfo);

            _mainViewModel.Logout();

            Assert.IsNull(ServiceLocator.LocalStorage.LoginInfo);
            _cachedAppDirectApiMock.ReceivedWithAnyArgs().UnAuthenticate();
        }

        [TestMethod]
        public void InstallApplicationIncrementsMyApplicationDecrementsSuggested()
        {
            _mainViewModel.MyApplications.Clear();

            var app = _mainViewModel.SuggestedApplications.First(a => a.IsLocalApp);

            _mainViewModel.Install(app);

            Assert.IsTrue(_mainViewModel.MyApplications.Contains(app));
            Assert.IsFalse(_mainViewModel.SuggestedApplications.Contains(app));
        }

        [TestMethod]
        public void UninstallLocalApplicationIncrementsSuggestedApplicationsDecrementsMyApplications()
        {
            _mainViewModel.MyApplications.Clear();
            var app = _mainViewModel.SuggestedApplications.First(a => a.IsLocalApp);
            _mainViewModel.Install(app);

            Assert.IsTrue(_mainViewModel.MyApplications.Contains(app));
            _mainViewModel.Uninstall(app);

            Assert.IsTrue(_mainViewModel.SuggestedApplications.Contains(app));
            Assert.IsFalse(_mainViewModel.MyApplications.Contains(app));
        }
        
        [TestMethod]
        public void LocalStorageInitializedByConstructor()
        {
            Assert.IsNotNull(ServiceLocator.LocalStorage.InstalledLocalApps);
        }
    }
}
