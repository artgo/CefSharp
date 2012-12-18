using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace MainApplication
{
    public class ApplicationSettings
    {
        //public static ApplicationSettings Instance = new ApplicationSettings();
        private readonly string _filename = @"\appdirectclient.local";

        public List<Application> InstalledLocalApps { get; set; }
          
        public List<Application> SuggestedLocalApps { get; set; }

        [XmlIgnore]
        public Uri AppStoreUrl { get; set; } 

        //public ApplicationSettings()
        //{
        //    if (LoadAppSettings())
        //        return;
            
        //    var facebook = new Application
        //        {
        //            Description = "Mock Facebook App",
        //            Id = "1",
        //            ImagePath = "Icons/facebook.png",
        //            Name = "Facebook",
        //            URL = new Uri("http://www.facebook.com"),
        //            AlertCount = 0
        //        };

        //    var gmail = new Application
        //    {
        //        Description = "Mock Gmail App",
        //        Id = "2",
        //        ImagePath = "Icons/gmail.png",
        //        Name = "Gmail",
        //        URL = new Uri("http://www.gmail.com"),
        //        AlertCount = 1
        //    };

        //    var pandora = new Application
        //    {
        //        Description = "Mock Pandora App",
        //        Id = "3",
        //        ImagePath = "Icons/pandora.png",
        //        Name = "Pandora",
        //        URL = new Uri("http://www.pandora.com"),
        //        AlertCount = 1
        //    };

        //    var linkedIn = new Application
        //    {
        //        Description = "Mock LinkedIn App",
        //        Id = "4",
        //        ImagePath = "Icons/linkedIn.png",
        //        Name = "LinkedIn",
        //        URL = new Uri("http://www.linkedIn.com"),
        //        AlertCount = 1
        //    };

        //    InstalledLocalApps = new List<Application> { facebook, gmail, linkedIn, pandora, facebook, gmail, linkedIn };
        //    SuggestedLocalApps = new List<Application> {facebook, gmail, linkedIn, pandora};
        //    AppStoreUrl = new Uri("https://appcenter.staples.com/home");

        //    SaveAppSettings();
        //}

        public bool LoadAppSettings()
        {
            XmlSerializer mySerializer = null;
            FileStream myFileStream = null;
            bool fileExists = false;

            try
            {
                // Create an XmlSerializer for the ApplicationSettings type.
                mySerializer = new XmlSerializer(typeof(ApplicationSettings));
                FileInfo fi = new FileInfo(System.Environment.SpecialFolder.ApplicationData + _filename);
                // If the config file exists, open it.
                if (fi.Exists)
                {
                    myFileStream = fi.OpenRead();
                    // Create a new instance of the ApplicationSettings by deserializing the config file.
                    ApplicationSettings myAppSettings = (ApplicationSettings)mySerializer.Deserialize(myFileStream);
                    // Assign the property values to this instance of the ApplicationSettings class.
                    InstalledLocalApps = myAppSettings.InstalledLocalApps;
                    SuggestedLocalApps = myAppSettings.SuggestedLocalApps;
                    AppStoreUrl = myAppSettings.AppStoreUrl;
                    fileExists = true;
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

            return fileExists;
        }

        public bool SaveAppSettings()
        {
            StreamWriter myWriter = null;

            var facebook = new Application
            {
                Description = "Mock Facebook App",
                Id = "1",
                ImagePath = "Icons/facebook.png",
                Name = "Facebook",
                URL = new Uri("http://www.facebook.com"),
                AlertCount = 0
            };

            try
            {
                // Create an XmlSerializer for the ApplicationSettings type.
                XmlSerializer mySerializer = new XmlSerializer(typeof (ApplicationSettings));
                myWriter = new StreamWriter(System.Environment.SpecialFolder.ApplicationData + _filename, false);
                // Serialize this instance of the ApplicationSettings class to the config file.
                mySerializer.Serialize(myWriter, this);
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
            return true;
        }

    }
}
