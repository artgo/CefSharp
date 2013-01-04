using AppDirect.WindowsClient.ObjectMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace AppDirect.WindowsClient.Tests
{
    [TestClass]
    public class AppDirectApiUnitTest
    {
        [TestMethod]
        public void TestDataIsNullForMyAppsNonAuthenticated()
        {
            var apps = new AppDirectApi().MyApps;

            Assert.IsNull(apps);
        }

        [TestMethod]
        public void TestDataIsReturnedForSuggestedApps()
        {
            var apps = new AppDirectApi().SuggestedApps;

            Assert.IsNotNull(apps);
        }
    }
}
