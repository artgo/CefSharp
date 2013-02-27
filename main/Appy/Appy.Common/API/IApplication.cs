using System;
using System.Runtime.Serialization;

namespace AppDirect.WindowsClient.Common.API
{
    [DataContract]
    [KnownType(typeof(Application))]
    [KnownType(typeof(LocalApplication))]
    [Serializable]
    public abstract class IApplication
    {
        [DataMember]
        public abstract string Id { get; set; }

        [DataMember]
        public abstract string Name { get; set; }

        [DataMember]
        public abstract string ImagePath { get; set; }

        [DataMember]
        public abstract string LocalImagePath { get; set; }

        [DataMember]
        public abstract string Description { get; set; }

        [DataMember]
        public abstract int AlertCount { get; set; }

        [DataMember]
        public abstract bool IsLocalApp { get; set; }

        [DataMember]
        public abstract string UrlString { get; set; }

        [DataMember]
        public abstract string Price { get; set; }

        [DataMember]
        public abstract int BrowserHeight { get; set; }
        
        [DataMember]
        public abstract int BrowserWidth { get; set; }
        
        [DataMember]
        public abstract bool BrowserResizable { get; set; }
    }
}