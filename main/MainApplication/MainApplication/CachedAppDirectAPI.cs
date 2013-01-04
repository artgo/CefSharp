using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using AppDirect.WindowsClient.Models;
using AppDirect.WindowsClient.ObjectMapping;
using Application = AppDirect.WindowsClient.Models.Application;

namespace AppDirect.WindowsClient
{
    /// <summary>
    /// Represents a Cached AppDirect Web Api
    /// </summary>
    public class CachedAppDirectApi : ICachedAppDirectApi
    {
        private readonly IAppDirectApi _appDirectApi;
        private const int MaxApps = 10;
        private IList<Application> _suggestedApps;
        private IList<Application> _myApps; 

        public CachedAppDirectApi(IAppDirectApi appDirectApi)
        {
            _appDirectApi = appDirectApi;
        }

        public IList<Application> MyApps
        {
            get
            {
                MyappsMyapp[] myApps = _appDirectApi.MyApps;
                
                if (_myApps == null)
                {
                    _myApps = ConvertList(new List<Application>(), myApps);
                }
                else
                {
                    _myApps.Clear();
                    ConvertList(_myApps, myApps);
                }

                return _myApps;
            }
        }

        public IList<Application> SuggestedApps
        {
            get
            {
                if (_suggestedApps == null)
                {

                    WebApplicationsListApplication[] suggestedApps = _appDirectApi.SuggestedApps;
                return ConvertList(new List<Application>(), suggestedApps);
                }
                return _suggestedApps;
            }
            set { _suggestedApps = value; }
        }

        public void Authenticate(string key, string secret)
        {
            _appDirectApi.Authenticate(key, secret);
        }

        public bool IsAuthenticated
        {
            get { return _appDirectApi.IsAuthenticated; }
        }

        private static IList<Application> ConvertList(IList<Application> appList, WebApplicationsListApplication[] myApps)
        {
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

        private static IList<Application> ConvertList(IList<Application> appList, MyappsMyapp[] myApps)
        {
            if (myApps == null)
            {
                return appList;
            }

            foreach (var applicationsApplication in myApps)
            {
                Application app = new Application()
                {
                    Description = applicationsApplication.Description,
                    Id = applicationsApplication.MarketplaceUrl,
                    ImagePath = applicationsApplication.ImageUrl,
                    Name = applicationsApplication.Name,
                    UrlString = applicationsApplication.MarketplaceUrl
                };
                appList.Add(app);
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
                Authenticate(loginInfo.UserName, loginInfo.Password);
            }
            catch (Exception)
            {
                return false;
            }
            
            return IsAuthenticated;
        }
    }
}
