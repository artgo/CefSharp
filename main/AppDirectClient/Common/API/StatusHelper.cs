using System;

namespace AppDirect.WindowsClient.Common.API
{
    public enum DisplayStatus
    {
        Active = 0,
        ApiCallInProgress = 1,
        PendingAddition = 2,
        PendingRemoval = 4,
        Cancelled = 8
    }

    public enum ApiStatus
    {
        ACTIVE = 0,
        PENDING_REMOTE_CREATION = 2,
        PENDING_REMOTE_CANCELLATION = 4,
        FAILED = 6,
        CANCELLED = 8,
        UNKNOWN = 25
    }

    public enum ApiSubscriptionStatus
    {
        ACTIVE = 0,
        PENDING_REMOTE_CREATION = 2,
        FREE_TRIAL = 3,
        PENDING_REMOTE_CANCELLATION = 4,
        FREE_TRIAL_EXPIRED = 5,
        FAILED = 6,
        SUSPENDED = 7,
        CANCELLED = 8,
        UNKNOWN = 25
    }

    public static class StatusHelper
    {
        /// <summary>
        /// Resisted using bitwise flags for readability
        /// </summary>
        /// <param name="statusString"></param>
        /// <param name="subscriptionStatusString"></param>
        /// <returns></returns>
        public static DisplayStatus ConvertToDisplayStatus(ApiStatus? status, ApiSubscriptionStatus? subscriptionStatus)
        {
            if (subscriptionStatus >= ApiSubscriptionStatus.FREE_TRIAL_EXPIRED || status >= ApiStatus.FAILED)
            {
                return DisplayStatus.Cancelled;
            }

            if (subscriptionStatus >= ApiSubscriptionStatus.PENDING_REMOTE_CANCELLATION || status >= ApiStatus.PENDING_REMOTE_CANCELLATION)
            {
                return DisplayStatus.PendingRemoval;
            }


            if (subscriptionStatus >= ApiSubscriptionStatus.PENDING_REMOTE_CREATION || status >= ApiStatus.PENDING_REMOTE_CREATION)
            {
                return DisplayStatus.PendingAddition;
            }

            return DisplayStatus.Active;
        }
    }
}