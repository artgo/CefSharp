using System;
using System.Runtime.Serialization;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.Models
{
    ///<summary>
    /// Represents an Application of the sort that AppDirect distributes 
    ///</summary>
    [Serializable]
    public class Application : IApplication
    {
        public override string Id { get; set; }
        public override string Name { get; set; }
        public override string ImagePath { get; set; }
        public override string LocalImagePath { get; set; }
        public override string Description { get; set; }
        public override int AlertCount { get; set; }
        public override bool IsLocalApp { get; set; }
        public override string UrlString { get; set; }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(this, null)) return false;
            if (ReferenceEquals(obj, null)) return false;
            if (this.GetType() != obj.GetType()) return false;

            var application = obj as Application;

            if (application == null)
            {
                return false;
            }

            return string.Equals(this.Id, application.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
