using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.API.VO;
using NSubstitute;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.UnitTests
{
    [TestFixture]
    public class AppDirectApiUnitTest
    {
        private volatile AppDirectApi _appDirectApi;

        [SetUp]
        public void Init()
        {
            _appDirectApi = new AppDirectApi();
        }

        [Test]
        public void DataIsNullForMyAppsNonAuthenticated()
        {
            Assert.IsNull(_appDirectApi.MyApps);
        }

        [Test]
        public void IsAuthenticatedReturnsFalseByDefault()
        {
            Assert.IsFalse(_appDirectApi.IsAuthenticated);
        }

        [Test]
        public void ProvisionNullWhenNotAuthenticated()
        {
            var _subscriptionWs = new SubscriptionWS();
            var user = new UserWS() { id = "1" };
            var company = new CompanyWS() { id = "2" };
            _subscriptionWs.paymentPlanId = "0";
            _subscriptionWs.user = user;
            _subscriptionWs.company = company;

            Assert.IsNull(_appDirectApi.SubscribeUser(_subscriptionWs));
        }

        [Test]
        public void DeprovisionFalseWhenNotAuthenticated()
        {
            Assert.IsFalse(_appDirectApi.UnsubscribeUser("5"));
        }
    }
}