using AppDirect.WindowsClient.API.VO;
using AppDirect.WindowsClient.Common;
using AppDirect.WindowsClient.Common.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.API
{
    public class AppDirectApi : IAppDirectApi
    {
        private const string JSessionIdParamName = "JSESSIONID";
        private const string InitialSessionIdValue = "2E11DE721BAF465953CAFC0407F8F448-n1";
        private const string LoginParams = "id2_hf_0=&_spring_security_remember_me=on&email={0}&password={1}&signin";
        private const string FormContentType = "application/x-www-form-urlencoded";
        private const string JsonAcceptString = "application/json,text/javascript,*/*;q=0.01";
        private const string HtmlAcceptString = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        private const string UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.0 Safari/537.1";
        private const string LoggedInText = @"window.CurrentUser={";
        private static readonly TimeSpan TimeoutTimeSpan = TimeSpan.FromMinutes(30);
        private static readonly string DomainName = Helper.BaseAppStoreDomainName;
        private static readonly string DomainPrefix = Helper.BaseAppStoreUrl;
        private static readonly string MyAppsUrl = DomainPrefix + @"/api/account/v1/myapps.json";
        private static readonly string LoginUrlStr = DomainPrefix + @"/login?1434449477-1.IFormSubmitListener-loginpanel-signInForm";
        private static readonly string UserInfoUrl = DomainPrefix + @"/api/account/v1/userinfo";
        private static readonly string AppInfoTemplateUrl = DomainPrefix + @"/api/marketplace/v1/products/{0}";

        private static readonly string SubscribeTemplateUrl = DomainPrefix + @"/api/subscriptions/v1";
        private static readonly string UnsubscribeTemplateUrl = DomainPrefix + @"/api/subscriptions/v1/{0}";

        private static readonly string AssignTemplateUrl = DomainPrefix + @"/api/account/v1/companies/{0}/users/{1}/assign/{2}";
        private static readonly string UnassignTemplateUrl = DomainPrefix + @"/api/account/v1/companies/{0}/users/{1}/unassign/{2}";
        private const string postMethod = "POST";
        private const string deleteMethod = "DELETE";

        private readonly Uri _serviceUriSuggested = new Uri(DomainPrefix + @"/api/marketplace/v1/listing?filter=FREE");
        private readonly Uri _cookiesDomain = new Uri(DomainPrefix);
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private readonly XmlSerializer _subscriptionSerializer = new XmlSerializer(typeof(SubscriptionWS));

        private readonly IList<Cookie> _cookies = new List<Cookie>();
        private readonly object _timeContextLockObject = new object();

        private DateTime _time = DateTime.Now;
        private volatile CookieContainer _context = null;

        public MyappsMyapp[] MyApps
        {
            get
            {
                if (IsAuthenticated)
                {
                    var request = BuildHttpWebRequestForUrl(MyAppsUrl, false, true);
                    request.CookieContainer = _context;

                    var response = (HttpWebResponse)request.GetResponse();
                    var responseStream = response.GetResponseStream();
                    if (responseStream == null)
                    {
                        throw new IOException("No response stream returned");
                    }

                    var reader = new StreamReader(responseStream);
                    var result = reader.ReadToEnd();

                    return _serializer.Deserialize<MyappsMyapp[]>(result);
                }

                return null;
            }
        }

        public AppDirectSession Session
        {
            get { return new AppDirectSession() { Cookies = _cookies, ExpirationDate = _time + TimeoutTimeSpan }; }
        }

        private static HttpWebRequest BuildHttpWebRequestForUrl(string urlStr, bool isPost, bool isJson)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlStr);
            httpWebRequest.UserAgent = UserAgent;
            httpWebRequest.Method = isPost ? "POST" : "GET";
            httpWebRequest.Accept = isJson ? JsonAcceptString : HtmlAcceptString;
            httpWebRequest.Headers.Add("accept-language: en-us,en");
            httpWebRequest.Headers.Add("accept-charset: iso-8859-1,*,utf-8");
            httpWebRequest.KeepAlive = true;
            httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.Referer = DomainPrefix + @"/login";

            return httpWebRequest;
        }

        public WebApplicationsListApplication[] SuggestedApps
        {
            get
            {
                var result = new WebClient().DownloadString(_serviceUriSuggested);

                return _serializer.Deserialize<WebApplicationsListApplication[]>(result);
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                bool expired;
                bool authenticated;
                lock (_timeContextLockObject)
                {
                    authenticated = _context != null;
                    expired = (DateTime.Now - _time) > TimeoutTimeSpan;
                }
                if (authenticated && expired)
                {
                    UnAuthenticate();
                }

                return authenticated;
            }
        }

        public bool Authenticate(string key, string secret)
        {
            var request = BuildHttpWebRequestForUrl(LoginUrlStr, true, false);
            var cookies = new CookieContainer();
            cookies.Add(new Cookie(JSessionIdParamName, InitialSessionIdValue, "/", DomainName));
            request.CookieContainer = cookies;
            request.ContentType = FormContentType;

            var encodedKey = HttpUtility.UrlEncode(key);

            var sw = new StreamWriter(request.GetRequestStream());
            sw.Write(String.Format(LoginParams, encodedKey, secret));
            sw.Close();

            var response = (HttpWebResponse)request.GetResponse();

            var authenticatedTime = DateTime.Now;
            var responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                throw new IOException("No response stream returned");
            }
            var reader = new StreamReader(responseStream);
            var result = reader.ReadToEnd();
            if ((response.StatusCode != HttpStatusCode.OK) || String.IsNullOrEmpty(result) || (!result.Contains(LoggedInText)))
            {
                return false;
            }

            _cookies.Clear();

            var cookiesForDomain = cookies.GetCookies(_cookiesDomain);

            for (var j = 0; j < cookiesForDomain.Count; j++)
            {
                var oCookie = cookiesForDomain[j];
                var oC = new Cookie
                    {
                        Domain = request.RequestUri.Host,
                        Expires = oCookie.Expires,
                        Name = oCookie.Name,
                        Path = oCookie.Path,
                        Secure = oCookie.Secure,
                        Value = oCookie.Value
                    };

                _cookies.Add(oC);
            }

            lock (_timeContextLockObject)
            {
                _time = authenticatedTime;
                _context = cookies;
            }

            return true;
        }

        public void UnAuthenticate()
        {
            _context = null;
        }

        public bool RegisterUser(string firstName, string lastName, string password, string confirmPassword,
            string email, string confirmEmail, string companyName, string phone, string industryId, string companySize)
        {
            return true;
        }

        public bool ConfirmUserEmail(string email, string confirmationCode)
        {
            return true;
        }

        public bool IsEmailConfirmed(string email)
        {
            return true;
        }

        public UserInfoRaw UserInfo
        {
            get
            {
                if (!IsAuthenticated)
                {
                    return null;
                }

                var request = BuildHttpWebRequestForUrl(UserInfoUrl, false, true);

                request.CookieContainer = _context;
                var response = (HttpWebResponse)request.GetResponse();

                var responseStream = response.GetResponseStream();
                if (responseStream == null)
                {
                    throw new IOException("No response stream returned");
                }
                var reader = new StreamReader(responseStream);
                var result = reader.ReadToEnd();
                if ((response.StatusCode != HttpStatusCode.OK) || String.IsNullOrEmpty(result))
                {
                    return null;
                }

                return _serializer.Deserialize<UserInfoRaw>(result);
            }
        }

        public SubscriptionWS SubscribeUser(SubscriptionWS subscriptionWs)
        {
            if (subscriptionWs == null)
            {
                throw new ArgumentNullException("subscriptionWs");
            }

            if (subscriptionWs.user == null)
            {
                throw new ArgumentNullException("subscriptionWs.user");
            }

            if (subscriptionWs.company == null)
            {
                throw new ArgumentNullException("subscriptionWs.company");
            }

            if (subscriptionWs.paymentPlanId == null)
            {
                throw new ArgumentNullException("subscriptionWs.paymentPlanId");
            }

            if (!IsAuthenticated)
            {
                return null;
            }

            var requestUrl = OAuthBase.GetOAuthSignedUrl(SubscribeTemplateUrl, postMethod);
            Console.WriteLine(requestUrl);

            var request = BuildHttpWebRequestForUrl(requestUrl, true, false);

            request.ContentType = "application/xml";

            var outStream = request.GetRequestStream();
            _subscriptionSerializer.Serialize(outStream, subscriptionWs);
            outStream.Close();

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                var exceptionResponse = (HttpWebResponse)ex.Response;

                if (exceptionResponse.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new ConflictException();
                }
                if ((int)exceptionResponse.StatusCode == 424)
                {
                    throw new FailedDependencyException();
                }

                throw;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                throw new IOException("No response stream returned");
            }

            var result = _subscriptionSerializer.Deserialize(responseStream) as SubscriptionWS;

            return result;
        }

        public bool UnsubscribeUser(string subscriptionId)
        {
            if (string.IsNullOrEmpty(subscriptionId))
            {
                throw new ArgumentNullException("subscriptionId");
            }

            if (!IsAuthenticated)
            {
                return false;
            }

            var url = string.Format(UnsubscribeTemplateUrl, subscriptionId);
            string requestUrl = OAuthBase.GetOAuthSignedUrl(url, deleteMethod);

            var request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.UserAgent = UserAgent;
            request.Method = deleteMethod;
            request.Accept = HtmlAcceptString;
            request.Headers.Add("accept-language: en-us,en");
            request.Headers.Add("accept-charset: iso-8859-1,*,utf-8");
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;
            request.Referer = DomainPrefix + @"/login";

            var response = (HttpWebResponse)request.GetResponse();

            var responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                throw new IOException("No response stream returned");
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            return true;
        }

        private bool AssignUnassignApiCall(string template, string companyId, string userId, string subscriptionId)
        {
            if (string.IsNullOrEmpty(template))
            {
                throw new ArgumentNullException("template");
            }

            if (string.IsNullOrEmpty(companyId))
            {
                throw new ArgumentNullException("companyId");
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }

            if (string.IsNullOrEmpty(subscriptionId))
            {
                throw new ArgumentNullException("subscriptionId");
            }

            if (!IsAuthenticated)
            {
                return false;
            }

            var request = BuildHttpWebRequestForUrl(string.Format(template, HttpUtility.UrlEncode(companyId),
                HttpUtility.UrlEncode(userId), HttpUtility.UrlEncode(subscriptionId)), true, true);

            request.CookieContainer = _context;
            request.ContentType = "application/json";

            var sw = new StreamWriter(request.GetRequestStream());
            sw.Write("[]");
            sw.Close();

            var response = (HttpWebResponse)request.GetResponse();

            var responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                throw new IOException("No response stream returned");
            }
            var reader = new StreamReader(responseStream);
            var result = reader.ReadToEnd();
            if ((response.StatusCode != HttpStatusCode.OK) || String.IsNullOrEmpty(result))
            {
                return false;
            }

            return true;
        }

        public bool AssignEditionToUser(string companyId, string userId, string subscriptionId)
        {
            return AssignUnassignApiCall(AssignTemplateUrl, companyId, userId, subscriptionId);
        }

        public bool UnassignEditionFromUser(string companyId, string userId, string subscriptionId)
        {
            return AssignUnassignApiCall(UnassignTemplateUrl, companyId, userId, subscriptionId);
        }

        public Product GetExtendedAppInfo(string applicationId)
        {
            var request = BuildHttpWebRequestForUrl(string.Format(AppInfoTemplateUrl, applicationId), false, true);

            request.CookieContainer = _context;
            var response = (HttpWebResponse)request.GetResponse();

            var responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                throw new IOException("No response stream returned");
            }
            var reader = new StreamReader(responseStream);
            var result = reader.ReadToEnd();
            if ((response.StatusCode != HttpStatusCode.OK) || String.IsNullOrEmpty(result))
            {
                return null;
            }

            return _serializer.Deserialize<Product>(result);
        }
    }
}