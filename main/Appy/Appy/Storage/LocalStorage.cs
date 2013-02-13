using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Models;

namespace AppDirect.WindowsClient.Storage
{
    ///<summary>
    /// Represents the Serializable Data that persists locally 
    ///</summary>
    public sealed class LocalStorage
    {
        private List<string> _hiddenApps = new List<string>();
        private const string FileName = @"\LocalStorage";
        private const int DaysBeforePasswordExpires = 30;
        private static readonly string DefaultFileLocation = string.Empty;
        private static readonly FileInfo FileInfo = new FileInfo(Environment.SpecialFolder.ApplicationData + FileName);

        public List<Application> InstalledLocalApps{ get; set;}
        public List<Application> InstalledAppDirectApps { get; set; }
        public List<Application> LastSuggestedApps { get; set; }
        public bool UpdateDownloaded { get; set; }  

        [XmlIgnore]
        public List<Application> AllInstalledApplications
        {
            get
            {
                if (InstalledLocalApps == null)
                {
                    InstalledLocalApps = new List<Application>();
                }

                if (InstalledAppDirectApps == null)
                {
                    InstalledAppDirectApps = new List<Application>();
                }

                return InstalledLocalApps.Concat(InstalledAppDirectApps).ToList();
            }
           
        }

        public List<string> HiddenApps
        {
            get { return _hiddenApps; }
            set { _hiddenApps = value; }
        }

        public LoginObject LoginInfo { get; set; }

        public bool HasCredentials
        {
            get
            {
                return LoginInfo != null &&
                       !String.IsNullOrEmpty(LoginInfo.EncryptedUsername) &&
                       !String.IsNullOrEmpty(LoginInfo.EncryptedPassword) &&
                       !String.IsNullOrEmpty(LoginInfo.Salt) &&
                       LoginInfo.PasswordSetDate.AddDays(DaysBeforePasswordExpires) > DateTime.Now;
            }                               
        }

        public LocalStorage(){}

         
        public LocalStorage(bool loadFromLocalStorage) 
        {
            if (loadFromLocalStorage)
            {
                var mySerializer = new XmlSerializer(typeof(LocalStorage));

                lock (FileInfo)
                {
                    // If the file exists, open it.
                    if (FileInfo.Exists)
                    {

                        using (var fileStream = FileInfo.OpenRead())
                        {
                            // Create a new instance of the LocalStorage by deserializing the file.
                            var localStorage = (LocalStorage)mySerializer.Deserialize(fileStream);

                            LoginInfo = localStorage.LoginInfo;                         
                            InstalledLocalApps = localStorage.InstalledLocalApps;
                            InstalledAppDirectApps = localStorage.InstalledAppDirectApps;
                            UpdateDownloaded = localStorage.UpdateDownloaded;

                            HiddenApps = localStorage.HiddenApps;

                            if (!localStorage.HasCredentials)
                            {
                                localStorage.ClearLoginCredentials();
                            }
                        }
                    }
                }
            }   
        }     

       public void SaveAppSettings()
        {
            //Create the directory if it does not exist
            if (FileInfo.Directory != null)
            {
                FileInfo.Directory.Create();
            }

            // Create an XmlSerializer for the LocalStorage type.
            var mySerializer = new XmlSerializer(typeof (LocalStorage));

            lock (FileInfo)
            {
                using (var streamWriter = new StreamWriter(FileInfo.FullName, false))
                {
                    // Serialize this instance of the LocalStorage class to the config file.
                    mySerializer.Serialize(streamWriter, this);
                }
            }
        }

        public string SaveAppIcon(string imageUrl, string id)
        {
            if (String.IsNullOrEmpty(imageUrl) || String.IsNullOrEmpty(id))
            {
                return DefaultFileLocation;
            }

            id = SanitizeFileName(id);

            var imageFile = new FileInfo(Environment.SpecialFolder.ApplicationData + @"\" + id);

            if (!imageFile.Exists)
            {
                try
                {
                    if (imageFile.Directory != null)
                    {
                        imageFile.Directory.Create();
                    }
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(imageUrl, imageFile.FullName);
                    }
                }
                catch (WebException)
                {
                    imageFile.Delete();
                    return DefaultFileLocation;
                }
            }

            return imageFile.FullName;
        }

        private string SanitizeFileName(string id)
        {
            var newId = id;
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                newId = newId.Replace(c, '_');
            }
            return newId;
        }

        public void ClearLoginCredentials()
        {
            LoginInfo = null;
        }

        public void SetCredentials(string username, string password)
        {
            LoginInfo = new LoginObject {Salt = CipherUtility.GetNewSalt()};
            LoginInfo.EncryptedUsername = CipherUtility.Encrypt(username, LoginInfo.Salt);
            LoginInfo.EncryptedPassword = CipherUtility.Encrypt(password, LoginInfo.Salt);
            LoginInfo.PasswordSetDate = DateTime.Now.Date;
        }
    }
}
