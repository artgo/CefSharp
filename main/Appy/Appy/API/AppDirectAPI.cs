using AppDirect.WindowsClient.Common.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

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

        private readonly Uri _serviceUriSuggested = new Uri(DomainPrefix + @"/api/marketplace/v1/listing?filter=FREE");
        private readonly Uri _cookiesDomain = new Uri(DomainPrefix);
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
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

            var coockiesForDomain = cookies.GetCookies(_cookiesDomain);

            for (var j = 0; j < coockiesForDomain.Count; j++)
            {
                var oCookie = coockiesForDomain[j];
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
    }
}
