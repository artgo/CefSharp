using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDirect.WindowsClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MainApplicationUnitTests
{
    [TestClass]
    public class LocalStorageUnitTest
    {
        private const string FileName = @"\AppDirect\LocalStorage";

        [TestMethod]
        public void LoadLocalStorageInitializesAppsLists()
        {
            Assert.IsNull(LocalStorage.Instance.InstalledLocalApps);
            Assert.IsNull(LocalStorage.Instance.SuggestedLocalApps);
            LocalStorage.Instance.LoadLocalStorage();
            Assert.IsNotNull(LocalStorage.Instance.InstalledLocalApps);
            Assert.IsNotNull(LocalStorage.Instance.SuggestedLocalApps);
        }

        [TestMethod]
        public void LoadLocalStorageCreatesStorageFile()
        {
            FileInfo fi = new FileInfo(Environment.SpecialFolder.ApplicationData + FileName);

            fi.Delete();

            Assert.IsFalse(fi.Exists);
            LocalStorage.Instance.LoadLocalStorage();

            fi.Refresh();
            Assert.IsTrue(fi.Exists);
        }

        [TestMethod]
        public void SaveLocalStorageCreatesStorageFile()
        {
            FileInfo fi = new FileInfo(Environment.SpecialFolder.ApplicationData + FileName);

            fi.Delete();

            Assert.IsFalse(fi.Exists);
            LocalStorage.Instance.SaveAppSettings();

            fi.Refresh();
            Assert.IsTrue(fi.Exists);
        }
    }
}
