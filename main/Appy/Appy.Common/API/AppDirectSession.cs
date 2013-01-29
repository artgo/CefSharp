using System;
using System.Collections.Generic;
using System.Net;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.API
{
    [Serializable]
    public class AppDirectSession : IAppDirectSession
    {
        public override DateTime ExpirationDate { get; set; }
        public override IList<Cookie> Cookies { get; set; }
    }
}
