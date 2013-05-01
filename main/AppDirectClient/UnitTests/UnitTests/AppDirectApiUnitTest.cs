using System;
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
        public void SubscribeUserNullWhenNotAuthenticated()
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

        [Test]
        public void SubscribeThrowsErrorForNull()
        {
            Assert.Throws<ArgumentNullException> (() =>_appDirectApi.SubscribeUser(null));
        }

        [Test]
        public void SubscribeThrowsErrorForNullUser()
        {
            var _subscriptionWs = new SubscriptionWS();
            var company = new CompanyWS() { id = "2" };
            _subscriptionWs.paymentPlanId = "0";
            _subscriptionWs.company = company;

            Assert.Throws<ArgumentNullException>(() => _appDirectApi.SubscribeUser(_subscriptionWs));
        }

        [Test]
        public void SubscribeThrowsErrorForNullCompany()
        {
            var _subscriptionWs = new SubscriptionWS();
            var user = new UserWS() { id = "1" };
            _subscriptionWs.paymentPlanId = "0";
            _subscriptionWs.user = user;

            Assert.Throws<ArgumentNullException>(() => _appDirectApi.SubscribeUser(_subscriptionWs));
        }

        [Test]
        public void SubscribeThrowsErrorForNullPaymentPlanId()
        {
            var _subscriptionWs = new SubscriptionWS();
            var user = new UserWS() { id = "1" };
            var company = new CompanyWS() { id = "2" };
            _subscriptionWs.user = user;
            _subscriptionWs.company = company;

            Assert.Throws<ArgumentNullException>(() => _appDirectApi.SubscribeUser(_subscriptionWs));
        }

        [Test]
        public void UnSubscribeThrowsErrorForNullSubscriptionId()
        {
            Assert.Throws<ArgumentNullException>(() => _appDirectApi.UnsubscribeUser(null));
        }
    }
}