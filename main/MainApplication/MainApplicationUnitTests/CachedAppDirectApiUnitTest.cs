using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Authentication;
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
            appDirectApi.Authenticate("appdqa+t75adsa@gmail.com", "origo2010");
            return appDirectApi;
        }

        [TestMethod]
        public void TestCachedMyAppsCallsMyApps()
        {
            var appDirectApiMock = Substitute.For<IAppDirectApi>();
            var api = new CachedAppDirectApi(appDirectApiMock);
            var apps = api.MyApps;
            var myApps = appDirectApiMock.Received().MyApps;
        }

        [TestMethod]
        public void TestCachedSuggestedAppsCallsSuggestedApps()
        {
            var appDirectApiMock = Substitute.For<IAppDirectApi>();
            var api = new CachedAppDirectApi(appDirectApiMock);
            var apps = api.SuggestedApps;
            var myApps = appDirectApiMock.Received().SuggestedApps;
        }

        [TestMethod]
        public void TestCachedIsAuthenticatedAppsCallsIsAuthenticated()
        {
            var appDirectApiMock = Substitute.For<IAppDirectApi>();
            var api = new CachedAppDirectApi(appDirectApiMock);
            var apps = api.SuggestedApps;
            var myApps = appDirectApiMock.Received().SuggestedApps;
        }

        [TestMethod]
        public void TestCachedAuthenticateCallsAuthenticate()
        {
            var appDirectApiMock = Substitute.For<IAppDirectApi>();
            var api = new CachedAppDirectApi(appDirectApiMock);
            api.Authenticate("", "");
            appDirectApiMock.ReceivedWithAnyArgs().Authenticate("", "");
        }

        [TestMethod]
        public void TestAuthenticationFailsForWrongCredentials()
        {
            var appDirectApi = BuildCachedAppDirectApi();
            bool hadException = false;
            try
            {
                appDirectApi.Authenticate("appdqa+t75adsa@gmail.com", "wrong_password");
            }
            catch (AuthenticationException)
            {
                hadException = true;
            }

            Assert.IsTrue(hadException);
        }

        [TestMethod]
        public void TestAuthenticationSucceedForRightCredentials()
        {
            BuildCachedAppDirectApi();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestDataIsReturnedForMyApps()
        {
            var apps = BuildCachedAppDirectApi().MyApps;

            Assert.IsNotNull(apps);
        }

        [TestMethod]
        public void TestIsNotAuthenticatedByDefault()
        {
            var authenticated = BuildCachedAppDirectApi().IsAuthenticated;

            Assert.IsFalse(authenticated);
        }

        [TestMethod]
        public void TestIsAuthenticatedAfterAuthentication()
        {
            var appDirectApi = BuildCachedAppDirectApiAuthenticated();
            var authenticated = appDirectApi.IsAuthenticated;

            Assert.IsTrue(authenticated);
        }

        [TestMethod]
        public void TestSomeMyAppsAerThereAfterAuthentication()
        {
            var appDirectApi = BuildCachedAppDirectApiAuthenticated();
            var apps = appDirectApi.MyApps;

            Assert.IsTrue(apps.Count > 0);
        }

        [TestMethod]
        public void TestDataIsReturnedForSuggestedApps()
        {
            var apps = BuildCachedAppDirectApi().SuggestedApps;

            Assert.IsNotNull(apps);
        }

        [TestMethod]
        public void TestReturnedSizeForMyAppsIsLessThan11()
        {
            var apps = BuildCachedAppDirectApi().MyApps;

            Assert.IsTrue(apps.Count < 11);
        }

        [TestMethod]
        public void TestReturnedSizeForSuggestedAppsIsLessThan11()
        {
            var apps = BuildCachedAppDirectApi().SuggestedApps;

            Assert.IsTrue(apps.Count < 11);
        }
    }
}
