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
        public static List<Application> GetLocalApplications()
        {
            return Applications;
        }


        private static readonly List<Application> Applications = new List<Application>
            {
                new Application
                    {
                        Description = "Facebook App",
                        Id = "1",
                        ImagePath = "Icons/facebook.png",
                        Name = "Facebook",
                        UrlString = "http://www.facebook.com",
                        AlertCount = 0,
                        IsLocalApp = true
                    },

                new Application
                    {
                        Description = "Gmail App",
                        Id = "2",
                        ImagePath = "Icons/gmail.png",
                        Name = "Gmail",
                        UrlString = "http://www.gmail.com",
                        AlertCount = 1,
                        IsLocalApp = true
                    },
                new Application
                    {
                        Description = "Pandora App",
                        Id = "3",
                        ImagePath = "Icons/pandora.png",
                        Name = "Pandora",
                        UrlString = "http://www.pandora.com",
                        AlertCount = 1,
                        IsLocalApp = true
                    },
                new Application
                    {
                        Description = "LinkedIn App",
                        Id = "4",
                        ImagePath = "Icons/linked.png",
                        Name = "LinkedIn",
                        UrlString = "http://www.linkedIn.com",
                        AlertCount = 1,
                        IsLocalApp = true
                    },
                new Application
                    {
                        Description = "TeamChat App",
                        Id = "5",
                        ImagePath = "Icons/teamchat.png",
                        Name = "Team Chat",
                        UrlString = "http://www.teamchat.com",
                        AlertCount = 1,
                        IsLocalApp = true
                    },
                    new Application
                    {
                        Description = "Freshbooks App",
                        Id = "6",
                        ImagePath = "Icons/freshbooks.png",
                        Name = "Freshbooks",
                        UrlString = "http://www.freshbooks.com",
                        AlertCount = 1,
                        IsLocalApp = true
                    }
            };
    }
}
