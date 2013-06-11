namespace AppDirect.WindowsClient.API.Subscription
{
    public enum ApiSubscriptionStatus
    {
        ACTIVE,
        INITIALIZED,
        FAILED,
        FREE_TRIAL,
        FREE_TRIAL_EXPIRED,
        SUSPENDED,
        CANCELLED,
        // Old deprecated value
        PENDING_REMOTE_CREATION,
        // Old deprecated value
        PENDING_REMOTE_CANCELLATION,
        UNKNOWN
    }
}