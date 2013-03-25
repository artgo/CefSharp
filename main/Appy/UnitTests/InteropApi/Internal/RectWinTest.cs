using NUnit.Framework;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    [TestFixture]
    public class RectWinTest
    {
        [Test]
        public void TestWidthIsCalculatedCorrectly()
        {
            var rect = new RectWin() { Left = 1, Right = 10 };
            Assert.AreEqual(9, rect.Width);
        }

        [Test]
        public void TestHeightIsCalculatedCorrectly()
        {
            var rect = new RectWin() { Top = 1, Bottom = 10 };
            Assert.AreEqual(9, rect.Height);
        }

        [Test]
        public void TestSettingWidthSetsRightCorrectly()
        {
            var rect = new RectWin() { Left = 1, Right = 10 };
            rect.Width = 5;
            Assert.AreEqual(6, rect.Right);
        }

        [Test]
        public void TestSettingHeightSetsBottomCorrectly()
        {
            var rect = new RectWin() { Top = 1, Bottom = 10 };
            rect.Height = 5;
            Assert.AreEqual(6, rect.Bottom);
        }
    }
}
