using System.Collections.Generic;
using System.Text.RegularExpressions;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.API
{
    /// <summary>
    /// Represents a Cached AppDirect Web Api
    /// </summary>
    public class CachedAppDirectApi : ICachedAppDirectApi
    {
        private readonly IAppDirectApi _appDirectApi;
        private const int MaxApps = 15;
        private IList<Application> _suggestedApps;
        private IList<Application> _myApps;
        private static readonly Regex IdFromUrl = new Regex(@"\d+$");

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

        public AppDirectSession Session
        {
            get { return _appDirectApi.Session; }
        }

        public bool Authenticate(string key, string secret)
        {
            return _appDirectApi.Authenticate(key, secret);
        }

        public void UnAuthenticate()
        {
            _appDirectApi.UnAuthenticate();
        }

        public bool IsAuthenticated
        {
            get { return _appDirectApi.IsAuthenticated; }
        }

        private static IList<Application> ConvertList(IList<Application> appList, IEnumerable<WebApplicationsListApplication> apiAppList)
        {
            int appN = 0;

            if (apiAppList == null)
            {
                return appList;
            }

            foreach (var applicationsApplication in apiAppList)
            {
                var app = new Application
                    {
                        Description = applicationsApplication.Description,
                        Id = applicationsApplication.Id,
                        Name = applicationsApplication.Name,
                        UrlString = applicationsApplication.Url,
                        ImagePath = applicationsApplication.IconUrl,
                        IsLocalApp = false,
                        Price = applicationsApplication.StartingPrice
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
                var app = new Application()
                {
                    Description = applicationsApplication.Description,
                    Id = IdFromUrl.Match(applicationsApplication.MarketplaceUrl).Captures[0].Value,
                    ImagePath = applicationsApplication.ImageUrl,
                    Name = applicationsApplication.Name,
                    UrlString = applicationsApplication.LoginUrl,
                    IsLocalApp = false,
                    Price = applicationsApplication.StartingPrice
                };
                appList.Add(app);
            }
            return appList;
        }

        public bool RegisterUser(string firstName, string lastName, string password, string confirmPassword, string email,
                                 string confirmEmail, string companyName)
        {
            return _appDirectApi.RegisterUser(firstName, lastName, password, confirmPassword, email, confirmEmail, companyName,
                                       null, null, null);
        }

        public bool ConfirmUserEmail(string email, string confirmationCode)
        {
            return _appDirectApi.ConfirmUserEmail(email, confirmationCode);
        }

        public bool IsEmailConfirmed(string email)
        {
            return _appDirectApi.IsEmailConfirmed(email);
        }
    }
}
