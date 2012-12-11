using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MainApplication
{
    public class CachedAppDirectAPI
    {
        public CachedAppDirectAPI Instance { get; private set; } 
        public IEnumerable<Application> MyApps { get; private set; }
        public IEnumerable<Application> SuggestedApps { get; private set; }
    }
}
