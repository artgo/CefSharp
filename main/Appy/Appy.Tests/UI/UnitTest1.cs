using System;
using AppDirect.WindowsClient.InteropAPI;
using AppDirect.WindowsClient.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppDirect.WindowsClient.Tests.UI
{
    [TestClass]
    public class TaskbarViewTests
    {
        [TestMethod]
        public void Orientation()
        {
            var taskbar = new Deskband();
            taskbar.PositionChanged(TaskbarPosition.Bottom);
        }
    }
}
