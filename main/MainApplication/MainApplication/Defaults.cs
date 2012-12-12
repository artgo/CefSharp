using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MainApplication
{
    public class Defaults
    {
        public List<Application> InstalledLocalApps { get; set; }
        public List<Application> SuggestedLocalApps { get; set; }
        public Uri AppStoreUrl { get; set; } 

        public Defaults()
        {
            var facebook = new Application
                {
                    Description = "Mock Facebook App",
                    Id = "1",
                    ImagePath = "facebook.png",
                    Name = "Facebook",
                    URL = new Uri("http://www.facebook.com"),
                    AlertCount = 1
                };

            var gmail = new Application
            {
                Description = "Mock Gmail App",
                Id = "2",
                ImagePath = "gmail.png",
                Name = "Gmail",
                URL = new Uri("http://www.gmail.com"),
                AlertCount = 1
            };

            var pandora = new Application
            {
                Description = "Mock Pandora App",
                Id = "3",
                ImagePath = "pandora.png",
                Name = "Pandora",
                URL = new Uri("http://www.pandora.com"),
                AlertCount = 1
            };

            var linkedIn = new Application
            {
                Description = "Mock LinkedIn App",
                Id = "4",
                ImagePath = "linkedIn.png",
                Name = "LinkedIn",
                URL = new Uri("http://www.linkedIn.com"),
                AlertCount = 1
            };

            InstalledLocalApps = new List<Application> { facebook, gmail, linkedIn, pandora };
            SuggestedLocalApps = new List<Application> {facebook, gmail, linkedIn, pandora};
            AppStoreUrl = new Uri("https://appcenter.staples.com/home");
        }
    }
}
