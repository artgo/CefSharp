using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using AppDirect.WindowsClient.Models;

namespace AppDirect.WindowsClient.Storage
{
    ///<summary>
    /// Represents the Serializable Data that persists locally 
    ///</summary>
    public sealed class LocalStorage
    {
        private List<string> _hiddenApps = new List<string>();
        private const string FileName = @"\AppDirect\LocalStorage";
        private const int DaysBeforePasswordExpires = 30;
        FileInfo fileInfo = new FileInfo(Environment.SpecialFolder.ApplicationData + FileName);

        public List<Application> InstalledLocalApps { get; set; }
        public List<Application> InstalledApiApps { get; set; }

        [XmlIgnore]
        public List<Application> AllApplications
        {
            get
            {
                if (InstalledLocalApps == null)
                {
                    InstalledLocalApps = new List<Application>();
                }

                if (InstalledApiApps == null)
                {
                    InstalledApiApps = new List<Application>();
                }

                return InstalledLocalApps.Concat(InstalledApiApps).ToList();
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
                XmlSerializer mySerializer = new XmlSerializer(typeof(LocalStorage));

                lock (fileInfo)
                {
                    // If the file exists, open it.
                    if (fileInfo.Exists)
                    {

                        using (FileStream fileStream = fileInfo.OpenRead())
                        {
                            // Create a new instance of the LocalStorage by deserializing the file.
                            var localStorage = (LocalStorage)mySerializer.Deserialize(fileStream);

                            LoginInfo = localStorage.LoginInfo;                         
                            InstalledLocalApps = localStorage.InstalledLocalApps;
                            InstalledApiApps = localStorage.InstalledApiApps;

                            HiddenApps = localStorage.HiddenApps;


                            if (!localStorage.HasCredentials)
                            {
                                localStorage.ClearLoginCredentials();
                            }
                        }
                    }
                    else
                    {
                    }
                }
            }   
        }     
        
        

       public void SaveAppSettings()
        {
            //Create the directory if it does not exist
            if (fileInfo.Directory != null)
            {
                fileInfo.Directory.Create();
            }

            // Create an XmlSerializer for the LocalStorage type.
            XmlSerializer mySerializer = new XmlSerializer(typeof (LocalStorage));

            lock (fileInfo)
            {
                using (StreamWriter streamWriter = new StreamWriter(fileInfo.FullName, false))
                {
                    // Serialize this instance of the LocalStorage class to the config file.
                    mySerializer.Serialize(streamWriter, this);
                }
            }
        }

        public string SaveAppIcon(string imageUrl, string name)
        {
            if (String.IsNullOrEmpty(imageUrl) || String.IsNullOrEmpty(name))
            {
                return imageUrl;
            }

            SanitizeFileName(name);

            var imageFile = new FileInfo(Environment.SpecialFolder.ApplicationData + @"\AppDirect\" + name);

            if (!imageFile.Exists)
            {
                if (imageFile.Directory != null)
                {
                    imageFile.Directory.Create();
                }

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(imageUrl, imageFile.FullName);
                }
            }
           

            return imageFile.FullName;
        }

        private void SanitizeFileName(string name)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
        }

        public void ClearLoginCredentials()
        {
            LoginInfo = null;
        }

        public void SetCredentials(string username, string password)
        {
            LoginInfo = new LoginObject();
            LoginInfo.Salt = CipherUtility.GetNewSalt();
            LoginInfo.EncryptedUsername = CipherUtility.Encrypt(username, LoginInfo.Salt);
            LoginInfo.EncryptedPassword = CipherUtility.Encrypt(password, LoginInfo.Salt);
            LoginInfo.PasswordSetDate = DateTime.Now.Date;
        }
    }
}
