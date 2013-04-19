using System.Collections.Generic;
using System.Threading;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Tests;
using NSubstitute;
using NUnit.Framework;

namespace AppDirect.WindowsClient.IntegrationTests.API
{
    [TestFixture]
    public class CachedAppDirectApiIntegrationTest
    {
        private volatile AppDirectApi _appDirectApi;
        private volatile CachedAppDirectApi _cachedAppDirectApi;
        private volatile ILogger _log;

        private ICachedAppDirectApi BuildCachedAppDirectApiAuthenticated()
        {
            _cachedAppDirectApi.Authenticate(TestData.TestUsername2, TestData.TestPassword2);
            return _cachedAppDirectApi;
        }

        [SetUp]
        public void Init()
        {
            _appDirectApi = new AppDirectApi();
            _log = Substitute.For<ILogger>();
            _cachedAppDirectApi = new CachedAppDirectApi(_appDirectApi, _log);
        }

        [Test]
        public void IsAuthenticatedAfterAuthentication()
        {
            var appDirectApi = BuildCachedAppDirectApiAuthenticated();
            var authenticated = appDirectApi.IsAuthenticated;

            Assert.IsTrue(authenticated);
        }

        [Test]
        public void SomeMyAppsAreThereAfterAuthentication()
        {
            var appDirectApi = BuildCachedAppDirectApiAuthenticated();
            var apps = appDirectApi.MyApps;

            Assert.IsTrue(apps.Count > 0);
        }

        [Test]
        public void IsAuthenticatedAfterAuthenticationWithNoApps()
        {
            _cachedAppDirectApi.Authenticate(TestData.NoAppsUsername, TestData.TestPassword);

            Assert.IsTrue(_cachedAppDirectApi.IsAuthenticated);
        }


        [Test]
        public void DataIsReturnedForSuggestedApps()
        {
            Assert.IsNotNull(_cachedAppDirectApi.SuggestedApps);
        }

        [Test]
        public void FreeAppHasFreePlan()
        {
            var planId = _cachedAppDirectApi.GetFreeSubscriptionPlanId(TestData.TestFreeAppId);

            Assert.IsNotNullOrEmpty(planId);
        }

        [Test]
        public void AuthenticationFailsForWrongCredentials()
        {
            Assert.IsFalse(_cachedAppDirectApi.Authenticate(TestData.TestUsername2, "wrong_password"));
        }

        [Test]
        public void AuthenticationSucceedForRightCredentials()
        {
            Assert.IsTrue(_cachedAppDirectApi.Authenticate(TestData.TestUsername2, TestData.TestPassword2));
        } 

        [Test]
        public void ProvisionProvisionsAndDeprovisionDeprovisions()
        {
            _cachedAppDirectApi.Authenticate(TestData.TestUsername2, TestData.TestPassword2);
            var originalMyApps = _cachedAppDirectApi.MyApps;

            var origAppsSet = new HashSet<Application>(originalMyApps);

            Assert.IsFalse(origAppsSet.Contains(new Application() { Id = TestData.TestFreeAppId }));

            var userInfo = _cachedAppDirectApi.UserInfo;
            var planId = _cachedAppDirectApi.GetFreeSubscriptionPlanId(TestData.TestFreeAppId);

            var subscriptionId = _cachedAppDirectApi.ProvisionApplication(userInfo.UserId, userInfo.CompanyId, planId);

            Thread.Sleep(200);

            var myApps = _cachedAppDirectApi.MyApps;

            var appsSet = new HashSet<Application>(myApps);

            Assert.IsTrue(appsSet.Contains(new Application() { Id = TestData.TestFreeAppId }));

            Thread.Sleep(200);

            var result = _cachedAppDirectApi.DeprovisionApplication(subscriptionId);
            Assert.IsTrue(result);

            Thread.Sleep(200);

            var unsMyApps = _cachedAppDirectApi.MyApps;

            var unsAppsSet = new HashSet<Application>(unsMyApps);

            Assert.IsFalse(unsAppsSet.Contains(new Application() { Id = TestData.TestFreeAppId }));
        }
    }
}