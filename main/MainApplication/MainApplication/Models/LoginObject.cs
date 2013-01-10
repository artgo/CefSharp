using System;
using System.Security.Cryptography;
using System.Xml.Serialization;
using AppDirect.WindowsClient.Storage;

namespace AppDirect.WindowsClient.Models
{
    ///<summary>
    ///Represents the User's Login Credentials 
    ///</summary>
    public class LoginObject
    {
        [XmlElement("String1")]
        public string EncryptedUsername { get; set; }

        [XmlElement("String2")]
        public string EncryptedPassword { get; set; }

        [XmlElement("String3")]
        public string Salt { get; set; }

        [XmlElement("String4")]
        public DateTime PasswordSetDate { get; set; }
        
        [XmlIgnore]
        public string Password
        {
            get { return CipherUtility.Decrypt(EncryptedPassword, Salt); }
        }


        [XmlIgnore]
        public string Username
        {
            get { return CipherUtility.Decrypt(EncryptedUsername, Salt); }
        }
    }
}
