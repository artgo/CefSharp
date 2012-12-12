using System;
using System.Collections;
using System.Collections.Generic;
using MainApplication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MainApplicationUnitTests
{
    [TestClass]
    public class CachedAppDirectApiUnitTest
    {
        [TestMethod]
        public void TestDataIsReturnedForMyApps()
        {
            IEnumerable<Application> apps = CachedAppDirectAPI.Instance.MyApps;

            Assert.IsNotNull(apps);
        }

        [TestMethod]
        public void TestDataIsReturnedForSuggestedApps()
        {
            IEnumerable<Application> apps = CachedAppDirectAPI.Instance.SuggestedApps;

            Assert.IsNotNull(apps);
        }

        [TestMethod]
        public void TestDataIsReturnedSizeForMyAppsIsLessThan11()
        {
            IEnumerable<Application> apps = CachedAppDirectAPI.Instance.MyApps;

            IList<Application> loadedApps = new List<Application>(apps);

            Assert.IsTrue(loadedApps.Count < 11);
        }

        [TestMethod]
        public void TestDataIsReturnedSizeForSuggestedAppsIsLessThan11()
        {
            IEnumerable<Application> apps = CachedAppDirectAPI.Instance.SuggestedApps;

            IList<Application> loadedApps = new List<Application>(apps);

            Assert.IsTrue(loadedApps.Count < 11);
        }

    }
}
