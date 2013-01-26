using System;
using AppDirect.WindowsClient.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppDirect.WindowsClient.Tests
{
    [TestClass]
    public class CipherUtilityUnitTest
    {
        [TestMethod]
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
