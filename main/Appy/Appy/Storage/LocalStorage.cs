using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.Storage
{
    ///<summary>
    /// Represents the Serializable Data that persists locally
    ///</summary>
    public class LocalStorage
    {
        private const string FileName = @"\LocalStorage";
        private const int DaysBeforePasswordExpires = 30;
        private static readonly string DefaultFileLocation = string.Empty;
        public static readonly FileInfo FileInfo = new FileInfo(Environment.SpecialFolder.ApplicationData + FileName);
        private static readonly ILogger _log = new NLogLogger("LocalStorage");

        public List<Application> InstalledLocalApps { get; set; }

        public List<Application> InstalledAppDirectApps { get; set; }

        public List<Application> LastSuggestedApps { get; set; }

        public List<Application> PinnedApps { get; set; }

        public bool IsLoadedFromFile { get; set; }

        public object Locker = new object();

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

        public List<string> HiddenApps { get; set; }

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

        public LocalStorage()
        {
            InstalledLocalApps = new List<Application>();
            InstalledAppDirectApps = new List<Application>();
            LastSuggestedApps = new List<Application>();
            PinnedApps = new List<Application>();
            IsLoadedFromFile = false;
        }

        public void LoadStorage()
        {
            var mySerializer = new XmlSerializer(typeof(LocalStorage));

            lock (FileInfo)
            {
                var localStorage = new LocalStorage();

                // If the file exists, open it.
                if (FileInfo.Exists)
                {
                    try
                    {
                        using (var fileStream = FileInfo.OpenRead())
                        {
                            // Create a new instance of the LocalStorage by deserializing the file.
                            localStorage = (LocalStorage)mySerializer.Deserialize(fileStream);

                            if (!localStorage.HasCredentials)
                            {
                                localStorage.ClearLoginCredentials();
                            }

                            IsLoadedFromFile = true;
                        }
                    }
                    catch (InvalidOperationException e)
                    {
                        _log.ErrorException("Invalid operation", e);
                    }
                    catch (XmlException e)
                    {
                        _log.ErrorException("XML format issue", e);
                    }
                }

                LoginInfo = localStorage.LoginInfo;
                InstalledLocalApps = localStorage.InstalledLocalApps ?? new List<Application>();
                InstalledAppDirectApps = localStorage.InstalledAppDirectApps ?? new List<Application>();
                LastSuggestedApps = localStorage.LastSuggestedApps ?? new List<Application>();
                PinnedApps = localStorage.PinnedApps ?? new List<Application>();
                UpdateDownloaded = localStorage.UpdateDownloaded;
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
            var mySerializer = new XmlSerializer(typeof(LocalStorage));

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
                catch (WebException e)
                {
                    _log.ErrorException("File download error", e);
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

        public void ClearAllStoredData()
        {
            LoginInfo = null;
            InstalledLocalApps = new List<Application>();
            InstalledAppDirectApps = new List<Application>();
            LastSuggestedApps = new List<Application>();
            PinnedApps = new List<Application>();
        }

        public void SetCredentials(string username, string password)
        {
            LoginInfo = new LoginObject { Salt = CipherUtility.GetNewSalt() };
            LoginInfo.EncryptedUsername = CipherUtility.Encrypt(username, LoginInfo.Salt);
            LoginInfo.EncryptedPassword = CipherUtility.Encrypt(password, LoginInfo.Salt);
            LoginInfo.PasswordSetDate = DateTime.Now.Date;
        }
    }
}