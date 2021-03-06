﻿using AppDirect.WindowsClient.API.Subscription;
using AppDirect.WindowsClient.API.VO;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        public UserInfo UserInfo
        {
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

        public bool Authenticate(string key, string secret, int timeoutMs)
        {
            return _appDirectApi.Authenticate(key, secret, timeoutMs);
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
                if (!String.IsNullOrEmpty(applicationsApplication.Referable) && applicationsApplication.Referable.ToLower() == "true")
                {
                    continue;
                }

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
                var id = applicationsApplication.ProductId;
                if (string.IsNullOrEmpty(id))
                {
                    var match = IdFromUrl.Match(applicationsApplication.MarketplaceUrl);
                    if (match.Captures.Count > 0)
                    {
                        id = IdFromUrl.Match(applicationsApplication.MarketplaceUrl).Captures[0].Value;
                    }
                }

                var app = new Application()
                {
                    Description = applicationsApplication.Description,
                    Id = id,
                    ImagePath = applicationsApplication.ImageUrl,
                    Name = applicationsApplication.Name,
                    UrlString = applicationsApplication.LoginUrl,
                    IsLocalApp = false,
                    Price = applicationsApplication.StartingPrice,
                    Status = StatusHelper.ConvertToDisplayStatus(applicationsApplication.Status, applicationsApplication.SubscriptionStatus),
                    SubscriptionId = applicationsApplication.SubscriptionId
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
            subscriptionWs.user = new UserWS() { id = userId };
            subscriptionWs.company = new CompanyWS() { id = companyId };

            var resultSubscription = _appDirectApi.SubscribeUser(subscriptionWs);

            return resultSubscription.id;
        }

        public bool DeprovisionApplication(string subscriptionId)
        {
            return _appDirectApi.UnsubscribeUser(subscriptionId);
        }

        public string SendUserEmail(string text)
        {
            return _appDirectApi.SendUserEmail(text);
        }
    }
}