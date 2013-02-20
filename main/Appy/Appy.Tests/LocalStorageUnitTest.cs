﻿using System;
using System.IO;
using AppDirect.WindowsClient.Models;
using AppDirect.WindowsClient.Storage;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests
{
    [TestFixture]
    public class LocalStorageUnitTest
    {
        private const string FileName = @"\LocalStorage";
        private FileInfo File;

        private LocalStorage localStorage;

        [TestFixtureSetUp]
        public void Setup()
        {
            if (File.Exists)
            {
                File.Delete();
            }
            localStorage = new LocalStorage();
            File = localStorage.FileInfo;
        }

        [Test]
        public void InstalledAppsListNullWithoutFile()
        {
            Assert.IsNull(localStorage.InstalledLocalApps);
        }

        [Test]
        public void LocalStorageListsNotNullWhenFileContainsApps()
        {
            localStorage.InstalledLocalApps = LocalApplications.LocalApplicationsList;
            localStorage.SaveAppSettings();

            File.Refresh();
            Assert.IsTrue(File.Exists);

            localStorage = new LocalStorage(true);
            
            Assert.IsNotNull(localStorage.InstalledLocalApps);
        }

        [Test]
        public void SaveLocalStorageCreatesStorageFile()
        {
            localStorage.SaveAppSettings();

            File.Refresh();
            Assert.IsTrue(File.Exists);
        }

        [Test]
        public void HasCredentialsTrueForUnexpiredCredentials()
        {
            string unencryptedPassword = "IamPassWordValue84";
            string unencryptedUserName = "emailIsUserName@emailme.com";

            SaveCredentialsReloadFile(unencryptedPassword, unencryptedUserName, DateTime.Now.AddDays(-29));
            
            Assert.IsTrue(localStorage.HasCredentials);
        }

        [Test]
        public void StoredCredentialsRestoredToOriginalValues()
        {
            string unencryptedPassword = "IamPassWordValue84";
            string unencryptedUserName = "emailIsUserName@emailme.com";

            SaveCredentialsReloadFile(unencryptedPassword, unencryptedUserName, DateTime.Now.AddDays(-29));

            Assert.AreEqual(unencryptedPassword, localStorage.LoginInfo.Password);
            Assert.AreEqual(unencryptedUserName, localStorage.LoginInfo.Username);
        }

        [Test]
        public void CredentialsOlderThanLimitAreCleared()
        {
            string unencryptedPassword = "IamPassWordValue84";
            string unencryptedUserName = "emailIsUserName@emailme.com";

            SaveCredentialsReloadFile(unencryptedPassword, unencryptedUserName, DateTime.Now.AddDays(-31));

            Assert.IsFalse(localStorage.HasCredentials);
        }

        [Test]
        public void BadImageFile()
        {
            string appId = "BadImageTest";
            var imagePath = localStorage.SaveAppIcon("http://Not.a.Real.Url", appId);

            Assert.AreEqual(String.Empty, imagePath);
        }

        private void SaveCredentialsReloadFile(string unencryptedPassword, string unencryptedUserName, DateTime passwordSetDate)
        {
            localStorage.SetCredentials(unencryptedUserName, unencryptedPassword);
            localStorage.LoginInfo.PasswordSetDate = passwordSetDate;
            localStorage.SaveAppSettings();

            File.Refresh();
            Assert.IsTrue(File.Exists);

            localStorage = new LocalStorage(true);
        }
    }
}
