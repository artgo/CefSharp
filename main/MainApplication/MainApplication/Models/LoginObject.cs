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

        public readonly int DaysBeforePasswordExpires = 30;
        public DateTime PasswordSetDate { get; set; }
        public string Salt
        {
            get
            {
                if (_salt == null)
                {
                    PasswordSetDate = DateTime.Now;
                    _salt = CipherUtility.GetNewSalt();
                }
                return _salt;
            }
            set { _salt = value; }
        }
        
        public string EncryptedPassword
        {
            get { return _password; }
            set
            {
                _password = value;
                _unencryptedPassword = CipherUtility.Decrypt(value, Salt);
            }
        }
        
        public string EncryptedUsername
        {
            get { return _userName; }
            set
            {
                _userName = value;
                _unencryptedUserName = CipherUtility.Decrypt(value, Salt);
            }
        }

        [XmlIgnore]
        public string Password
        {
            get
            {
                return _unencryptedPassword;
            }
            set
            {
                _unencryptedPassword = value;
                _password = CipherUtility.Encrypt(value, Salt);
            }
        }

        [XmlIgnore]
        public string Username
        {
            get { return _unencryptedUserName; }
            set
            {
                _unencryptedUserName = value;
                _userName = CipherUtility.Encrypt(value, Salt);
            }
        }

        public bool IsCredentialsExpired()
        {
            if (PasswordSetDate == DateTime.MinValue || Salt == null)
            {
                return false;
            }

            return DateTime.Now > PasswordSetDate.AddDays(DaysBeforePasswordExpires);
        }
    }
}
