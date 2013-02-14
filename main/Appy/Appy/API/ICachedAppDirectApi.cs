using System.Collections.Generic;
using AppDirect.WindowsClient.Common.API;

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

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <param name="email"></param>
        /// <param name="confirmEmail"></param>
        /// <param name="companyName"></param>
        /// <returns>true if success</returns>
        bool RegisterUser(string firstName, string lastName, string password, string confirmPassword, 
            string email, string confirmEmail, string companyName);

        /// <summary>
        /// Enter confirmation code received by user in email to confirm email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="confirmationCode"></param>
        /// <returns>true if succeded</returns>
        bool ConfirmUserEmail(string email, string confirmationCode);

        /// <summary>
        /// Check if email was confirmed by user by clicking on email confirmation link
        /// </summary>
        /// <param name="email"></param>
        /// <returns>true if confirmed</returns>
        bool IsEmailConfirmed(string email);
    }
}