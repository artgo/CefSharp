using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient
{
    public static class StatusHelper
    {
        private static HashSet<ApiStatus> _cancelledApiStatuses = new HashSet<ApiStatus>() { ApiStatus.FAILED, ApiStatus.CANCELLED };
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

            DisplayStatus convertedSubscriptionStatus = DisplayStatus.Cancelled;

            switch (subscriptionStatus)
            {
                case ApiSubscriptionStatus.ACTIVE:
                case ApiSubscriptionStatus.FREE_TRIAL:
                    convertedSubscriptionStatus = DisplayStatus.Active;
                    break;
                case ApiSubscriptionStatus.PENDING_REMOTE_CREATION:
                case ApiSubscriptionStatus.PENDING:
                    convertedSubscriptionStatus = DisplayStatus.SubscriptionPendingAddition;
                    break;
                default://Apps in all other status should not appear
                    return DisplayStatus.Cancelled;
            }

            switch (status)
            {
                case ApiStatus.ACTIVE:
                    return convertedSubscriptionStatus;
                case ApiStatus.PENDING_REMOTE_CANCELLATION:
                    return DisplayStatus.UserPendingRemoval;
                case ApiStatus.PENDING_REMOTE_CREATION:
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

        public static ApiStatus ApiStatusFromString(string status)
        {
            try
            {
                return (ApiStatus)Enum.Parse(typeof(ApiStatus), status);
            }
            catch (ArgumentException)
            {
                return ApiStatus.UNKNOWN;
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