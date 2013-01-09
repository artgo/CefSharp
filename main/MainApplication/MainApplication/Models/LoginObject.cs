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
        private string _encryptedPassword;
        private string _encryptedUserName;
        private string _password;
        private string _userName;
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
            get { return _encryptedPassword; }
            set
            {
                _encryptedPassword = value;
                _password = CipherUtility.Decrypt(value, Salt);
            }
        }
        
        public string EncryptedUsername
        {
            get { return _encryptedUserName; }
            set
            {
                _encryptedUserName = value;
                _userName = CipherUtility.Decrypt(value, Salt);
            }
        }

        [XmlIgnore]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                _encryptedPassword = CipherUtility.Encrypt(value, Salt);
            }
        }

        [XmlIgnore]
        public string Username
        {
            get { return _userName; }
            set
            {
                _userName = value;
                _encryptedUserName = CipherUtility.Encrypt(value, Salt);
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
