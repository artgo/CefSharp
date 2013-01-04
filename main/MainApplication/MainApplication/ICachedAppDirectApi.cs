using System.Collections.Generic;
using AppDirect.WindowsClient.Models;

namespace AppDirect.WindowsClient
{
    public interface ICachedAppDirectApi
    {
        IList<Application> MyApps { get; }
        IList<Application> SuggestedApps { get; }
        bool IsAuthenticated { get; }
        void Authenticate(string key, string secret);
    }
}