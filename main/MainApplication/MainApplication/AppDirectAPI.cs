using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using com.appdirect.WindowsClient;
using com.appdirect.WindowsClient.DataAccess;

namespace MainApplication
{
    public class AppDirectApi
    {
        private static readonly AppDirectApi instance = new AppDirectApi();
        private static readonly Uri ServiceUri = new Uri("https://test.appdirect.com/api/marketplace/v1/listing?filter=FEATURED");
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private readonly WebClient _serviceRequest = new WebClient();

        private AppDirectApi() { }

        public static AppDirectApi Instance
        {
            get
            {
                return instance;
            }
        }

        public WebApplicationsListApplication[] MyApps
        {
            get {
                try
                {
                    string result = _serviceRequest.DownloadString(ServiceUri);
                    return _serializer.Deserialize<WebApplicationsListApplication[]>(result);
                }
                catch (Exception e)
                {
                    return null;
                }

            }
        }
        public WebApplicationsListApplication[] SuggestedApps
        {
            get
            {
                string result = _serviceRequest.DownloadString(ServiceUri);
                return _serializer.Deserialize<WebApplicationsListApplication[]>(result);
            }
        }
    }
}
