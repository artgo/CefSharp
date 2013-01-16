using System;
using AppDirect.WindowsClient.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace AppDirect.WindowsClient.Tests
{
    [TestClass]
    public class ServiceLocatorTest
    {
        [TestMethod]
        public void ReturnsAlwaysTheSameObject()
        {
            ServiceLocator.Initialize();

            var api1 = ServiceLocator.CachedAppDirectApi;
            var api2 = ServiceLocator.CachedAppDirectApi;

            Assert.AreSame(api1, api2);
        }

        [TestMethod]
        public void AppDirectApiIsTheSame()
        {
            ServiceLocator.Initialize();

            var api1 = ServiceLocator.Kernel.Get<IAppDirectApi>();
            var api2 = ServiceLocator.Kernel.Get<IAppDirectApi>();

            Assert.AreSame(api1, api2);
        }

        [TestMethod]
        public void LocalStorageIsTheSame()
        {
            ServiceLocator.Initialize();

            var localStorage1 = ServiceLocator.LocalStorage;
            var localStorage2 = ServiceLocator.LocalStorage;

            Assert.AreSame(localStorage1, localStorage2);
        }
    }
}
