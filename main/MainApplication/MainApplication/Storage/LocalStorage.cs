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
    public sealed class LocalStorage
    {
        [XmlIgnore] private static LocalStorage _instance;
        
        private const string AppStoreUrlString = "https://appcenter.staples.com/home";
        private const string FileName = @"\AppDirect\LocalStorage";
        private Uri _appStoreUrl;
        

        public bool HasCredentials
        {
            get
            {
                return Instance.LoginInfo != null && Instance.LoginInfo.UserName != null &&
                       Instance.LoginInfo.Password != null;
            }
        }

        public static LocalStorage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = LoadLocalStorage();
                }
                return _instance;
            }
        }

        public List<Application> InstalledLocalApps { get; set; }          
        public List<Application> SuggestedLocalApps { get; set; }

        [XmlIgnore]
        public Uri AppStoreUrl
        {
            get
            {
                if (_appStoreUrl == null)
                    _appStoreUrl = new Uri(AppStoreUrlString);

                return _appStoreUrl;
            }
        }
        
        [XmlIgnore]
        public LoginObject LoginInfo{get;set;}

        private static LocalStorage LoadLocalStorage()
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
                    return (LocalStorage) mySerializer.Deserialize(fileStream);
                }
            }
            else
            {
                return new LocalStorage();
            }
        }

        public void ForceReloadFromFile()
        {
            _instance = null;
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
                mySerializer.Serialize(streamWriter, _instance);
            }
        }

        public void ClearLoginCredentials()
        {
            Instance.LoginInfo = null;
        }
    }
}
