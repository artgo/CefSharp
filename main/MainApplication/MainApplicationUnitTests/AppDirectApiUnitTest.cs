using AppDirect.WindowsClient.ObjectMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace AppDirect.WindowsClient.Tests
{
    [TestClass]
    public class AppDirectApiUnitTest
    {
        [TestMethod]
        public void DataIsNullForMyAppsNonAuthenticated()
        {
            var apps = new AppDirectApi().MyApps;

            Assert.IsNull(apps);
        }

        [TestMethod]
        public void DataIsReturnedForSuggestedApps()
        {
            var apps = new AppDirectApi().SuggestedApps;

            Assert.IsNotNull(apps);
        }
    }
}
