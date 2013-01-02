using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using AppDirect.WindowsClient;

namespace AppDirect.WindowsClient
{
    ///<summary>
    /// Represents a Cached AppDirect Web Api
    /// </summary>
    public class CachedAppDirectApi
    {
        private readonly AppDirectApi _appDirectApi = AppDirectApi.Instance;
        private static readonly CachedAppDirectApi instance = new CachedAppDirectApi();
        private const int MaxApps = 10;
        private List<Application> _suggestedApps;
        private List<Application> _myApps; 


        private CachedAppDirectApi() { }

        public static CachedAppDirectApi Instance {
            get
            {
                return instance;
            }
        }

        public List<Application> MyApps
        {
            get
            {
                if (_myApps == null)
                {
                    WebApplicationsListApplication[] myApps = _appDirectApi.MyApps;
                    _myApps = ConvertList(myApps);
                }

                return _myApps;
            }
            set { _myApps = value; }
        }

        public List<Application> SuggestedApps
        {
            get
            {
                if (_suggestedApps == null)
                {

                    WebApplicationsListApplication[] suggestedApps = _appDirectApi.SuggestedApps;
                    _suggestedApps = ConvertList(suggestedApps);
                }
                return _suggestedApps;
            }
            set { _suggestedApps = value; }
        }

        private static List<Application> ConvertList(WebApplicationsListApplication[] myApps)
        {
            List<Application> appList = new List<Application>(MaxApps);
            int appN = 0;

            if (myApps == null)
            {
                return appList;
            }

            foreach (var applicationsApplication in myApps)
            {
                Application app = new Application
                    {
                        Description = applicationsApplication.Description,
                        Id = applicationsApplication.Id,
                        Name = applicationsApplication.Name,
                        UrlString = applicationsApplication.Href,
                        ImagePath = applicationsApplication.IconUrl,
                        IsLocalApp = false,
                    };
                   
                appList.Add(app);
                if (++appN >= MaxApps)
                {
                    break;
                }
            }
            return appList;
        }

        public bool Login(LoginObject loginInfo)
        {
            //Temporary condition to test behavior for failed login
            if (loginInfo.UserName == "error")
            {
                return false;
            }

            try
            {
                //make call to API to login

                //Mock assignment of an authToken
                loginInfo.AuthToken = "MockAuthToken";
            }
            catch (Exception)
            {
                MessageBox.Show("Login was unsuccessful");
            }
            
            return true;
        }
    }
}
