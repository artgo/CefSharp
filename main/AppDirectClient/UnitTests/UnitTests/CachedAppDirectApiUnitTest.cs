using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.Log;
using NSubstitute;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.UnitTests
{
    [TestFixture]
    public class CachedAppDirectApiUnitTest
    {
        private volatile IAppDirectApi _appDirectApiMock;
        private volatile CachedAppDirectApi _cachedAppDirectApi;
        private volatile ILogger _log;

        [SetUp]
        public void Init()
        {
            _appDirectApiMock = Substitute.For<IAppDirectApi>();
            _log = Substitute.For<ILogger>();
            _cachedAppDirectApi = new CachedAppDirectApi(_appDirectApiMock, _log);
        }

        [Test]
        public void CachedMyAppsCallsMyApps()
        {
            var apps = _cachedAppDirectApi.MyApps;
            var myApps = _appDirectApiMock.Received().MyApps;
        }

        [Test]
        public void CachedSuggestedAppsCallsSuggestedApps()
        {
            var apps = _cachedAppDirectApi.SuggestedApps;
            var myApps = _appDirectApiMock.Received().SuggestedApps;
        }

        [Test]
        public void CachedIsAuthenticatedAppsCallsIsAuthenticated()
        {
            var apps = _cachedAppDirectApi.SuggestedApps;
            var myApps = _appDirectApiMock.Received().SuggestedApps;
        }

        [Test]
        public void CachedAuthenticateCallsAuthenticate()
        {
            _cachedAppDirectApi.Authenticate("1", "2");
            _appDirectApiMock.ReceivedWithAnyArgs().Authenticate("1", "2");
        }

        [Test]
        public void CachedUnAuthenticateCallsUnAuthenticate()
        {
            _cachedAppDirectApi.UnAuthenticate();
            _appDirectApiMock.ReceivedWithAnyArgs().UnAuthenticate();
        }

        [Test]
        public void MyAppsAreNeverNull()
        {
            Assert.IsNotNull(_cachedAppDirectApi.MyApps);
        }

        [Test]
        public void IsNotAuthenticatedByDefault()
        {
            Assert.IsFalse(_cachedAppDirectApi.IsAuthenticated);
        }

        [Test]
        public void ReturnedSizeForSuggestedAppsIsLessThanOrEqualTo25()
        {
            var suggestedApps = new WebApplicationsListApplication[101];
            for (var i = 0;  i < 101; i++)
            {
                var app = new WebApplicationsListApplication();
                app.Id = "1";
                suggestedApps[i] = app;
            }

            _appDirectApiMock.SuggestedApps.ReturnsForAnyArgs(suggestedApps);

            var apps = _cachedAppDirectApi.SuggestedApps;

            Assert.IsTrue(apps.Count <= 25);
        }
    }
}