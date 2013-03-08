using System;
using NUnit.Framework;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    [TestFixture]
    public class CoordsPackagerTest
    {
        [Test]
        public void TestPackParamsThrowsOnNullInput()
        {
            var packager = new CoordsPackager();
            Assert.Throws<ArgumentNullException>(delegate { packager.PackParams(null); });
        }

        [Test]
        public void TestUnpackParamsAreTheSameAsOriginal()
        {
            var packager = new CoordsPackager();
            var param = new RectWin() { Left = 1, Top = 2, Right = 3, Bottom = 4 };
            var packedParams = packager.PackParams(param);
            var result = packager.UnpackParams(packedParams.LParam, packedParams.WParam);
            Assert.AreEqual(param, result);
        }
    }
}
