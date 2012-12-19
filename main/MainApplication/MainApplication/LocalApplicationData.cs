using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace MainApplication
{
    public sealed class LocalApplicationData
    {
        [XmlIgnore]
        private static LocalApplicationData _instance;
        
        [XmlIgnore]
        private static readonly object padlock = new object();

        private const string AppStoreUrlString = "https://appcenter.staples.com/home";
        private const string FileName = @"\LocalApplicationData";
        private Uri _appStoreUrl;

        public static LocalApplicationData Instance
        {
            get 
            {
                if (_instance == null)
                {
                    lock (padlock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LocalApplicationData();
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

        public void LoadAppSettings()
        {
            XmlSerializer mySerializer = null;
            FileStream myFileStream = null;

            try
            {
                // Create an XmlSerializer for the ApplicationSettings type.
                mySerializer = new XmlSerializer(typeof (LocalApplicationData));
                FileInfo fi = new FileInfo(Environment.SpecialFolder.ApplicationData
                    + FileName);
                // If the config file exists, open it.
                if (fi.Exists)
                {
                    myFileStream = fi.OpenRead();
                    // Create a new instance of the ApplicationSettings by deserializing the file.
                    _instance =
                        (LocalApplicationData)mySerializer.Deserialize(
                            myFileStream);
                }
                else
                {
                    _instance = new LocalApplicationData();
                    _instance.SuggestedLocalApps = GetBackUpLocalAppsList();

                    SaveAppSettings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // If the FileStream is open, close it.
                if (myFileStream != null)
                {
                    myFileStream.Close();
                }
            }
        }


        public static void SaveAppSettings()
        {
            StreamWriter myWriter = null;

            try
            {
                // Create an XmlSerializer for the 
                // ApplicationSettings type.
                XmlSerializer mySerializer = new XmlSerializer(
                    typeof(LocalApplicationData));
                myWriter =
                    new StreamWriter(Environment.SpecialFolder.ApplicationData
                    + FileName, false);
                // Serialize this instance of the ApplicationSettings 
                // class to the config file.
                mySerializer.Serialize(myWriter, _instance);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // If the FileStream is open, close it.
                if (myWriter != null)
                {
                    myWriter.Close();
                }
            }
        }

        public static List<Application> GetBackUpLocalAppsList()
        {
            var facebook = new Application
            {
                Description = "Mock Facebook App",
                Id = "1",
                ImagePath = "Icons/facebook.png",
                Name = "Facebook",
                UrlString = "http://www.facebook.com",
                AlertCount = 0
            };

            var gmail = new Application
            {
                Description = "Mock Gmail App",
                Id = "2",
                ImagePath = "Icons/gmail.ico",
                Name = "Gmail",
                UrlString = "http://www.gmail.com",
                AlertCount = 1
            };

            var pandora = new Application
            {
                Description = "Mock Pandora App",
                Id = "3",
                ImagePath = "Icons/pandora.png",
                Name = "Pandora",
                UrlString = "http://www.pandora.com",
                AlertCount = 1
            };

            var linkedIn = new Application
            {
                Description = "Mock LinkedIn App",
                Id = "4",
                ImagePath = "Icons/linkedIn.png",
                Name = "LinkedIn",
                UrlString = "http://www.linkedIn.com",
                AlertCount = 1
            };

            return new List<Application> {facebook, gmail, linkedIn, pandora};
        }
    }
}
