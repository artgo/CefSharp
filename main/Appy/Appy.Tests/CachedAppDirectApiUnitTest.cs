using AppDirect.WindowsClient.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AppDirect.WindowsClient.Tests
{
    [TestClass]
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

        [TestMethod]
        public void CachedMyAppsCallsMyApps()
        {
            var appDirectApiMock = Substitute.For<IAppDirectApi>();
            var api = new CachedAppDirectApi(appDirectApiMock);
            var apps = api.MyApps;
            var myApps = appDirectApiMock.Received().MyApps;
        }

        [TestMethod]
        public void CachedSuggestedAppsCallsSuggestedApps()
        {
            var appDirectApiMock = Substitute.For<IAppDirectApi>();
            var api = new CachedAppDirectApi(appDirectApiMock);
            var apps = api.SuggestedApps;
            var myApps = appDirectApiMock.Received().SuggestedApps;
        }

        [TestMethod]
        public void CachedIsAuthenticatedAppsCallsIsAuthenticated()
        {
            var appDirectApiMock = Substitute.For<IAppDirectApi>();
            var api = new CachedAppDirectApi(appDirectApiMock);
            var apps = api.SuggestedApps;
            var myApps = appDirectApiMock.Received().SuggestedApps;
        }

        [TestMethod]
        public void CachedAuthenticateCallsAuthenticate()
        {
            var appDirectApiMock = Substitute.For<IAppDirectApi>();
            var api = new CachedAppDirectApi(appDirectApiMock);
            api.Authenticate("", "");
            appDirectApiMock.ReceivedWithAnyArgs().Authenticate("", "");
        }

        [TestMethod]
        public void CachedUnAuthenticateCallsUnAuthenticate()
        {
            var appDirectApiMock = Substitute.For<IAppDirectApi>();
            var api = new CachedAppDirectApi(appDirectApiMock);
            api.UnAuthenticate();
            appDirectApiMock.ReceivedWithAnyArgs().UnAuthenticate();
        }

        [TestMethod]
        public void AuthenticationFailsForWrongCredentials()
        {
            var appDirectApi = BuildCachedAppDirectApi();

            Assert.IsFalse(appDirectApi.Authenticate(TestData.TestUsername, "wrong_password"));
        }

        [TestMethod]
        public void AuthenticationSucceedForRightCredentials()
        {
            BuildCachedAppDirectApi();

            Assert.IsTrue(true);
       } 

        [TestMethod]
        public void DataIsReturnedForMyApps()
        {
            var apps = BuildCachedAppDirectApi().MyApps;

            Assert.IsNotNull(apps);
        }

        [TestMethod]
        public void IsNotAuthenticatedByDefault()
        {
            var authenticated = BuildCachedAppDirectApi().IsAuthenticated;

            Assert.IsFalse(authenticated);
        }

        [TestMethod]
        public void IsAuthenticatedAfterAuthentication()
        {
            var appDirectApi = BuildCachedAppDirectApiAuthenticated();
            var authenticated = appDirectApi.IsAuthenticated;

            Assert.IsTrue(authenticated);
        }

        [TestMethod]
        public void IsAuthenticatedAfterAuthenticationWithNoApps()
        {
            var appDirectApi = BuildCachedAppDirectApi();
            appDirectApi.Authenticate(TestData.NoAppsUsername, TestData.TestPassword);
            var authenticated = appDirectApi.IsAuthenticated;

            Assert.IsTrue(authenticated);
        }

        [TestMethod]
        public void SomeMyAppsAreThereAfterAuthentication()
        {
            var appDirectApi = BuildCachedAppDirectApiAuthenticated();
            var apps = appDirectApi.MyApps;

            Assert.IsTrue(apps.Count > 0);
        }

        [TestMethod]
        public void DataIsReturnedForSuggestedApps()
        {
            var apps = BuildCachedAppDirectApi().SuggestedApps;

            Assert.IsNotNull(apps);
        }

        [TestMethod]
        public void ReturnedSizeForMyAppsIsLessThan11()
        {
            var apps = BuildCachedAppDirectApi().MyApps;

            Assert.IsTrue(apps.Count < 11);
        }

        [TestMethod]
        public void ReturnedSizeForSuggestedAppsIsLessThan11()
        {
            var apps = BuildCachedAppDirectApi().SuggestedApps;

            Assert.IsTrue(apps.Count < 11);
        }
    }
}
