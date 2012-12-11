using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MainApplication
{
    public class CachedAppDirectAPI
    {
        public IEnumerable<Application> MyApps { get; set; }
        public IEnumerable<Application> SuggestedApps { get; set; }
    }
}
