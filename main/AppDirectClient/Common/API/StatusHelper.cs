using System.Collections.Generic;

namespace AppDirect.WindowsClient.Common.API
{
    public static class StatusHelper
    {
        private static HashSet<ApiStatus> _cancelledApiStatuses = new HashSet<ApiStatus>() { ApiStatus.FAILED, ApiStatus.CANCELLED };
        private static HashSet<ApiSubscriptionStatus> _cancelledApiSubscriptionStatuses = new HashSet<ApiSubscriptionStatus>() { ApiSubscriptionStatus.FREE_TRIAL_EXPIRED, ApiSubscriptionStatus.SUSPENDED, ApiSubscriptionStatus.FAILED, ApiSubscriptionStatus.CANCELLED };

        /// <summary>
        /// Resisted using bitwise flags for readability
        /// </summary>
        /// <param name="statusString"></param>
        /// <param name="subscriptionStatusString"></param>
        /// <returns></returns>
        public static DisplayStatus ConvertToDisplayStatus(ApiStatus? status, ApiSubscriptionStatus? subscriptionStatus)
        {
            if (status == null || subscriptionStatus == null || _cancelledApiSubscriptionStatuses.Contains(subscriptionStatus.Value) || _cancelledApiStatuses.Contains(status.Value))
            {
                return DisplayStatus.Cancelled;
            }

            if (subscriptionStatus == ApiSubscriptionStatus.PENDING_REMOTE_CANCELLATION || status == ApiStatus.PENDING_REMOTE_CANCELLATION)
            {
                return DisplayStatus.PendingRemoval;
            }

            if (subscriptionStatus == ApiSubscriptionStatus.PENDING_REMOTE_CREATION || status == ApiStatus.PENDING_REMOTE_CREATION)
            {
                return DisplayStatus.PendingAddition;
            }

            return DisplayStatus.Active;
        }
    }
}