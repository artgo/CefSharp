using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using com.appdirect.WindowsClient;
using com.appdirect.WindowsClient.DataAccess;

namespace MainApplication
{
    public class CachedAppDirectAPI
    {
        private readonly AppDirectApi _appDirectApi = AppDirectApi.Instance;
        private static readonly CachedAppDirectAPI instance = new CachedAppDirectAPI();
        private const int MaxApps = 10;

        private CachedAppDirectAPI() { }

        public static CachedAppDirectAPI Instance {
            get
            {
                return instance;
            }
        } 

        public IEnumerable<Application> MyApps {
            get
            {
                WebApplicationsListApplication[] myApps = _appDirectApi.MyApps;
                return convertList(myApps);
            }
        }
        public IEnumerable<Application> SuggestedApps
        {
            get
            {
                WebApplicationsListApplication[] suggestedApps = _appDirectApi.SuggestedApps;
                return convertList(suggestedApps);
            }
        }

        private static IEnumerable<Application> convertList(WebApplicationsListApplication[] myApps)
        {
            IList<Application> appList = new List<Application>(MaxApps);
            int appN = 0;
            foreach (var applicationsApplication in myApps)
            {
                Application app = new Application()
                    {
                        Description = applicationsApplication.Description,
                        Id = applicationsApplication.Id,
                        ImagePath = applicationsApplication.IconUrl,
                        Name = applicationsApplication.Name,
                        URL = new Uri(applicationsApplication.Href)
                    };
                appList.Add(app);
                if (++appN >= MaxApps)
                {
                    break;
                }
            }
            return appList;
        }
    }
}
