using System.Linq;
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
            if (IsUnsupported)
            {
                application.PinnedToTaskbar = false;
            }
        }

        public Application Application { get; private set; }

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
                ServiceLocator.LocalStorage.InstalledAppDirectApps.FirstOrDefault(a => a.Equals(Application)).PinnedToTaskbar = value;
                ServiceLocator.LocalStorage.SaveAppSettings();
            }
        }

        public Visibility DisplayContextMenu { get; private set; }

        public bool IsUnsupported { get; private set; }

        public bool IsEnabled
        {
            get { return !IsUnsupported && (ApplicationStatus == DisplayStatus.Active); }
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