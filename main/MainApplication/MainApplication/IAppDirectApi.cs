using AppDirect.WindowsClient.ObjectMapping;

namespace AppDirect.WindowsClient
{
    public interface IAppDirectApi
    {
        MyappsMyapp[] MyApps { get; }
        WebApplicationsListApplication[] SuggestedApps { get; }
        bool IsAuthenticated { get; }
        void Authenticate(string key, string secret);
    }
}