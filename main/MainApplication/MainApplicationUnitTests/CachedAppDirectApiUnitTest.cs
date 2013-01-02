using System;
using System.Collections;
using System.Collections.Generic;
using AppDirect.WindowsClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MainApplicationUnitTests
{
    [TestClass]
    public class CachedAppDirectApiUnitTest
    {
        [TestMethod]
        public void TestDataIsReturnedForMyApps()
        {
            IEnumerable<Application> apps = CachedAppDirectApi.Instance.MyApps;

            Assert.IsNotNull(apps);
        }

        [TestMethod]
        public void TestDataIsReturnedForSuggestedApps()
        {
            IEnumerable<Application> apps = CachedAppDirectApi.Instance.SuggestedApps;

            Assert.IsNotNull(apps);
        }

        [TestMethod]
        public void TestDataIsReturnedSizeForMyAppsIsLessThan11()
        {
            IEnumerable<Application> apps = CachedAppDirectApi.Instance.MyApps;

            IList<Application> loadedApps = new List<Application>(apps);

            Assert.IsTrue(loadedApps.Count < 11);
        }

        [TestMethod]
        public void TestDataIsReturnedSizeForSuggestedAppsIsLessThan11()
        {
            IEnumerable<Application> apps = CachedAppDirectApi.Instance.SuggestedApps;

            IList<Application> loadedApps = new List<Application>(apps);

            Assert.IsTrue(loadedApps.Count < 11);
        }

    }
}
