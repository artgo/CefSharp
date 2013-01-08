using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppDirect.WindowsClient.Models;

namespace AppDirect.WindowsClient
{
    ///<summary>
    /// A list of the applications that the Windows Client will recommend in addition to those applications that are recommended via the AppDirect API calls 
    ///</summary>
    public static class LocalApplications
    {
        public static List<Application> Applications = new List<Application>
            {
                new Application
                    {
                        Description = "Mock Facebook App",
                        Id = "1",
                        ImagePath = "Icons/facebook.png",
                        Name = "Facebook",
                        UrlString = "http://www.facebook.com",
                        AlertCount = 0,
                        IsLocalApp = true
                    },

                new Application
                    {
                        Description = "Mock Gmail App",
                        Id = "2",
                        ImagePath = "Icons/gmail.ico",
                        Name = "Gmail",
                        UrlString = "http://www.gmail.com",
                        AlertCount = 1,
                        IsLocalApp = true
                    },
                new Application
                    {
                        Description = "Mock Pandora App",
                        Id = "3",
                        ImagePath = "Icons/pandora.png",
                        Name = "Pandora",
                        UrlString = "http://www.pandora.com",
                        AlertCount = 1,
                        IsLocalApp = true
                    },
                new Application
                    {
                        Description = "Mock LinkedIn App",
                        Id = "4",
                        ImagePath = "Icons/linkedIn.png",
                        Name = "LinkedIn",
                        UrlString = "http://www.linkedIn.com",
                        AlertCount = 1,
                        IsLocalApp = true

                    }
            };
    }
}
