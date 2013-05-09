namespace AppDirect.WindowsClient.Common.API
{
    public enum Status
    {
        Active = 0,
        Unknown = 1,
        ApiCallInProgress = 2,
        Pending_Remote_Creation = 3,
        Deprovisioning = 4,
        NotActive = 5,
        Cancelled = 6
    }
}