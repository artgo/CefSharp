using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.API;
using System.Collections.Generic;

namespace AppDirect.WindowsClient
{
    ///<summary>
    /// A list of the applications that the Windows Client will recommend in addition to those applications that are recommended via the AppDirect API calls
    ///</summary>
    public static class LocalApplications
    {
        private static readonly LocalApplication AppStore = new LocalApplication()
        {
            Description = "App Store",
            Id = "AppStore",
            LocalImagePath = "Icons/appStore-Icon.png",
            ImagePath = "Icons/appStore-Icon.png",
            Name = "Browse Apps",
            UrlString = Helper.BaseAppStoreUrl + Properties.Resources.AppStorePath,
            BrowserWidth = 1150,
            BrowserHeight = 830
        };

        public static List<Application> LocalApplicationsList
        {
            get { return Applications; }
        }

        public static Application AppStoreApp
        {
            get { return AppStore; }
        }

        private static readonly List<Application> Applications = new List<Application>
            {
                new LocalApplication()
                    {
                        Description = "Facebook App",
                        Id = "Facebook",
                        LocalImagePath = "Icons/facebook.png",
                        ImagePath = "Icons/facebook.png",
                        Name = "Facebook",
                        UrlString = "http://www.facebook.com",
                        BrowserHeight = 700,
                        BrowserWidth = 1100
                    },

                new LocalApplication()
                    {
                        Description = "Gmail App",
                        Id = "Gmail",
                        LocalImagePath = "Icons/gmail.png",
                        ImagePath =  "Icons/gmail.png",
                        Name = "Gmail",
                        UrlString = "http://www.gmail.com",
                        BrowserHeight = 800,
                        BrowserWidth = 1000
                    },
                new LocalApplication()
                    {
                        Description = "Pandora App",
                        Id = "Pandora",
                        LocalImagePath = "Icons/pandora.png",
                        ImagePath = "Icons/pandora.png",
                        Name = "Pandora",
                        UrlString = "http://www.pandora.com"
                    },
                new LocalApplication()
                    {
                        Description = "LinkedIn App",
                        Id = "LinkedIn",
                        LocalImagePath = "Icons/linked.png",
                        ImagePath = "Icons/pandora.png",
                        Name = "LinkedIn",
                        UrlString = "http://www.linkedIn.com"
                    },
                    new LocalApplication()
                    {
                        Description = "Freshbooks App",
                        Id = "Freshbooks",
                        LocalImagePath = "Icons/freshbooks.png",
                        ImagePath = "Icons/freshbooks.png",
                        Name = "Freshbooks",
                        UrlString = "http://www.freshbooks.com"
                    },
                     new LocalApplication()
                    {
                        Description = "Pinterest",
                        Id = "Pinterest",
                        LocalImagePath = "Icons/pinterest.png",
                        ImagePath = "Icons/pinterest.png",
                        Name = "Pinterest",
                        UrlString = "http://www.Pinterest.com",
                        BrowserHeight = 700,
                        BrowserWidth = 1100
                    },
                    new LocalApplication()
                    {
                        Description = "Yelp",
                        Id = "Yelp",
                        LocalImagePath = "Icons/yelp.ico",
                        ImagePath = "Icons/yelp.ico",
                        Name = "Yelp",
                        UrlString = "http://www.Yelp.com",
                    },
                    new LocalApplication()
                    {
                        Description = "Groupon",
                        Id = "Groupon",
                        LocalImagePath = "Icons/Groupon.png",
                        ImagePath = "Icons/Groupon.png",
                        Name = "Groupon",
                        UrlString = "http://www.Groupon.com"
                    },
                    new LocalApplication()
                    {
                        Description = "LivingSocial",
                        Id = "LivingSocial",
                        LocalImagePath = "Icons/LivingSocial.png",
                        ImagePath = "Icons/LivingSocial.png",
                        Name = "LivingSocial",
                        UrlString = "http://www.LivingSocial.com"
                    },
                    new LocalApplication()
                    {
                        Description = "Instagram",
                        Id = "Instagram",
                        LocalImagePath = "Icons/Instagram.png",
                        ImagePath = "Icons/Instagram.png",
                        Name = "Instagram",
                        UrlString = "http://www.Instagram.com",
                        BrowserHeight = 700,
                        BrowserWidth = 800
                    },
                    new LocalApplication()
                    {
                        Description = "Hulu",
                        Id = "Hulu",
                        LocalImagePath = "Icons/Hulu.png",
                        ImagePath = "Icons/Hulu.png",
                        Name = "Hulu",
                        UrlString = "http://www.Hulu.com"
                    }, new LocalApplication()
                    {
                        Description = "Rdio",
                        Id = "Rdio",
                        LocalImagePath = "Icons/Rdio.png",
                        ImagePath = "Icons/Rdio.png",
                        Name = "Rdio",
                        UrlString = "http://www.Rdio.com"
                    },
                    new LocalApplication()
                    {
                        Description = "GrubHub",
                        Id = "GrubHub",
                        LocalImagePath = "Icons/GrubHub.png",
                        ImagePath = "Icons/GrubHub.png",
                        Name = "GrubHub",
                        UrlString = "http://www.GrubHub.com"
                    },
                    new LocalApplication()
                    {
                        Description = "Grooveshark",
                        Id = "Grooveshark",
                        LocalImagePath = "Icons/Grooveshark.png",
                        ImagePath = "Icons/Grooveshark.png",
                        Name = "Grooveshark",
                        UrlString = "http://www.Grooveshark.com"
                    },
                    new LocalApplication()
                    {
                        Description = "Reddit",
                        Id = "Reddit",
                        LocalImagePath = "Icons/Reddi.png",
                        ImagePath = "Icons/Reddi.png",
                        Name = "Reddit",
                        UrlString = "http://www.Reddit.com"
                    },
                    new LocalApplication()
                    {
                        Description = "Weather Channel",
                        Id = "WeatherChannel",
                        LocalImagePath = "Icons/WeatherChannel.png",
                        ImagePath = "Icons/WeatherChannel.png",
                        Name = "Weather Channel",
                        UrlString = "http://www.WeatherChannel.com",
                        BrowserHeight = 900,
                        BrowserWidth = 1100
                    },
                    new LocalApplication()
                    {
                        Description = "Yahoo Mail",
                        Id = "Ymail",
                        LocalImagePath = "Icons/Ymail.png",
                        ImagePath = "Icons/Ymail.png",
                        Name = "Y!Mail",
                        UrlString = "http://mail.yahoo.com"
                    },
                    new LocalApplication()
                    {
                        Description = "Flickr",
                        Id = "Flickr",
                        LocalImagePath = "Icons/Flickr.png",
                        ImagePath = "Icons/Flickr.png",
                        Name = "Flickr",
                        UrlString = "http://www.Flickr.com"
                    }
            };
    }
}