using System;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Updates;
using NUnit.Framework;

namespace AppDirect.WindowsClient.IntegrationTests
{
    [TestFixture]
    public class UpdaterTests
    {
        readonly Updater _updater = new Updater();

        [Test]
        public void GetUpdatesReturnsTrueForEmptyVersion()
        {
            Assert.IsTrue(_updater.GetUpdates(String.Empty, 1, 0));
        }

        [Test]
        public void GetUpdatesReturnsFalseForCurrentVersion()
        {
            Assert.IsFalse(_updater.GetUpdates(Helper.ApplicationVersion, 1, 0));
        }
    }
}
