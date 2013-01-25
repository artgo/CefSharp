using System.Collections.Generic;
using System.Net;

namespace AppDirect.WindowsClient.Common.API
{
    public interface IAppDirectSession
    {
        bool HasExpired { get; }
        IEnumerable<Cookie> Cookies { get; }
    }
}