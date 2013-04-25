using AppDirect.WindowsClient.API;
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
    }
}