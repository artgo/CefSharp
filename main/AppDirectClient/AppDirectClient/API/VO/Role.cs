using System;

namespace AppDirect.WindowsClient.API.VO
{
    [Serializable]
    public enum Role
    {
        USER,
        APPLICATION,
        PARTNER,

        BILLING_ADMIN,
        SYS_ADMIN,
        DEVELOPER,
        CHANNEL_PRODUCT_SUPPORT,
        CHANNEL_SUPPORT,
        CHANNEL_ADMIN,
        SUPERUSER,
        ANALYTICS_ADMIN
    }
}