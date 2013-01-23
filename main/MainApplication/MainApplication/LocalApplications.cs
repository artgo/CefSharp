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
                        Id = "Facebook",
                        LocalImagePath = "Icons/facebook.png",
                        Name = "Facebook",
                        UrlString = "http://www.facebook.com",
                        AlertCount = 0,
                        IsLocalApp = true
                    },

                new Application
                    {
                        Description = "Gmail App",
                        Id = "Gmail ",
                        LocalImagePath = "Icons/gmail.png",
                        Name = "Gmail",
                        UrlString = "http://www.gmail.com",
                        AlertCount = 1,
                        IsLocalApp = true
                    },
                new Application
                    {
                        Description = "Pandora App",
                        Id = "Pandora",
                        LocalImagePath = "Icons/pandora.png",
                        Name = "Pandora",
                        UrlString = "http://www.pandora.com",
                        AlertCount = 1,
                        IsLocalApp = true
                    },
                new Application
                    {
                        Description = "LinkedIn App",
                        Id = "LinkedIn",
                        LocalImagePath = "Icons/linked.png",
                        Name = "LinkedIn",
                        UrlString = "http://www.linkedIn.com",
                        AlertCount = 1,
                        IsLocalApp = true
                    },
                new Application
                    {
                        Description = "TeamChat App",
                        Id = "TeamChat",
                        LocalImagePath = "Icons/teamchat.png",
                        Name = "Team Chat",
                        UrlString = "http://www.teamchat.com",
                        AlertCount = 1,
                        IsLocalApp = true
                    },
                    new Application
                    {
                        Description = "Freshbooks App",
                        Id = "Freshbooks",
                        LocalImagePath = "Icons/freshbooks.png",
                        Name = "Freshbooks",
                        UrlString = "http://www.freshbooks.com",
                        AlertCount = 1,
                        IsLocalApp = true
                    },
                     new Application
                    {
                        Description = "Pinterest",
                        Id = "Pinterest",
                        LocalImagePath = "Icons/pinterest.png",
                        Name = "Pinterest",
                        UrlString = "http://www.Pinterest.com",
                        AlertCount = 0,
                        IsLocalApp = true
                    }, 
                    //new Application
                    //{
                    //    Description = "Yelp",
                    //    Id = "Yelp",
                    //    LocalImagePath = "Icons/yelp.png",
                    //    Name = "Yelp",
                    //    UrlString = "http://www.Yelp.com",
                    //    AlertCount = 0,
                    //    IsLocalApp = true
                    //}, new Application
                    //{
                    //    Description = "Groupon",
                    //    Id = "Groupon",
                    //    LocalImagePath = "Icons/Groupon.png",
                    //    Name = "Groupon",
                    //    UrlString = "http://www.Groupon.com",
                    //    AlertCount = 1,
                    //    IsLocalApp = true
                    //}, new Application
                    //{
                    //    Description = "LivingSocial",
                    //    Id = "LivingSocial",
                    //    LocalImagePath = "Icons/LivingSocial.png",
                    //    Name = "LivingSocial",
                    //    UrlString = "http://www.LivingSocial.com",
                    //    AlertCount =0,
                    //    IsLocalApp = true
                    //}, 
                    //new Application
                    //{
                    //    Description = "Instagram",
                    //    Id = "Instagram",
                    //    LocalImagePath = "Icons/Instagram.png",
                    //    Name = "Instagram",
                    //    UrlString = "http://www.Instagram.com",
                    //    AlertCount = 0,
                    //    IsLocalApp = true
                    //}, 
                    new Application
                    {
                        Description = "Hulu",
                        Id = "Hulu",
                        LocalImagePath = "Icons/Hulu.png",
                        Name = "Hulu",
                        UrlString = "http://www.Hulu.com",
                        AlertCount = 0,
                        IsLocalApp = true
                    }, new Application
                    {
                        Description = "Rdio",
                        Id = "Rdio",
                        LocalImagePath = "Icons/Rdio.png",
                        Name = "Rdio",
                        UrlString = "http://www.Rdio.com",
                        AlertCount = 0,
                        IsLocalApp = true
                    }, 
                    //new Application
                    //{
                    //    Description = "GrubHub",
                    //    Id = "GrubHub",
                    //    LocalImagePath = "Icons/GrubHub.png",
                    //    Name = "GrubHub",
                    //    UrlString = "http://www.GrubHub.com",
                    //    AlertCount = 1,
                    //    IsLocalApp = true
                    //}, 
                    new Application
                    {
                        Description = "Grooveshark",
                        Id = "Grooveshark",
                        LocalImagePath = "Icons/Grooveshark.png",
                        Name = "Grooveshark",
                        UrlString = "http://www.Grooveshark.com",
                        AlertCount = 0,
                        IsLocalApp = true
                    }, new Application
                    {
                        Description = "Reddi",
                        Id = "Reddi",
                        LocalImagePath = "Icons/Reddi.png",
                        Name = "Reddi",
                        UrlString = "http://www.Reddi.com",
                        AlertCount = 0,
                        IsLocalApp = true
                    }, new Application
                    {
                        Description = "Weather Channel",
                        Id = "WeatherChannel",
                        LocalImagePath = "Icons/WeatherChannel.png",
                        Name = "Weather Channel",
                        UrlString = "http://www.WeatherChannel.com",
                        AlertCount = 0,
                        IsLocalApp = true
                    }, new Application
                    {
                        Description = "Yahoo Mail",
                        Id = "Ymail",
                        LocalImagePath = "Icons/Ymail.png",
                        Name = "Y!Mail",
                        UrlString = "http://mail.yahoo.com",
                        AlertCount = 0,
                        IsLocalApp = true
                    }, 
                    //new Application
                    //{
                    //    Description = "ESPN",
                    //    Id = "ESPN",
                    //    LocalImagePath = "Icons/ESPN.png",
                    //    Name = "ESPN",
                    //    UrlString = "http://www.ESPN.com",
                    //    AlertCount = 0,
                    //    IsLocalApp = true
                    //}, 
                    new Application
                    {
                        Description = "Flickr",
                        Id = "Flickr",
                        LocalImagePath = "Icons/Flickr.png",
                        Name = "Flickr",
                        UrlString = "http://www.Flickr.com",
                        AlertCount = 1,
                        IsLocalApp = true
                    }
            };
    }
}
