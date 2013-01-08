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
        private string _userName;
        private string _unencryptedPassword;
        private string _unencryptedUserName;
        private string _salt;

        public DateTime ExpirationDate { get; set; }

        public string Salt
        {
            get
            {
                if (_salt == null)
                {
                    ExpirationDate = DateTime.Now.AddDays(30);
                    _salt = CipherUtility.GetNewSalt();
                }
                return _salt;
            }
            set { _salt = value; }
        }
        
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                _unencryptedPassword = CipherUtility.Decrypt(value, Salt);
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                _unencryptedUserName = CipherUtility.Decrypt(value, Salt);
            }
        }

        [XmlIgnore]
        public string UnencryptedPassword
        {
            get { return _unencryptedPassword; }
            set
            {
                _unencryptedPassword = value;
                _password = CipherUtility.Encrypt(value, Salt);
            }
        }

        [XmlIgnore]
        public string UnencryptedUsername
        {
            get { return _unencryptedUserName; }
            set
            {
                _unencryptedUserName = value;
                _userName = CipherUtility.Encrypt(value, Salt);
            }
        }

    }
}
