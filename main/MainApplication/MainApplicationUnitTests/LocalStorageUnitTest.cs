using System;
using System.IO;
using AppDirect.WindowsClient.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppDirect.WindowsClient.Tests
{
    [TestClass]
    public class LocalStorageUnitTest
    {
        private const string FileName = @"\AppDirect\LocalStorage";
        private FileInfo File = new FileInfo(Environment.SpecialFolder.ApplicationData + FileName);

        [TestInitialize]
        public void Setup()
        {
            File.Delete();
            LocalStorage.Instance.ForceReloadFromFile();
        }

        [TestMethod]
        public void LocalStorageListsNullWithoutFile()
        {
            Assert.IsNull(LocalStorage.Instance.InstalledLocalApps);
            Assert.IsNull(LocalStorage.Instance.SuggestedLocalApps);
        }

        [TestMethod]
        public void LocalStorageListsNotNullWhenFileContainsApps()
        {
            Assert.IsFalse(File.Exists);

            LocalStorage.Instance.InstalledLocalApps = LocalApplications.GetBackUpLocalAppsList();
            LocalStorage.Instance.SuggestedLocalApps = LocalApplications.GetBackUpLocalAppsList();

            LocalStorage.Instance.SaveAppSettings();

            File.Refresh();
            Assert.IsTrue(File.Exists);
            
            LocalStorage.Instance.ForceReloadFromFile();

            Assert.IsNotNull(LocalStorage.Instance.InstalledLocalApps);
            Assert.IsNotNull(LocalStorage.Instance.SuggestedLocalApps);
        }

        [TestMethod]
        public void SaveLocalStorageCreatesStorageFile()
        {
            Assert.IsFalse(File.Exists);
            LocalStorage.Instance.SaveAppSettings();

            File.Refresh();
            Assert.IsTrue(File.Exists);
        }
    }
}
