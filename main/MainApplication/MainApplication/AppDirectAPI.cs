using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using com.appdirect.WindowsClient;

namespace MainApplication
{
    public class AppDirectApi
    {
        private static readonly AppDirectApi instance = new AppDirectApi();
        private static readonly Uri ServiceUri = new Uri("https://load.appdirect.com/api/marketplace/v1/listing?filter=FEATURED");
        private JavaScriptSerializer serializer = new JavaScriptSerializer();
        private WebClient serviceRequest = new WebClient();

        private AppDirectApi() { }

        public static AppDirectApi Instance
        {
            get
            {
                return instance;
            }
        }

        public applicationsApplication[] MyApps
        {
            get { 
                string result = serviceRequest.DownloadString(ServiceUri);
                return serializer.Deserialize<applicationsApplication[]>(result);
            }
        }
        public applications SuggestedApps
        {
            get
            {
                string result = serviceRequest.DownloadString(ServiceUri);
                return serializer.Deserialize<applications>(result);
            }
        }
    }
}
