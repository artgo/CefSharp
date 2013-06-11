using AppDirect.WindowsClient.API;
using Ninject;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.UnitTests
{
    [TestFixture]
    public class ServiceLocatorTest
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            ServiceLocator.Initialize();
        }

        [Test]
        public void ReturnsAlwaysTheSameObject()
        {
            var api1 = ServiceLocator.CachedAppDirectApi;
            var api2 = ServiceLocator.CachedAppDirectApi;

            Assert.AreSame(api1, api2);
        }

        [Test]
        public void AppDirectApiIsTheSame()
        {
            var api1 = ServiceLocator.Kernel.Get<IAppDirectApi>();
            var api2 = ServiceLocator.Kernel.Get<IAppDirectApi>();

            Assert.AreSame(api1, api2);
        }

        [Test]
        public void LocalStorageIsTheSame()
        {
            var localStorage1 = ServiceLocator.LocalStorage;
            var localStorage2 = ServiceLocator.LocalStorage;

            Assert.AreSame(localStorage1, localStorage2);
        }
    }
}