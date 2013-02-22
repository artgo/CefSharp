using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.API;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests
{
    [TestFixture]
    public class HelperTests
    {
        [Test]
        [STAThread]
        public void GetClickedAppFromContextMenuReturnsApp()
        {
            MenuItem testMenuItem = new MenuItem();
            var testApp = LocalApplications.AppStoreApp;
            testMenuItem.DataContext = testApp;

            Assert.AreEqual(testApp, Helper.GetClickedAppFromContextMenuClick(testMenuItem));
        }
    }
}
