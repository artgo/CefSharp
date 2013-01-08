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
        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = CipherUtility.Encrypt<RijndaelManaged>(value);
            }
        }

        public string UserName { get; set; }

        [XmlIgnore]
        public string UnEncryptedPassword
        {
            get { return CipherUtility.Decrypt<RijndaelManaged>(Password); }
        }
    }
}
