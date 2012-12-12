using System;
using MainApplication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using com.appdirect.WindowsClient;

namespace MainApplicationUnitTests
{
    [TestClass]
    public class AppDirectApiUnitTest
    {
        [TestMethod]
        public void TestDataIsReturned()
        {
            applicationsApplication[] apps = AppDirectApi.Instance.MyApps;

            Assert.IsNotNull(apps);
        }
    }
}
