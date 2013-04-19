using AppDirect.WindowsClient.API.VO;
using AppDirect.WindowsClient.Common.API;
using System.Collections.Generic;

namespace AppDirect.WindowsClient.API
{
    public interface ICachedAppDirectApi
    {
        IList<Application> MyApps { get; }

        IList<Application> SuggestedApps { get; }

        AppDirectSession Session { get; }

        bool IsAuthenticated { get; }

        /// <summary>
        /// Information about the user and company
        /// </summary>
        UserInfo UserInfo { get; }

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

        /// <summary>
        /// Gets free subscription plan ID which can used to singup user for this applicaiton for free.
        /// </summary>
        /// <param name="applicationId">Application ID</param>
        /// <returns>Payment plan ID for free plan</returns>
        string GetFreeSubscriptionPlanId(string applicationId);

        /// <summary>
        /// Get pricingPlanId from GetFreeSubscriptionPlanId()
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="companyId"></param>
        /// <param name="pricingPlanId"></param>
        /// <returns>subscription ID</returns>
        string ProvisionApplication(string userId, string companyId, string pricingPlanId);

        /// <summary>
        /// Get subscriptionId from ProvisionApplication()
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns>true if success</returns>
        bool DeprovisionApplication(string subscriptionId);
    }
}