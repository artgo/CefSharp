using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using com.appdirect.WindowsClient;
using com.appdirect.WindowsClient.DataAccess;

namespace MainApplication
{
    public class CachedAppDirectApi
    {
        private readonly AppDirectApi _appDirectApi = AppDirectApi.Instance;
        private static readonly CachedAppDirectApi instance = new CachedAppDirectApi();
        private const int MaxApps = 10;

        private CachedAppDirectApi() { }

        public static CachedAppDirectApi Instance {
            get
            {
                return instance;
            }
        } 

        public IEnumerable<Application> MyApps {
            get
            {
                WebApplicationsListApplication[] myApps = _appDirectApi.MyApps;
                return ConvertList(myApps);
            }
        }
        public IEnumerable<Application> SuggestedApps
        {
            get
            {
                WebApplicationsListApplication[] suggestedApps = _appDirectApi.SuggestedApps;
                return ConvertList(suggestedApps);
            }
        }

        private static IEnumerable<Application> ConvertList(WebApplicationsListApplication[] myApps)
        {
            IList<Application> appList = new List<Application>(MaxApps);
            int appN = 0;

            if (myApps == null)
            {
                return appList;
            }

            foreach (var applicationsApplication in myApps)
            {
                Application app = new Application()
                    {
                        Description = applicationsApplication.Description,
                        Id = applicationsApplication.Id,
                        ImagePath = applicationsApplication.IconUrl,
                        Name = applicationsApplication.Name,
                        UrlString = applicationsApplication.Href
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
