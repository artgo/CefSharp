namespace AppDirect.WindowsClient.Common.API
{
    public enum ApiSubscriptionStatus
    {
        ACTIVE,
        PENDING_REMOTE_CREATION,
        FREE_TRIAL,
        PENDING_REMOTE_CANCELLATION,
        FREE_TRIAL_EXPIRED,
        FAILED,
        SUSPENDED,
        CANCELLED
    }
}