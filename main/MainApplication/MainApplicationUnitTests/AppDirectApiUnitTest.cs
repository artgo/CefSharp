using AppDirect.WindowsClient.ObjectMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

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
            api.Authenticate("appdqa+t75adsa@gmail.com", "origo2010");
            Assert.IsTrue(api.IsAuthenticated);
        }

        [TestMethod]
        public void IsNotAuthenticatedAfterUnAuthentication()
        {
            var api = new AppDirectApi();
            api.Authenticate("appdqa+t75adsa@gmail.com", "origo2010");
            api.UnAuthenticate();
            Assert.IsFalse(api.IsAuthenticated);
        }
    }
}
