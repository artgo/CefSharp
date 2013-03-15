using System.ComponentModel;
using System.Windows;

namespace AppDirect.WindowsClient.UI
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private Visibility _isVisible = Visibility.Collapsed;
        private string _loginFailedMessage = Properties.Resources.CredentialsProblemError;
        private string _loginHeaderText = Properties.Resources.LoginHeaderDefault;

        public Visibility IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyPropertyChanged("IsVisible");
            }
        }
        
        public string LoginFailedMessage
        {
            get { return _loginFailedMessage; }
            set
            {
                _loginFailedMessage = value;
                NotifyPropertyChanged("LoginFailedMessage");
            }
        }

        public string LoginHeaderText
        {
            get { return _loginHeaderText; }
            set
            {
                _loginHeaderText = value;
                NotifyPropertyChanged("LoginHeaderText");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool Login(string username, string password)
        {
            if (ServiceLocator.CachedAppDirectApi.Authenticate(username, password))
            {
                lock (ServiceLocator.LocalStorage.Locker)
                {
                    ServiceLocator.LocalStorage.SetCredentials(username, password);
                }
                
                return true;
            }

            return false;
        }
    }
}