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
            Assert.IsFalse(File.Exists);

            localStorage.InstalledLocalApps = LocalApplications.Applications;
            localStorage.SaveAppSettings();

            File.Refresh();
            Assert.IsTrue(File.Exists);

            localStorage = LocalStorage.LoadLocalStorage();

            Assert.IsNotNull(localStorage.InstalledLocalApps);
        }

        [TestMethod]
        public void SaveLocalStorageCreatesStorageFile()
        {
            Assert.IsFalse(File.Exists);
            localStorage.SaveAppSettings();

            File.Refresh();
            Assert.IsTrue(File.Exists);
        }
    }
}
