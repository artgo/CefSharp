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

        public bool HasCredentials
        {
            get
            {
                return LoginInfo != null && LoginInfo.Password != null &&
                       LoginInfo.Password != null;
            }
        }

        public List<Application> InstalledLocalApps { get; set; }
        public LoginObject LoginInfo { get; set; }

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

                    if (localStorage.HasCredentials && localStorage.LoginInfo.IsCredentialsExpired())
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
    }
}
