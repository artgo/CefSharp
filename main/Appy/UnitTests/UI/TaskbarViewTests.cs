﻿using AppDirect.WindowsClient.InteropAPI;
using AppDirect.WindowsClient.UI;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.UI
{
    [TestFixture]
    public class TaskbarViewTests
    {
        [Test]
        public void Orientation()
        {
            var taskbar = new TaskbarPanel(null, null, null);
            taskbar.PositionChanged(TaskbarPosition.Bottom);
            Assert.Fail("Should fix it");
        }
    }
}