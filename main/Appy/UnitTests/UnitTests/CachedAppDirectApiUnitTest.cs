using AppDirect.WindowsClient.API;
using NSubstitute;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.UnitTests
{
    [TestFixture]
    public class CachedAppDirectApiUnitTest
    {
        private static ICachedAppDirectApi BuildCachedAppDirectApi()
        {
            return new CachedAppDirectApi(new AppDirectApi());
        }
  
        private static ICachedAppDirectApi BuildCachedAppDirectApiAuthenticated()
        {
            var appDirectApi = BuildCachedAppDirectApi();
            appDirectApi.Authenticate(TestData.TestUsername, TestData.TestPassword);
            return appDirectApi;
        }

        [Test]
        public void CachedMyAppsCallsMyApps()
        {
            var appDirectApiMock = Substitute.For<IAppDirectApi>();
            var api = new CachedAppDirectApi(appDirectApiMock);
            var apps = api.MyApps;
            var myApps = appDirectApiMock.Received().MyApps;
        }

        [Test]
        public void CachedSuggestedAppsCallsSuggestedApps()
        {
            var appDirectApiMock = Substitute.For<IAppDirectApi>();
            var api = new CachedAppDirectApi(appDirectApiMock);
            var apps = api.SuggestedApps;
            var myApps = appDirectApiMock.Received().SuggestedApps;
        }

        [Test]
        public void CachedIsAuthenticatedAppsCallsIsAuthenticated()
        {
            var appDirectApiMock = Substitute.For<IAppDirectApi>();
            var api = new CachedAppDirectApi(appDirectApiMock);
            var apps = api.SuggestedApps;
            var myApps = appDirectApiMock.Received().SuggestedApps;
        }

        [Test]
        public void CachedAuthenticateCallsAuthenticate()
        {
            var appDirectApiMock = Substitute.For<IAppDirectApi>();
            var api = new CachedAppDirectApi(appDirectApiMock);
            api.Authenticate("", "");
            appDirectApiMock.ReceivedWithAnyArgs().Authenticate("", "");
        }

        [Test]
        public void CachedUnAuthenticateCallsUnAuthenticate()
        {
            var appDirectApiMock = Substitute.For<IAppDirectApi>();
            var api = new CachedAppDirectApi(appDirectApiMock);
            api.UnAuthenticate();
            appDirectApiMock.ReceivedWithAnyArgs().UnAuthenticate();
        }

        [Test]
        public void AuthenticationFailsForWrongCredentials()
        {
            var appDirectApi = BuildCachedAppDirectApi();

            Assert.IsFalse(appDirectApi.Authenticate(TestData.TestUsername, "wrong_password"));
        }

        [Test]
        public void AuthenticationSucceedForRightCredentials()
        {
            var appDirectApi = BuildCachedAppDirectApi();

            Assert.IsTrue(appDirectApi.Authenticate(TestData.TestUsername, TestData.TestPassword));
       } 

        [Test]
        public void MyAppsAreNeverNull()
        {
            var apps = BuildCachedAppDirectApi().MyApps;

            Assert.IsNotNull(apps);
        }

        [Test]
        public void IsNotAuthenticatedByDefault()
        {
            var authenticated = BuildCachedAppDirectApi().IsAuthenticated;

            Assert.IsFalse(authenticated);
        }

        [Test]
        public void IsAuthenticatedAfterAuthentication()
        {
            var appDirectApi = BuildCachedAppDirectApiAuthenticated();
            var authenticated = appDirectApi.IsAuthenticated;

            Assert.IsTrue(authenticated);
        }

        [Test]
        public void IsAuthenticatedAfterAuthenticationWithNoApps()
        {
            var appDirectApi = BuildCachedAppDirectApi();
            appDirectApi.Authenticate(TestData.NoAppsUsername, TestData.TestPassword);
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
        public void DataIsReturnedForSuggestedApps()
        {
            var apps = BuildCachedAppDirectApi().SuggestedApps;

            Assert.IsNotNull(apps);
        }

        [Test]
        public void ReturnedSizeForMyAppsIsLessThan11()
        {
            var apps = BuildCachedAppDirectApi().MyApps;

            Assert.IsTrue(apps.Count < 11);
        }

        [Test]
        public void ReturnedSizeForSuggestedAppsIsLessThan11()
        {
            var apps = BuildCachedAppDirectApi().SuggestedApps;

            Assert.IsTrue(apps.Count < 11);
        }
    }
}
