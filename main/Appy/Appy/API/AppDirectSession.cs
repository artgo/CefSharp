using System;
using System.Collections.Generic;
using System.Net;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.API
{
    public class AppDirectSession : IAppDirectSession
    {
        private readonly DateTime _lastAccessed;
        private readonly IEnumerable<Cookie> _cookies;

        internal AppDirectSession(DateTime lastAccessed, IEnumerable<Cookie> cookies)
        {
            _lastAccessed = lastAccessed;
            _cookies = cookies;
        }

        public bool HasExpired
        {
            get { return (DateTime.Now - _lastAccessed) > (new TimeSpan(0, 1, 0, 0)); }
        }

        public IEnumerable<Cookie> Cookies
        {
            get { return _cookies; }
        }
    }
}
