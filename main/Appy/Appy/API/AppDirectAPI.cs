using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.API
{
    public class AppDirectApi : IAppDirectApi
    {
        private const string JSessionIdParamName = "JSESSIONID";
        private const string InitialSessionIdValue = "2E11DE721BAF465953CAFC0407F8F448-n1";
        private static readonly string DomainName = Helper.BaseAppStoreDomainName;
        private static readonly string DomainPrefix = Helper.BaseAppStoreUrl;
        private const string LoginParams = "id2_hf_0=&_spring_security_remember_me=on&email={0}&password={1}&signin";
        private static readonly string MyAppsUrl = DomainPrefix + @"/api/account/v1/myapps.json";
        private const string FormContentType = "application/x-www-form-urlencoded";
        private const string JsonAcceptString = "application/json,text/javascript,*/*;q=0.01";
        private const string HtmlAcceptString = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        private const string UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.0 Safari/537.1";
        private const string LoggedInText = @"window.CurrentUser={";
        private readonly Uri ServiceUriSuggested = new Uri(DomainPrefix + @"/api/marketplace/v1/listing?filter=FREE");
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private DateTime _time = DateTime.Now;
        private volatile CookieContainer _context = null;
        private readonly IList<Cookie> _cookies = new List<Cookie>();
        private TimeSpan timeoutTimeSpan;

        public MyappsMyapp[] MyApps
        {
            get {
                if (IsAuthenticated)
                {
                    var request = BuildHttpWebRequestForUrl(MyAppsUrl, false, true);
                    request.CookieContainer = _context;

                    var response = (HttpWebResponse) request.GetResponse();

                    var reader = new StreamReader(response.GetResponseStream());
                    string result = reader.ReadToEnd();
                    return _serializer.Deserialize<MyappsMyapp[]>(result);
                }

                return null;
            }
        }

        public AppDirectSession Session
        {
            get { return new AppDirectSession(){Cookies = _cookies, ExpirationDate = _time + new TimeSpan(0, 1, 0, 0)}; }
        }

        private static HttpWebRequest BuildHttpWebRequestForUrl(string urlStr, bool isPost, bool isJson)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(urlStr);
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
                string result = new WebClient().DownloadString(ServiceUriSuggested);
                return _serializer.Deserialize<WebApplicationsListApplication[]>(result);
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                timeoutTimeSpan = TimeSpan.FromMinutes(30);
                if ((_context != null) && (DateTime.Now - _time) > (timeoutTimeSpan))
                {
                    UnAuthenticate();
                }
                return _context != null;
            }
        }

        public bool Authenticate(string key, string secret)
        {
            var request = BuildHttpWebRequestForUrl(DomainPrefix + @"/login?1434449477-1.IFormSubmitListener-loginpanel-signInForm", true, false);
            var cookies = new CookieContainer();
            cookies.Add(new Cookie(JSessionIdParamName, InitialSessionIdValue, "/", DomainName));
            request.CookieContainer = cookies;
            request.ContentType = FormContentType;

            string encodedKey = HttpUtility.UrlEncode(key);

            var sw = new StreamWriter(request.GetRequestStream());
            sw.Write(String.Format(LoginParams, encodedKey, secret));
            sw.Close();

            var response = (HttpWebResponse) request.GetResponse();

            _time = DateTime.Now;

            var reader = new StreamReader(response.GetResponseStream());
            string result = reader.ReadToEnd();
            if ((response.StatusCode != HttpStatusCode.OK) || String.IsNullOrEmpty(result) || (!result.Contains(LoggedInText)))
            {
                return false;
            }

            _cookies.Clear();

            var coockiesForDomain = cookies.GetCookies(new Uri(DomainPrefix));

            for (int j = 0; j < coockiesForDomain.Count; j++) 
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

                _cookies.Add( oC );
            }

            _context = cookies;

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
