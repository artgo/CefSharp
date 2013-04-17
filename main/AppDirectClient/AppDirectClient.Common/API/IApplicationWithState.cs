using System;
using System.Runtime.Serialization;
using System.Windows;

namespace AppDirect.WindowsClient.Common.API
{
    [DataContract]
    [KnownType(typeof(ApplicationWithState))]
    [Serializable]
    public abstract class IApplicationWithState
    {
        [DataMember]
        public virtual IApplication Application { get; set; }

        [DataMember]
        public virtual WindowState WindowState { get; set; }
    }
}