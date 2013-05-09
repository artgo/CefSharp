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
}