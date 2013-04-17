using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AppDirect.WindowsClient.Common.API;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.UnitTests
{
    [TestFixture]
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

        [Test]
        public void EqualsReturnsTrueForIdenticalIds()
        {
            Assert.AreEqual(app1, app2);
        }

        [Test]
        public void EqualsReturnsFalseForDifferentIds()
        {
            Assert.AreNotEqual(app1, app3);
        }

        [Test]
        public void GetHashReturnsSameForIdenticalIds()
        {
            Assert.AreEqual(app1.GetHashCode(), app2.GetHashCode());
        }

        [Test]
        public void GetHashReturnsNotSameForDifferentIds()
        {
            Assert.AreNotEqual(app1.GetHashCode(), app3.GetHashCode());
        }

        [Test]
        public void CopiedApplicationsAreEqual()
        {
            var localApplications = LocalApplications.LocalApplicationsList;

            var copyOfLocal = new ObservableCollection<Application>(localApplications);

            foreach (var application in copyOfLocal)
            {
                Assert.IsTrue(localApplications.Contains(application));
            }
        }

        [Test]
        public void CopiedApplicationsHashAreEqual()
        {
            var localApplications = LocalApplications.LocalApplicationsList;

            var copyOfLocal = new ObservableCollection<Application>(localApplications);

            var difference = localApplications.Except(copyOfLocal);

            Assert.AreEqual(0, difference.Count());

        }
    }
}
