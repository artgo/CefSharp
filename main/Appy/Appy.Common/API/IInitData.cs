using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AppDirect.WindowsClient.Common.API
{
    [DataContract]
    [KnownType(typeof(InitData))]
    [Serializable]
    public abstract class IInitData
    {
        [DataMember]
        public virtual IAppDirectSession Session { get; set; }

        [DataMember]
        public virtual IEnumerable<IApplication> Applications { get; set; }
    }
}