using System.Collections.ObjectModel;

namespace MainApplication
{
    public interface ICachedAppDirectApi
    {
        ObservableCollection<Application> MyApps { get; }
        ObservableCollection<Application> SuggestedApps { get; }
        bool IsAuthenticated { get; }
        void Authenticate(string key, string secret);
    }
}