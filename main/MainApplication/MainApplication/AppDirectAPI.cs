using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Xml.Serialization;
using AppDirect.WindowsClient.ObjectMapping;
using System.Web;
using System.Web.Script.Serialization;

namespace AppDirect.WindowsClient
{
    public class AppDirectApi : IAppDirectApi
    {
        private const string JSessionIdParamName = "JSESSIONID";
        private const string InitialSessionIdValue = "2E11DE721BAF465953CAFC0407F8F448-n1";
        private const string DomainName = @"test.appdirect.com";
        private const string DomainPrefix = @"https://" + DomainName;
        private const string LoginParams = "id2_hf_0=&_spring_security_remember_me=on&email={0}&password={1}&signin";
        private const string MyAppsUrl = DomainPrefix + @"/api/account/v1/myapps.json";
        private const string FormContentType = "application/x-www-form-urlencoded";
        private const string JsonAcceptString = "application/json,text/javascript,*/*;q=0.01";
        private const string HtmlAcceptString = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        private static readonly Uri ServiceUriSuggested = new Uri(DomainPrefix + @"/api/marketplace/v1/listing?filter=FEATURED");
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private readonly WebClient _serviceRequest = new WebClient();
        private DateTime _time = DateTime.Now;
        private CookieContainer _context = null;

        public MyappsMyapp[] MyApps
        {
            get {
                if (IsAuthenticated)
                {
                    var request = BuildHttpWebRequestForUrl(MyAppsUrl, false, true);
                    request.CookieContainer = _context;

                    HttpWebResponse response = (HttpWebResponse) request.GetResponse();

                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string result = reader.ReadToEnd();
                    return _serializer.Deserialize<MyappsMyapp[]>(result);
                }

                return null;
            }
        }

        private HttpWebRequest BuildHttpWebRequestForUrl(string urlStr, bool isPost, bool isJson)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest) HttpWebRequest.Create(urlStr);
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:17.0) Gecko/20100101 Firefox/17.0";
            httpWebRequest.Method = isPost ? "POST" : "GET";
            httpWebRequest.Accept = isJson ? JsonAcceptString : HtmlAcceptString;
            httpWebRequest.Headers.Add("Accept-Language: en-US,en;q=0.5");
            httpWebRequest.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
            httpWebRequest.KeepAlive = true;
            httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.Headers.Add("Keep-Alive: 300");
            httpWebRequest.Referer = DomainPrefix + @"/login";
            return httpWebRequest;
        }

        public WebApplicationsListApplication[] SuggestedApps
        {
            get
            {
                string result = _serviceRequest.DownloadString(ServiceUriSuggested);
                return _serializer.Deserialize<WebApplicationsListApplication[]>(result);
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                if ((_context != null) && (DateTime.Now - _time) > (new TimeSpan(0, 1, 0, 0)))
                {
                    UnAuthenticate();
                }
                return _context != null;
            }
        }

        public bool Authenticate(string key, string secret)
        {
            HttpWebRequest request = BuildHttpWebRequestForUrl(DomainPrefix + @"/login?1434449477-1.IFormSubmitListener-loginpanel-signInForm", true, false);
            var cookies = new CookieContainer();
            cookies.Add(new Cookie(JSessionIdParamName, InitialSessionIdValue, "/", DomainName));
            request.CookieContainer = cookies;
            request.ContentType = FormContentType;

            string encodedKey = HttpUtility.UrlEncode(key);

            StreamWriter sw = new StreamWriter(request.GetRequestStream());
            sw.Write(String.Format(LoginParams, encodedKey, secret));
            sw.Close();

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string result = reader.ReadToEnd();
            if ((response.StatusCode != HttpStatusCode.OK) || String.IsNullOrEmpty(result) || (!result.Contains("myAppsPage")))
            {
                return false;
            }

            for ( int j = 0; j < response.Cookies.Count; j++ ) 
            {
                var oCookie = response.Cookies[j];
                var oC = new Cookie();

                // Convert between the System.Net.Cookie to a System.Web.HttpCookie...
                oC.Domain   = request.RequestUri.Host;
                oC.Expires  = oCookie.Expires;
                oC.Name     = oCookie.Name;
                oC.Path     = oCookie.Path;
                oC.Secure   = oCookie.Secure;
                oC.Value    = oCookie.Value;

                cookies.Add( oC );
            }

            _time = DateTime.Now;
            _context = cookies;

            return true;
        }

        public void UnAuthenticate()
        {
            _context = null;
        }
    }
}
