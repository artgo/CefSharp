using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.Common.API
{
    ///<summary>
    /// Represents an Application of the sort that AppDirect distributes 
    ///</summary>
    [Serializable]
    [XmlInclude(typeof(Application)), XmlInclude(typeof(LocalApplication))]
    public class Application : IApplication, INotifyPropertyChanged
    {
        public override string Id { get; set; }
        public override string Name { get; set; }
        public override string ImagePath { get; set; }
        public override string LocalImagePath { get; set; }
        public override string Description { get; set; }
        public override int AlertCount { get; set; }
        public override bool IsLocalApp { get; set; }
        public override string UrlString { get; set; }
        public override string Price { get; set; }
        public override int BrowserHeight { get; set; }
        public override int BrowserWidth { get; set; }
        public override bool BrowserResizable { get; set; }

        /// <summary>
        /// Public only for XML - use PinnedToTaskbarPublisher to set value
        /// </summary>
        public bool PinnedToTaskbar { get; set; }

        [XmlIgnore]
        public bool PinnedToTaskbarPublisher
        {
            get { return PinnedToTaskbar; }
            set
            {
                PinnedToTaskbar = value;
                NotifyPropertyChanged("PinnedToTaskbarPublisher");
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
