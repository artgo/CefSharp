using AppDirect.WindowsClient.InteropAPI.Internal;
using NUnit.Framework;
using System;

namespace AppDirect.WindowsClient.Tests.InteropApi.Internal
{
    [TestFixture]
    public class CoordsPackagerTest
    {
        [Test]
        public void TestUnpackParamsAreTheSameAsOriginal()
        {
            var packager = new CoordsPackager();
            var param = new RECT { Left = 1, Top = 2, Right = 3, Bottom = 4 };
            var packedParams = packager.PackParams(param);
            var result = packager.UnpackParams(packedParams.LParam, packedParams.WParam);
            Assert.AreEqual(param, result);
        }
    }
}