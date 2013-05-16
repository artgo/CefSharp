using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.UI;
using AppDirect.WindowsClient.Tests.Common.UI;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.API
{
    [TestFixture]
    public class ExplorerWatcherTests
    {
        [Test]
        public void ExplorerProcessFound()
        {
            var watcher = new ExplorerWatcher(new TestUiHelper(), null);
            watcher.Start();
            Assert.IsNotNull(watcher.ExplorerProcess);
        }
    }
}
