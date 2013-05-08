using System;
using System.Globalization;
using AppDirect.WindowsClient.API.VO;
using AppDirect.WindowsClient.Common.API;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AppDirect.WindowsClient.Common.Log;

namespace AppDirect.WindowsClient.API
{
    /// <summary>
    /// Represents a Cached AppDirect Web Api
    /// </summary>
    public class CachedAppDirectApi : ICachedAppDirectApi
    {
        private const int MaxApps = 35;
        private static readonly Regex IdFromUrl = new Regex(@"\d+$");
        private readonly IAppDirectApi _appDirectApi;
        private readonly ILogger _log;

        private volatile IList<Application> _suggestedApps;
        private volatile IList<Application> _myApps;

        public CachedAppDirectApi(IAppDirectApi appDirectApi, ILogger logger)
        {
            _appDirectApi = appDirectApi;
            _log = logger;
        }

        public IList<Application> MyApps
        {
            get
            {
                var myApps = _appDirectApi.MyApps;

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
                    var suggestedApps = _appDirectApi.SuggestedApps;
                    return ConvertList(new List<Application>(), suggestedApps);
                }
                return _suggestedApps;
            }
            set
            {
                _suggestedApps = value;
            }
        }

        public AppDirectSession Session
        {
            get { return _appDirectApi.Session; }
        }

        public UserInfo UserInfo { 
            get 
            { 
                var userInfoRaw = _appDirectApi.UserInfo;
                var result = new UserInfo()
                    {
                        UserId = userInfoRaw.User_Id,
                        CompanyId = userInfoRaw.Company_Id,
                        Email = userInfoRaw.Email,
                        Name = userInfoRaw.Name,
                        GivenName = userInfoRaw.Given_Name,
                        FamilyName = userInfoRaw.Family_Name,
                        Verified = userInfoRaw.Verified,
                        Locale = userInfoRaw.Locale
                    };

                return result;
            } 
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
            var appN = 0;

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

        private static IList<Application> ConvertList(IList<Application> appList, IEnumerable<MyappsMyapp> myApps)
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
                    Price = applicationsApplication.StartingPrice,
                    Status = ConvertStatus(applicationsApplication.Status, applicationsApplication.SubscriptionStatus),
                    SubscriptionId = applicationsApplication.SubscriptionId
                };
                appList.Add(app);
            }

            return appList;
        }

        /// <summary>
        /// Considers the value of status and subscriptionStatus to return the most accurate current status
        /// </summary>
        /// <param name="status"></param>
        /// <param name="subscriptionStatus"></param>
        /// <returns></returns>
        private static Status ConvertStatus(string status, string subscriptionStatus)
        {
            var status1 = TryParseStatus(status);
            var status2 = TryParseStatus(subscriptionStatus);
            
            return (Status)Math.Max((int)status1, (int)status2);
        }

        private static Status TryParseStatus(string status)
        {
            try
            {
                return (Status)Enum.Parse(typeof(Status), status, true);
            }
            catch (ArgumentException)
            {
                return Status.Unknown;
            }
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

        public string GetFreeSubscriptionPlanId(string applicationId)
        {
            var app = _appDirectApi.GetExtendedAppInfo(applicationId);

            foreach (var edition in app.Pricing.Editions)
            {
                foreach (var plan in edition.Plans)
                {
                    var isFreePlan = true;

                    foreach (var cost in plan.Costs)
                    {
                        if (cost.MeteredUsage.HasValue && (cost.MeteredUsage == true))
                        {
                            isFreePlan = false;
                            break;
                        }

                        var isFreeCost = true;

                        foreach (var amount in cost.Amounts)
                        {
                            if (amount.Value.HasValue && (amount.Value > Decimal.Zero))
                            {
                                isFreeCost = false;
                                break;
                            }
                        }

                        if (!isFreeCost)
                        {
                            isFreePlan = false;
                            break;
                        }
                    }

                    if (isFreePlan)
                    {
                        return plan.Id;
                    }
                }
            }

            return null;
        }

        public string ProvisionApplication(string userId, string companyId, string pricingPlanId)
        {
            if (!IsAuthenticated)
            {
                return null;
            }

            var subscriptionWs = new SubscriptionWS();
            subscriptionWs.paymentPlanId = pricingPlanId;
            subscriptionWs.user = new UserWS() {id = userId};
            subscriptionWs.company = new CompanyWS() {id = companyId};

            var resultSubscription = _appDirectApi.SubscribeUser(subscriptionWs);

            return resultSubscription.id;
        }

        public bool DeprovisionApplication(string subscriptionId)
        {
            return _appDirectApi.UnsubscribeUser(subscriptionId);
        }
    }
}
