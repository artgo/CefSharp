using AppDirect.WindowsClient.ObjectMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppDirect.WindowsClient.Tests
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
