using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient
{
    public sealed class LocalStorage
    {
        [XmlIgnore]
        private static LocalStorage _instance;
        
        [XmlIgnore]
        private static readonly object padlock = new object();

        private const string AppStoreUrlString = "https://appcenter.staples.com/home";
        private const string FileName = @"\AppDirect\LocalStorage";
        private Uri _appStoreUrl;

        public static LocalStorage Instance
        {
            get 
            {
                if (_instance == null)
                {
                    lock (padlock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LocalStorage();
                        }
                    }
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

        public void LoadLocalStorage()
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
                    _instance = (LocalStorage) mySerializer.Deserialize(fileStream);
                }
            }
            else
            {
                _instance = new LocalStorage();
                _instance.SuggestedLocalApps = LocalApplications.GetBackUpLocalAppsList();
                _instance.InstalledLocalApps = new List<Application>();

                SaveAppSettings();
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
                mySerializer.Serialize(streamWriter, _instance);
            }
        }
    }
}
