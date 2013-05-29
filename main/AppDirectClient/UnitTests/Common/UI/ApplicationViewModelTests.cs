using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using AppDirect.WindowsClient.UI;
using NUnit.Framework;
using Application = AppDirect.WindowsClient.Common.API.Application;

namespace AppDirect.WindowsClient.Tests.Common.UI
{
    [TestFixture]
    public class ApplicationViewModelTests
    {
        [Test]
        public void DefaultHasVisibleContextMenu()
        {
            var application = new Application();
            var avm = new ApplicationViewModel(application);

            Assert.AreEqual(Visibility.Visible, avm.DisplayContextMenu);
        }

        [Test]
        public void HideContextMenuHasHiddenVisibility()
        {
            var application = new Application() {HideContextMenu = true};
            var avm = new ApplicationViewModel(application);

            Assert.AreEqual(Visibility.Hidden, avm.DisplayContextMenu);
        }
    }
}
