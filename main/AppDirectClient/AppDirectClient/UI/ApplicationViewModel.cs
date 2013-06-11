using AppDirect.WindowsClient.Common.API;
using System.ComponentModel;
using System.Windows;
using Application = AppDirect.WindowsClient.Common.API.Application;

namespace AppDirect.WindowsClient.UI
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public ApplicationViewModel(Application application)
        {
            Application = application;
            DisplayContextMenu = application.HideContextMenu ? Visibility.Hidden : Visibility.Visible;
            IsUnsupported = UnsupportedApps.IsUnsupported(application);
        }

        public Application Application { get; set; }

        public DisplayStatus ApplicationStatus
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

        public Visibility DisplayContextMenu { get; set; }

        public bool IsUnsupported { get; set; }

        public bool IsEnabled
        {
            get { return !IsUnsupported && (ApplicationStatus == DisplayStatus.Active); }
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