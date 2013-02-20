using System;
using AppDirect.WindowsClient.API;
using NUnit.Framework;
using Ninject;

namespace AppDirect.WindowsClient.Tests
{
    [TestFixture]
    public class ServiceLocatorTest
    {
        [Test]
        public void ReturnsAlwaysTheSameObject()
        {
            ServiceLocator.Initialize();

            var api1 = ServiceLocator.CachedAppDirectApi;
            var api2 = ServiceLocator.CachedAppDirectApi;

            Assert.AreSame(api1, api2);
        }

        [Test]
        public void AppDirectApiIsTheSame()
        {
            ServiceLocator.Initialize();

            var api1 = ServiceLocator.Kernel.Get<IAppDirectApi>();
            var api2 = ServiceLocator.Kernel.Get<IAppDirectApi>();

            Assert.AreSame(api1, api2);
        }

        [Test]
        public void LocalStorageIsTheSame()
        {
            ServiceLocator.Initialize();

            var localStorage1 = ServiceLocator.LocalStorage;
            var localStorage2 = ServiceLocator.LocalStorage;

            Assert.AreSame(localStorage1, localStorage2);
        }
    }
}
