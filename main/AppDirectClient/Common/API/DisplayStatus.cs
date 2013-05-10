using System.ComponentModel;

namespace AppDirect.WindowsClient.Common.API
{
    public enum DisplayStatus
    {
        Active,
        [Description("In Progress")]
        ApiCallInProgress,
        [Description("Your account is being provisioned")]
        UserPendingAddition,
        [Description("Your account is being deprovisioned")]
        UserPendingRemoval,
        [Description("Your company subscription is being provisioned")]
        SubscriptionPendingAddition,
        Cancelled
    }
}