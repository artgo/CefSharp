using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.API.Subscription
{
    public static class StatusHelper
    {
        private static HashSet<ApiUserAssingmentStatus> _cancelledApiStatuses = new HashSet<ApiUserAssingmentStatus>() { ApiUserAssingmentStatus.FAILED, ApiUserAssingmentStatus.CANCELLED };
        private static HashSet<ApiSubscriptionStatus> _cancelledApiSubscriptionStatuses = new HashSet<ApiSubscriptionStatus>() { ApiSubscriptionStatus.FREE_TRIAL_EXPIRED, ApiSubscriptionStatus.SUSPENDED, ApiSubscriptionStatus.FAILED, ApiSubscriptionStatus.CANCELLED };
        private static HashSet<ApiSubscriptionStatus> _validSubscriptionStatuses = new HashSet<ApiSubscriptionStatus>() { ApiSubscriptionStatus.ACTIVE, ApiSubscriptionStatus.SUSPENDED, ApiSubscriptionStatus.FAILED, ApiSubscriptionStatus.CANCELLED };

        /// <summary>
        /// Resisted using bitwise flags for readability
        /// </summary>
        /// <param name="statusString"></param>
        /// <param name="subscriptionStatusString"></param>
        /// <returns></returns>
        public static DisplayStatus ConvertToDisplayStatus(string statusString, string subscriptionStatusString)
        {
            var subscriptionStatus = ApiSubscriptionStatusFromString(subscriptionStatusString);
            var status = ApiStatusFromString(statusString);

            var convertedSubscriptionStatus = DisplayStatus.Cancelled;

            switch (subscriptionStatus)
            {
                case ApiSubscriptionStatus.ACTIVE:
                case ApiSubscriptionStatus.FREE_TRIAL:
                    convertedSubscriptionStatus = DisplayStatus.Active;
                    break;
                case ApiSubscriptionStatus.PENDING_REMOTE_CREATION:
                case ApiSubscriptionStatus.INITIALIZED:
                    convertedSubscriptionStatus = DisplayStatus.SubscriptionPendingAddition;
                    break;
                default://Apps in all other status should not appear
                    return DisplayStatus.Cancelled;
            }

            switch (status)
            {
                case ApiUserAssingmentStatus.ACTIVE:
                    return convertedSubscriptionStatus;
                case ApiUserAssingmentStatus.PENDING_REMOTE_CANCELLATION:
                    return DisplayStatus.UserPendingRemoval;
                case ApiUserAssingmentStatus.PENDING_REMOTE_CREATION:
                case ApiUserAssingmentStatus.PENDING_USER_ACTIVATION:
                    {
                        switch (convertedSubscriptionStatus)
                        {
                            case DisplayStatus.SubscriptionPendingAddition:
                                return DisplayStatus.SubscriptionPendingAddition;
                            case DisplayStatus.Active:
                                return DisplayStatus.UserPendingAddition;
                        }
                        break;
                    }
                default:
                    return DisplayStatus.Cancelled;
            }
            
            return DisplayStatus.Cancelled;
        }

        public static ApiUserAssingmentStatus ApiStatusFromString(string status)
        {
            try
            {
                return (ApiUserAssingmentStatus)Enum.Parse(typeof(ApiUserAssingmentStatus), status);
            }
            catch (ArgumentException)
            {
                return ApiUserAssingmentStatus.UNKNOWN;
            }
        }

        public static ApiSubscriptionStatus ApiSubscriptionStatusFromString(string status)
        {
            try
            {
                return (ApiSubscriptionStatus)Enum.Parse(typeof(ApiSubscriptionStatus), status);
            }
            catch (ArgumentException)
            {
                return ApiSubscriptionStatus.UNKNOWN;
            }
        }

        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}