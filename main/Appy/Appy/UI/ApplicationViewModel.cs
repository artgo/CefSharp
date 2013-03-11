using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.UI
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public ApplicationViewModel(Application application)
        {
            Application = application;
        }

        public Application Application { get; set; }

        public bool PinnedToTaskbarNotifier
        {
            get { return Application.PinnedToTaskbar; }
            set
            {
                Application.PinnedToTaskbar = value;
                NotifyPropertyChanged("PinnedToTaskbarNotifier");
            }
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
