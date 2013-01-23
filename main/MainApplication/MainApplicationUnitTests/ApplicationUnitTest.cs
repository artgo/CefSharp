using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AppDirect.WindowsClient.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppDirect.WindowsClient.Tests
{
    [TestClass]
    public class ApplicationUnitTest
    {
        [TestMethod]
        public void CopiedApplicationsAreEqual()
        {
            List<Application> localApplications = LocalApplications.GetLocalApplications();

            var copyOfLocal = new ObservableCollection<Application>(localApplications);

            foreach (var application in copyOfLocal)
            {
                Assert.IsTrue(localApplications.Contains(application));
            }
        }

        [TestMethod]
        public void CopiedApplicationsHashAreEqual()
        {
            List<Application> localApplications = LocalApplications.GetLocalApplications();

            var copyOfLocal = new ObservableCollection<Application>(localApplications);

            var difference = localApplications.Except(copyOfLocal);

            Assert.AreEqual(0, difference.Count());

        }
    }
}
