using AppDirect.WindowsClient.API;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.UnitTests
{
    [TestFixture]
    public class AppDirectApiUnitTest
    {
        [Test]
        public void DataIsNullForMyAppsNonAuthenticated()
        {
            var apps = new AppDirectApi().MyApps;

            Assert.IsNull(apps);
        }

        [Test]
        public void DataIsReturnedForSuggestedApps()
        {
            var apps = new AppDirectApi().SuggestedApps;

            Assert.IsNotNull(apps);
        }

        [Test]
        public void IsAuthenticatedReturnsFalseByDefault()
        {
            Assert.IsFalse(new AppDirectApi().IsAuthenticated);
        }

        [Test]
        public void IsAuthenticatedReturnsTrueIfAuthenticated()
        {
            var api = new AppDirectApi();
            Assert.IsFalse(api.IsAuthenticated);
            api.Authenticate(TestData.TestUsername, TestData.TestPassword);
            Assert.IsTrue(api.IsAuthenticated);
        }

        [Test]
        public void IsNotAuthenticatedAfterUnAuthentication()
        {
            var api = new AppDirectApi();
            api.Authenticate(TestData.TestUsername, TestData.TestPassword);
            api.UnAuthenticate();
            Assert.IsFalse(api.IsAuthenticated);
        }
    }
}
