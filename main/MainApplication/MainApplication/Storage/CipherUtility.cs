using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AppDirect.WindowsClient.Storage
{
    public static class CipherUtility
    {
        private const string Password = "1WLgWTe4m8(ObG*qwY(J)J8JubUcrX%JBjdAaNjNu3@$Xze(d#vGyrkV3Po#rws8Uk)Z5q7L$1aAcErsmwW$KfoKL3L0SYartNGDHWZ4yvac)ANMpADS)kno#Fghg4j!DRDEujZ2MpccuSp8r&fCQ&R6JCwEh8k8uIeMr1@sw95&E7Lt*DmD&UomAxJax*r4vKXpv7OW7pKbs@@jF(Xj4vtPxuhyo9zH)GvJts(Ohdl%dxrROQfzj@(FtoU^bzqwzGhzyt7SK)hrVnPUpkK&N0*gjXDJ*lRt78dEyrKNbQm%4MBv510%xbz9$5dz@@xRIsmFlPNGg53cP^5zvTn8EMoeb%jjE9uB(6gTqUfuOJt9%mLd#SCCdu%79jwFA4anIOdhOUbwNmdt0cx@Bs75s3fSdEODhlBox&#fNvKLPRNhMHgglzxAvuSLfUErU4Zm9(ZawbLMD!noD#HwOyQhXxwLfIc3lM^2R%@5wV3zLJxMxsIKXa^*u!o(%y3gcy";
        private const int SaltLength = 255;

        public static string Encrypt(string value, string salt)
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(Password, Encoding.Unicode.GetBytes(salt));

            RijndaelManaged algorithm = new RijndaelManaged();

            byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

            ICryptoTransform transform = algorithm.CreateEncryptor(rgbKey, rgbIV);

            using (MemoryStream buffer = new MemoryStream())
            {
                using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
                    {
                        writer.Write(value);
                    }
                }

                return Convert.ToBase64String(buffer.ToArray());
            }
        }

        public static string Decrypt(string text, string salt)
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(Password, Encoding.Unicode.GetBytes(salt));

            SymmetricAlgorithm algorithm = new RijndaelManaged();

            byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

            ICryptoTransform transform = algorithm.CreateDecryptor(rgbKey, rgbIV);

            using (MemoryStream buffer = new MemoryStream(Convert.FromBase64String(text)))
            {
                using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public static string GetNewSalt()
        {
           char[] alphaSet; 
           alphaSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()".ToCharArray(); 
           var crypto = new RNGCryptoServiceProvider();
           var bytes = new byte[SaltLength];
           crypto.GetBytes(bytes); //get a bucket of very random bytes
           var tempSB = new StringBuilder(SaltLength);
           foreach (var b in bytes)
           {   // use b , a random from 0-255 as the index to our source array. Just mod on length set
               tempSB.Append(alphaSet[b % (alphaSet.Length)]);
           }
           return tempSB.ToString();
        }
    }
}
