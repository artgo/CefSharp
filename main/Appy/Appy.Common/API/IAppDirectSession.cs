using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using AppDirect.WindowsClient.API;

namespace AppDirect.WindowsClient.Common.API
{
    [DataContract]
    [KnownType(typeof(AppDirectSession))]
    [Serializable]
    public abstract class IAppDirectSession
    {
        [DataMember]
        public abstract DateTime ExpirationDate { get; set;  }
        [DataMember]
        public abstract IList<Cookie> Cookies { get; set; }
    }
}