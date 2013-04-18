using System;
using System.ComponentModel;
using System.Windows;
using AppDirect.WindowsClient.Properties;

namespace AppDirect.WindowsClient.UI
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private Visibility _isVisible = Visibility.Collapsed;
        private string _loginFailedMessage = Resources.CredentialsProblemError;
        private string _loginHeaderText = Resources.LoginHeaderLoginRequired;
        private string _errorMessage;
        private bool _loginInProgress;

        public Visibility IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyPropertyChanged("IsVisible");
            }
        }

        public Visibility BackButtonVisibility
        {
            get { return Visibility.Collapsed; }
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

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged("ErrorMessage");
            }
        }

        public bool LoginInProgress
        {
            get { return _loginInProgress; }
            set
            {
                _loginInProgress = value;
                NotifyPropertyChanged("LoginInProgress");
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

        public void SetVisibility(bool isLoggedIn)
        {
            IsVisible = isLoggedIn ? Visibility.Collapsed : Visibility.Visible;
        }

        public void ShowNetworkProblem()
        {
            ErrorMessage = Resources.NetworkProblemError;
        }

        public void ClearErrorMessage()
        {
            ErrorMessage = String.Empty;
        }
    }
}