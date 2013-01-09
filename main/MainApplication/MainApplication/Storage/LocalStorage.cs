using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using AppDirect.WindowsClient.Models;

namespace AppDirect.WindowsClient.Storage
{
    ///<summary>
    /// Represents the Serializable Data that persists locally 
    ///</summary>
    public class LocalStorage
    {
        private const string FileName = @"\AppDirect\LocalStorage";

        public List<Application> InstalledLocalApps { get; set; }
        
        private const int DaysBeforePasswordExpires = 30;

        public LoginObject LoginInfo { get; set; }

        public bool HasCredentials
        {
            get
            {
                return LoginInfo != null &&
                       !String.IsNullOrEmpty(LoginInfo.String1) &&
                       !String.IsNullOrEmpty(LoginInfo.String2) &&
                       !String.IsNullOrEmpty(LoginInfo.String3) &&
                       LoginInfo.String4.AddDays(DaysBeforePasswordExpires) > DateTime.Now;
            }                               
        }                                   
                                            

        public static LocalStorage LoadLocalStorage()
        {
            // Create an XmlSerializer for the LocalStorage type.
            XmlSerializer mySerializer = new XmlSerializer(typeof(LocalStorage));
            FileInfo fi = new FileInfo(Environment.SpecialFolder.ApplicationData + FileName);

            // If the file exists, open it.
            if (fi.Exists)
            {
                using (FileStream fileStream = fi.OpenRead())
                {
                    // Create a new instance of the LocalStorage by deserializing the file.
                    var localStorage = (LocalStorage) mySerializer.Deserialize(fileStream);

                    if (!localStorage.HasCredentials)
                    {
                        localStorage.ClearLoginCredentials();
                    }

                    return localStorage;
                }
            }
            else
            {
                return new LocalStorage();
            }
        }

        public void SaveAppSettings()
        {
            //Create the directory if it does not exist
            var fileInfo = new FileInfo(Environment.SpecialFolder.ApplicationData + FileName);
            if (fileInfo.Directory != null)
            {
                fileInfo.Directory.Create();
            }

            // Create an XmlSerializer for the LocalStorage type.
            XmlSerializer mySerializer = new XmlSerializer(typeof (LocalStorage));

            using (StreamWriter streamWriter = new StreamWriter(Environment.SpecialFolder.ApplicationData + FileName, false))
            {
                // Serialize this instance of the LocalStorage class to the config file.
                mySerializer.Serialize(streamWriter, this);
            }
        }

        public void ClearLoginCredentials()
        {
            LoginInfo = null;
        }

        public void SetCredentials(string username, string password)
        {
            LoginInfo = new LoginObject();
            LoginInfo.String3 = CipherUtility.GetNewSalt();
            LoginInfo.String1 = CipherUtility.Encrypt(username, LoginInfo.String3);
            LoginInfo.String2 = CipherUtility.Encrypt(password, LoginInfo.String3);
            LoginInfo.String4 = DateTime.Now.Date;
        }
    }
}
