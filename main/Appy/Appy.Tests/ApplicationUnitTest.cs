using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppDirect.WindowsClient.Tests
{
    [TestClass]
    public class ApplicationUnitTest
    {
        private readonly Application app1 = new Application()
        {
            Name = "App1",
            Id = "10103"
        };

        private readonly Application app2 = new Application()
        {
            Name = "App2",
            Id = "10103"
        };

        private readonly Application app3 = new Application()
        {
            Name = "App3",
            Id = "1010"
        };

        [TestMethod]
        public void EqualsReturnsTrueForIdenticalIds()
        {
            Assert.AreEqual(app1, app2);
        }

        [TestMethod]
        public void EqualsReturnsFalseForDifferentIds()
        {
            Assert.AreNotEqual(app1, app3);
        }

        [TestMethod]
        public void GetHashReturnsSameForIdenticalIds()
        {
            Assert.AreEqual(app1.GetHashCode(), app2.GetHashCode());
        }

        [TestMethod]
        public void GetHashReturnsNotSameForDifferentIds()
        {
            Assert.AreNotEqual(app1.GetHashCode(), app3.GetHashCode());
        }

        [TestMethod]
        public void CopiedApplicationsAreEqual()
        {
            List<Application> localApplications = LocalApplications.LocalApplicationsList;

            var copyOfLocal = new ObservableCollection<Application>(localApplications);

            foreach (var application in copyOfLocal)
            {
                Assert.IsTrue(localApplications.Contains(application));
            }
        }

        [TestMethod]
        public void CopiedApplicationsHashAreEqual()
        {
            List<Application> localApplications = LocalApplications.LocalApplicationsList;

            var copyOfLocal = new ObservableCollection<Application>(localApplications);

            var difference = localApplications.Except(copyOfLocal);

            Assert.AreEqual(0, difference.Count());

        }
    }
}
