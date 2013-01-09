using System;
using System.IO;
using AppDirect.WindowsClient.Models;
using AppDirect.WindowsClient.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppDirect.WindowsClient.Tests
{
    [TestClass]
    public class LocalStorageUnitTest
    {
        private const string FileName = @"\AppDirect\LocalStorage";
        private FileInfo File = new FileInfo(Environment.SpecialFolder.ApplicationData + FileName);

        private LocalStorage localStorage;

        [TestInitialize]
        public void Setup()
        {
            File.Delete();
            localStorage = new LocalStorage();
        }

        [TestMethod]
        public void InstalledAppsListNullWithoutFile()
        {
            Assert.IsNull(localStorage.InstalledLocalApps);
        }

        [TestMethod]
        public void LocalStorageListsNotNullWhenFileContainsApps()
        {
            localStorage.InstalledLocalApps = LocalApplications.GetLocalApplications();
            localStorage.SaveAppSettings();

            File.Refresh();
            Assert.IsTrue(File.Exists);

            localStorage = LocalStorage.LoadLocalStorage();

            Assert.IsNotNull(localStorage.InstalledLocalApps);
        }

        [TestMethod]
        public void SaveLocalStorageCreatesStorageFile()
        {
            localStorage.SaveAppSettings();

            File.Refresh();
            Assert.IsTrue(File.Exists);
        }

        [TestMethod]
        public void StoredCredentialsRestoredToOriginalValues()
        {
            string unencryptedPassword = "IamPassWordValue84";
            string unencryptedUserName = "emailIsUserName@emailme.com";

            SaveCredentialsReloadFile(unencryptedPassword, unencryptedUserName, DateTime.Now.AddDays(-29));

            Assert.AreEqual(unencryptedPassword, localStorage.LoginInfo.Password);
            Assert.AreEqual(unencryptedUserName, localStorage.LoginInfo.Username);
        }

        [TestMethod]
        public void CredentialsOlderThanLimitAreCleared()
        {
            Assert.IsFalse(File.Exists);

            string unencryptedPassword = "IamPassWordValue84";
            string unencryptedUserName = "emailIsUserName@emailme.com";

            SaveCredentialsReloadFile(unencryptedPassword, unencryptedUserName, DateTime.Now.AddDays(-31));

            Assert.IsFalse(localStorage.HasCredentials);
        }

        private void SaveCredentialsReloadFile(string unencryptedPassword, string unencryptedUserName, DateTime passwordSetDate)
        {
            localStorage.SetCredentials(unencryptedUserName, unencryptedPassword);
            localStorage.String4 = passwordSetDate;
            localStorage.SaveAppSettings();

            File.Refresh();
            Assert.IsTrue(File.Exists);

            localStorage = null;
            localStorage = LocalStorage.LoadLocalStorage();
        }
    }
}
