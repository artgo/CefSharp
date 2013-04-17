using System;
using System.Collections.Generic;
using System.Net;

namespace AppDirect.WindowsClient.Common.API
{
    [Serializable]
    public class AppDirectSession : IAppDirectSession
    {
        public override DateTime ExpirationDate { get; set; }
        public override IList<Cookie> Cookies { get; set; }
    }
}
