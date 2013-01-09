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
        private readonly string _encryptedPassword;
        private readonly string _encryptedUserName;

        public LoginObject(string username, string password, string salt, DateTime passwordSetDate )
        {
            _encryptedPassword = password;
            _encryptedUserName = username;
            Salt = salt;
            PasswordSetDate = passwordSetDate;
        }

        private DateTime PasswordSetDate { get; set; }
        public string Salt { get; set; }

        public string Password
        {
            get { return CipherUtility.Decrypt(_encryptedPassword, Salt); }
        }

        public string Username
        {
            get { return CipherUtility.Decrypt(_encryptedUserName, Salt); }
        }
    }
}
