using System;
using System.Runtime.Serialization;
using AppDirect.WindowsClient.Models;

namespace AppDirect.WindowsClient.Common.API
{
    [DataContract]
    [KnownType(typeof (Application))]
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
    }
}