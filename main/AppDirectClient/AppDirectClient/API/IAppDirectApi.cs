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

        bool RegisterUser(string firstName, string lastName, string password, string confirmPassword,
            string email, string confirmEmail, string companyName, string phone, string industryId, string companySize);
        bool ConfirmUserEmail(string email, string confirmationCode);
        bool IsEmailConfirmed(string email);
    }
}