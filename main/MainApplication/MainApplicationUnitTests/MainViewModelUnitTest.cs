using System;
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

        private const string UserName = "appdqa+t75adsa@gmail.com";
        private const string Password = "origo2010";

        [TestInitialize]
        public void Initialize()
        {
            var appDirectApiMock = Substitute.For<IAppDirectApi>();
            var cachedAppDirectApiMock = Substitute.For<ICachedAppDirectApi>();

            cachedAppDirectApiMock.Authenticate(UserName, Password).Returns(true);

            IKernel Kernel = new StandardKernel();
            Kernel.Bind<IAppDirectApi>().ToConstant(appDirectApiMock);
            Kernel.Bind<ICachedAppDirectApi>().ToConstant(cachedAppDirectApiMock);

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
        }

        [TestMethod]
        public void ValidLoginIsStored()
        {
            LoginObject loginObject = new LoginObject {Password = Password, UserName = UserName};
            
            Assert.IsTrue(_mainViewModel.Login(loginObject.UserName, loginObject.Password));

            Assert.AreEqual(loginObject.UserName, LocalStorage.Instance.LoginInfo.UserName);
            Assert.AreEqual(loginObject.Password, LocalStorage.Instance.LoginInfo.Password);
        }

        [TestMethod]
        public void IncorrectLoginIsNotStored()
        {
            LocalStorage.Instance.ForceReloadFromFile();

            LoginObject loginObject = new LoginObject { Password = "WrongPassword", UserName = UserName };

            Assert.IsFalse(_mainViewModel.Login(loginObject.UserName, loginObject.Password));

            Assert.IsNull(LocalStorage.Instance.LoginInfo);
        }

        [TestMethod]
        public void LogOutRemovesLoginInfo()
        {

            LoginObject loginObject = new LoginObject { Password = Password, UserName = UserName };

            Assert.IsTrue(_mainViewModel.Login(loginObject.UserName, loginObject.Password));
            Assert.IsNotNull(LocalStorage.Instance.LoginInfo);

            _mainViewModel.Logout();

            Assert.IsNull(LocalStorage.Instance.LoginInfo); ;
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
            Assert.IsNotNull(LocalStorage.Instance.InstalledLocalApps);
            Assert.IsNotNull(LocalStorage.Instance.SuggestedLocalApps);
        }
    }
}
