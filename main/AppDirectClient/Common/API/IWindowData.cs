using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.Common.API
{
    [DataContract]
    [KnownType(typeof(WindowData))]
    [Serializable]
    public abstract class IWindowData
    {
        [DataMember]
        public virtual string ApplicationId { get; set; }

        [DataMember]
        public virtual WindowState WindowState { get; set; }
    }
}