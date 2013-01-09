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
        public string String1 { get; set; }//Username
        public string String2 { get; set; }//Password
        public string String3 { get; set; }//Salt
        public DateTime String4 { get; set; }//PasswordSetDate
        
        [XmlIgnore]
        public string Password
        {
            get { return CipherUtility.Decrypt(String2, String3); }
        }


        [XmlIgnore]
        public string Username
        {
            get { return CipherUtility.Decrypt(String1, String3); }
        }
    }
}
