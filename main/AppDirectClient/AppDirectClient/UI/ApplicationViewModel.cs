using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using AppDirect.WindowsClient.Common.API;
using Application = AppDirect.WindowsClient.Common.API.Application;

namespace AppDirect.WindowsClient.UI
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public ApplicationViewModel(Application application)
        {
            Application = application;
        }

        public Application Application { get; set; }

        public Status ApplicationStatus
        {
            get { return Application.Status; }
            set
            {
                Application.Status = value;
                NotifyPropertyChanged("ApplicationStatus");
            }
        }

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
        private Visibility _inProgressVisibility = Visibility.Collapsed;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
