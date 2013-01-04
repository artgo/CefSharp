using System;
using System.IO;
using System.Linq;
using AppDirect.WindowsClient.Models;
using AppDirect.WindowsClient.Storage;
using AppDirect.WindowsClient.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppDirect.WindowsClient.Tests
{
    [TestClass]
    public class MainViewModelUnitTest
    {
        private MainViewModel _mainViewModel;

        private const string FileName = @"\AppDirect\LocalStorage";
        private FileInfo File = new FileInfo(Environment.SpecialFolder.ApplicationData + FileName);

        [TestInitialize]
        public void TestInitialize()
        {
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
        public void LoginIsStored()
        {
            LoginObject loginObject = new LoginObject {Password = "PasswordTest", UserName = "UsernameTest"};

            _mainViewModel.Login(loginObject);

            Assert.AreEqual(loginObject.UserName, _mainViewModel.LoginInfo.UserName);
            Assert.AreEqual(loginObject.Password, _mainViewModel.LoginInfo.Password);
            Assert.IsNotNull(_mainViewModel.LoginInfo.AuthToken);
        }

        [TestMethod]
        public void LogOutRemovesLoginInfo()
        {
            LoginObject loginObject = new LoginObject { Password = "PasswordTest", UserName = "UsernameTest" };
            _mainViewModel.Login(loginObject);

            Assert.IsNotNull(_mainViewModel.LoginInfo.AuthToken);
            _mainViewModel.Logout();

            Assert.IsNull(_mainViewModel.LoginInfo.AuthToken);
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
            var app = _mainViewModel.MyApplications.First(a => a.IsLocalApp);
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
