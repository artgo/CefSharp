using System;
using System.Collections.Generic;

namespace AppDirect.WindowsClient.Models
{
    ///<summary>
    /// Represents an Application of the sort that AppDirect distributes 
    ///</summary>
    [Serializable]
    public class Application
    {
        public string Id { get; set; } 
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string LocalImagePath { get; set; }
        public string Description { get; set; }
        public int AlertCount { get; set; }
        public bool IsLocalApp { get; set; }
        public string UrlString { get; set; }

        public override bool Equals(System.Object y)
        {
            if (ReferenceEquals(this, y)) return true;
            if (ReferenceEquals(this, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (this.GetType() != y.GetType()) return false;

            Application yApp = y as Application;

            if (yApp == null)
            {
                return false;
            }

            return string.Equals(this.Id, yApp.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
