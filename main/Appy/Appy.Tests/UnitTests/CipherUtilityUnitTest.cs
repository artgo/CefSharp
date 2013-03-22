using AppDirect.WindowsClient.Storage;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.UnitTests
{
    [TestFixture]
    public class CipherUtilityUnitTest
    {
        [Test]
        public void CipherUtilitySymmetric()
        {
            string plaintext = "IamAPl4inTextSTring!@#$%^&*()<>";
            string salt = CipherUtility.GetNewSalt();

            string encryptedText = CipherUtility.Encrypt(plaintext, salt);
            string decryptedText = CipherUtility.Decrypt(encryptedText, salt);

            Assert.AreEqual(plaintext, decryptedText);
        }
    }
}
