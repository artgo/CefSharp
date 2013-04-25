using System;

namespace AppDirect.WindowsClient.API.VO
{
    [Serializable]
    public enum CompanyEntitlementStatus
    {
	    PENDING_REMOTE_CREATION,
	    FAILED,
	    FREE_TRIAL,
	    FREE_TRIAL_EXPIRED,
	    ACTIVE,
	    SUSPENDED,
	    PENDING_REMOTE_CANCELLATION,
	    CANCELLED
    }
}