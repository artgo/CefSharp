using MainApplication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using com.appdirect.WindowsClient;
using com.appdirect.WindowsClient.DataAccess;

namespace MainApplicationUnitTests
{
    [TestClass]
    public class AppDirectApiUnitTest
    {
        [TestMethod]
        public void TestDataIsReturnedForMyApps()
        {
            WebApplicationsListApplication[] apps = AppDirectApi.Instance.MyApps;

            Assert.IsNotNull(apps);
        }

        [TestMethod]
        public void TestDataIsReturnedForSuggestedApps()
        {
            WebApplicationsListApplication[] apps = AppDirectApi.Instance.SuggestedApps;

            Assert.IsNotNull(apps);
        }
    }
}
