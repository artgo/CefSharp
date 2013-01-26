using System.Collections.Generic;
using AppDirect.WindowsClient.Models;

namespace AppDirect.WindowsClient.API
{
    public interface ICachedAppDirectApi
    {
        IList<Application> MyApps { get; }
        IList<Application> SuggestedApps { get; }
        AppDirectSession Session { get; }
        bool IsAuthenticated { get; }
        bool Authenticate(string key, string secret);
        void UnAuthenticate();
    }
}