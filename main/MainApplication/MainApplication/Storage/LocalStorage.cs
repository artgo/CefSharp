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

        public string String1 { get; set; }//Username
        public string String2 { get; set; }//Password
        public string String3 { get; set; }//Salt
        public DateTime String4 { get; set; }//PasswordSetDate

        private const int DaysBeforePasswordExpires = 30;

        [XmlIgnore]
        public LoginObject LoginInfo { get; set; }

        public bool HasCredentials
        {
            get
            {
                return !String.IsNullOrEmpty(String1) &&
                       !String.IsNullOrEmpty(String2) &&
                       !String.IsNullOrEmpty(String3);
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

                    if (localStorage.HasCredentials && localStorage.String4.AddDays(DaysBeforePasswordExpires) > DateTime.Now)
                    {
                        localStorage.LoginInfo = new LoginObject(localStorage.String1, localStorage.String2, localStorage.String3, localStorage.String4);
                    }
                    else
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
            String1 = null;
            String2 = null;
            String3 = null;
            String4 = DateTime.MinValue;
        }

        public void SetCredentials(string username, string password)
        {
            String3 = CipherUtility.GetNewSalt();
            String1 = CipherUtility.Encrypt(username, String3);
            String2 = CipherUtility.Encrypt(password, String3);
            String4 = DateTime.Now.Date;

            LoginInfo = new LoginObject(String1, String2, String3, String4);
        }
    }
}
