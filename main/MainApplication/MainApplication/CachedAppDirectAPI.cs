using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using AppDirect.WindowsClient.ObjectMapping;
using AppDirect.WindowsClient.ObjectMapping;
using Application = AppDirect.WindowsClient.Models.Application;

namespace AppDirect.WindowsClient
{
    public class CachedAppDirectApi : ICachedAppDirectApi
    /// Represents a Cached AppDirect Web Api
    /// </summary>
    {
        private readonly IAppDirectApi _appDirectApi;
        private ObservableCollection<Application> _myApps;
        private const int MaxApps = 10;
        private List<Application> _suggestedApps;
        private List<Application> _myApps; 


        public CachedAppDirectApi(IAppDirectApi appDirectApi)
        {
            _appDirectApi = appDirectApi;
        }

        public ObservableCollection<Application> MyApps
        {
            get
            {
                MyappsMyapp[] myApps = _appDirectApi.MyApps;
                {
                if (_myApps == null)
                {
                    _myApps = ConvertList(new ObservableCollection<Application>(), myApps);
                }
                else
                {
                    _myApps.Clear();
                    ConvertList(_myApps, myApps);
                }

                return _myApps;
            }
        }

        public ObservableCollection<Application> SuggestedApps
        {
            get
            {
                if (_suggestedApps == null)
                {

                    WebApplicationsListApplication[] suggestedApps = _appDirectApi.SuggestedApps;
                return ConvertList(new ObservableCollection<Application>(), suggestedApps);
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

        private static ObservableCollection<Application> ConvertList(ObservableCollection<Application> appList, WebApplicationsListApplication[] myApps)
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
                        Url = new Uri(applicationsApplication.Href)
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

        private static ObservableCollection<Application> ConvertList(ObservableCollection<Application> appList, MyappsMyapp[] myApps)
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
                    Url = new Uri(applicationsApplication.MarketplaceUrl)
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
