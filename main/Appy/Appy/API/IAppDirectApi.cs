using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.API
{
    public interface IAppDirectApi
    {
        MyappsMyapp[] MyApps { get; }
        WebApplicationsListApplication[] SuggestedApps { get; }
        AppDirectSession Session { get; }
        bool IsAuthenticated { get; }
        bool Authenticate(string key, string secret);
        void UnAuthenticate();

        bool RegisterUser(string email, string firstName, string lastName, 
            string password, string confirmPassword, string companyName, 
            string industryId, string companySize, string phone);
        bool ConfirmUserEmail(string email, string confirmationCode);
        bool IsEmailConfirmed(string email);
    }
}