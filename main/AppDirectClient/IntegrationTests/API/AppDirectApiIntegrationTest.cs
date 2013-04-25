using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Tests;
using NUnit.Framework;

namespace AppDirect.WindowsClient.IntegrationTests.API
{
    [TestFixture]
    public class AppDirectApiIntegrationTest
    {
        private volatile AppDirectApi _appDirectApi;

        [SetUp]
        public void Init()
        {
            _appDirectApi = new AppDirectApi();
        }

        [Test]
        public void DataIsReturnedForSuggestedApps()
        {
            var apps = _appDirectApi.SuggestedApps;

            Assert.IsNotNull(apps);
        }

        [Test]
        public void IsAuthenticatedReturnsTrueIfAuthenticated()
        {
            Assert.IsFalse(_appDirectApi.IsAuthenticated);
            _appDirectApi.Authenticate(TestData.TestUsername2, TestData.TestPassword2);
            Assert.IsTrue(_appDirectApi.IsAuthenticated);
        }

        [Test]
        public void IsNotAuthenticatedAfterUnAuthentication()
        {
            _appDirectApi.Authenticate(TestData.TestUsername2, TestData.TestPassword2);
            _appDirectApi.UnAuthenticate();
            Assert.IsFalse(_appDirectApi.IsAuthenticated);
        }

        [Test]
        public void UserInfoReturnsNullIfNotAuthenticated()
        {
            var userInfo = _appDirectApi.UserInfo;

            Assert.IsNull(userInfo);
        }

        [Test]
        public void UserInfoReturnsNotNullIfAuthenticated()
        {
            _appDirectApi.Authenticate(TestData.TestUsername2, TestData.TestPassword2);
            var userInfo = _appDirectApi.UserInfo;

            Assert.IsNotNull(userInfo);
        }

        [Test]
        public void UserInfoReturnsNotNullIdsIfAuthenticated()
        {
            _appDirectApi.Authenticate(TestData.TestUsername2, TestData.TestPassword2);
            var userInfo = _appDirectApi.UserInfo;

            Assert.IsNotNull(userInfo.User_Id);
            Assert.IsNotNull(userInfo.Company_Id);
        }

        [Test]
        public void ProductDataIsNotNullForFreeApp()
        {
            var app = _appDirectApi.GetExtendedAppInfo(TestData.TestFreeAppId);

            Assert.IsNotNull(app);
        }

        [Test]
        public void ProductDataIsNotNullForSampleApp()
        {
            var app = _appDirectApi.GetExtendedAppInfo(TestData.TestAppId2);

            Assert.IsNotNull(app);
        }
    }
}
