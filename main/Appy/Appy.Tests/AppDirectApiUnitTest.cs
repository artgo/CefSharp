using AppDirect.WindowsClient.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppDirect.WindowsClient.Tests
{
    [TestClass]
    public class AppDirectApiUnitTest
    {
        [TestMethod]
        public void DataIsNullForMyAppsNonAuthenticated()
        {
            var apps = new AppDirectApi().MyApps;

            Assert.IsNull(apps);
        }

        [TestMethod]
        public void DataIsReturnedForSuggestedApps()
        {
            var apps = new AppDirectApi().SuggestedApps;

            Assert.IsNotNull(apps);
        }

        [TestMethod]
        public void IsAuthenticatedReturnsFalseByDefault()
        {
            Assert.IsFalse(new AppDirectApi().IsAuthenticated);
        }

        [TestMethod]
        public void IsAuthenticatedReturnsTrueIfAuthenticated()
        {
            var api = new AppDirectApi();
            Assert.IsFalse(api.IsAuthenticated);
            api.Authenticate(TestData.TestUsername, TestData.TestPassword);
            Assert.IsTrue(api.IsAuthenticated);
        }

        [TestMethod]
        public void IsNotAuthenticatedAfterUnAuthentication()
        {
            var api = new AppDirectApi();
            api.Authenticate(TestData.TestUsername, TestData.TestPassword);
            api.UnAuthenticate();
            Assert.IsFalse(api.IsAuthenticated);
        }
    }
}
