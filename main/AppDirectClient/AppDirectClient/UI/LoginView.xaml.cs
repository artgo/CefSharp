using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.Log;
using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public EventHandler RegistrationClick;
        public EventHandler CloseLogin;
        public EventHandler LoginSuccessfulNotifier;

        private readonly SolidColorBrush _errorColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#a32424"));
        private readonly SolidColorBrush _defaultColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#072b35"));
        private readonly SolidColorBrush _validColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#072b35"));

        private static readonly ILogger _log = new NLogLogger("LoginView");

        public LoginViewModel ViewModel
        {
            get { return DataContext as LoginViewModel; }
        }

        public LoginView()
        {
            InitializeComponent();
            SetFocusField();
        }

        private void LoginFailed()
        {
            UsernameTextBox.Focus();
            UsernameTextBox.Background = _errorColorBrush;
            PasswordBox.Background = _errorColorBrush;
            LoginFailedMessage.Visibility = Visibility.Visible;
        }

        private void ClearLoginFailed()
        {
            if (EmailFormatErrorMessage.Visibility != Visibility.Visible)
            {
                UsernameTextBox.Background = _validColorBrush;
            }

            if (LoginFailedMessage.Visibility == Visibility.Visible)
            {
                PasswordBox.Background = _validColorBrush;
                LoginFailedMessage.Visibility = Visibility.Hidden;
            }
        }

        private void UsernameTextBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                UsernameTextBox.Background = _validColorBrush;
                LoginButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void UsernameTextBox_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ClearLoginFailed();
            CheckUsername();
        }

        private void UsernameTextBox_OnFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            UsernameTextBox.Background = _validColorBrush;
            EmailFormatErrorMessage.Visibility = Visibility.Hidden;
            ClearLoginFailed();
        }

        private void UsernameTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (EmailFormatErrorMessage.Visibility == Visibility.Visible ||
                LoginFailedMessage.Visibility == Visibility.Visible)
            {
                ClearLoginFailed();
                CheckUsername();
            }
        }

        private void CheckUsername()
        {
            if (String.IsNullOrEmpty(UsernameTextBox.Text))
            {
                UsernameTextBox.Background = _defaultColorBrush;
                EmailFormatErrorMessage.Visibility = Visibility.Hidden;
            }
            else if (Helper.EmailMatchPattern.IsMatch(UsernameTextBox.Text))
            {
                UsernameTextBox.Background = _validColorBrush;
                EmailFormatErrorMessage.Visibility = Visibility.Hidden;
            }
            else
            {
                UsernameTextBox.Background = _errorColorBrush;
                EmailFormatErrorMessage.Visibility = Visibility.Visible;
            }
        }

        private void Password_OnFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            PasswordBox.Background = _validColorBrush;
            ClearLoginFailed();
            CheckUsername();
        }

        private void Password_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;

            if (String.IsNullOrEmpty(passwordBox.Password))
            {
                passwordBox.Background = _defaultColorBrush;
            }
        }

        private void PasswordBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            ClearLoginFailed();
            CheckUsername();

            if (e.Key == Key.Return)
            {
                LoginButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ClearLoginFailed();
            CheckUsername();
        }

        private void ForgotPassword(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Resources.ForgotPasswordUrlString);
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            if (!ValidateRequiredFields())
            {
                return;
            }

            ViewModel.LoginInProgress = true;

            BackgroundWorker loginBW = new BackgroundWorker();
            loginBW.DoWork += LoginTask;
            loginBW.RunWorkerCompleted += LoginComplete;

            ServiceLocator.LocalStorage.SetCredentials(UsernameTextBox.Text, PasswordBox.Password);

            loginBW.RunWorkerAsync();
        }

        private void LoginTask(object sender, DoWorkEventArgs e)
        {
            e.Result = Helper.Authenticate();
        }

        private void LoginComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null && e.Error.GetType() == typeof(WebException))
            {
                _log.ErrorException("Connection error", e.Error);
                ViewModel.ShowNetworkProblem();
            }
            else if ((bool)e.Result)
            {
                LoginFailedMessage.Visibility = Visibility.Hidden;
                LoginSuccessfulNotifier.Invoke(sender, e);
                CloseLogin.Invoke(sender, e);
            }
            else
            {
                LoginFailed();
            }

            ViewModel.LoginInProgress = false;
        }

        private bool ValidateRequiredFields()
        {
            bool passwordIsBad = String.IsNullOrEmpty(PasswordBox.Password);
            bool emailIsBad = String.IsNullOrEmpty(UsernameTextBox.Text) ||
                              !Helper.EmailMatchPattern.IsMatch(UsernameTextBox.Text);

            if (emailIsBad)
            {
                UsernameTextBox.Focus();
            }
            else if (passwordIsBad)
            {
                PasswordBox.Focus();
            }

            if (passwordIsBad)
            {
                PasswordBox.Background = _errorColorBrush;
            }

            if (emailIsBad)
            {
                UsernameTextBox.Background = _errorColorBrush;
                EmailFormatErrorMessage.Visibility = Visibility.Visible;
            }

            return !(passwordIsBad || emailIsBad);
        }

        private void CancelLoginClick(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = string.Empty;
            CloseLogin.Invoke(sender, e);
        }

        public void GoToRegistrationClick(object sender, EventArgs eventArgs)
        {
            ServiceLocator.BrowserWindowsCommunicator.DisplayApplicationWithoutSession(LocalApplications.RegistrationApp);
        }

        public void SetFocusField()
        {
            if (String.IsNullOrEmpty(UsernameTextBox.Text) ||
                                 !Helper.EmailMatchPattern.IsMatch(UsernameTextBox.Text))
            {
                UsernameTextBox.Focus();
            }
            else
            {
                PasswordBox.Focus();
            }
        }

        private void ErrorMessageOK_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.ClearErrorMessage();
        }
    }
}