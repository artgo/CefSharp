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

        private void ClearRegistrationEmailFailed()
        {
            if (RegistrationEmailFormatErrorMessage.Visibility != Visibility.Visible)
            {
                UsernameTextBox.Background = _validColorBrush;
                RegistrationEmailFormatErrorMessage.Visibility = Visibility.Hidden;
            }
        }

        private void RegistrationUsernameTextBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                UsernameTextBoxRegistration.Background = _validColorBrush;
                GoToRegistrationButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void RegistrationUsernameTextBox_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ClearRegistrationEmailFailed();
            CheckUsername(UsernameTextBoxRegistration, RegistrationEmailFormatErrorMessage);
        }

        private void RegistrationUsernameTextBox_OnFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            UsernameTextBoxRegistration.Background = _validColorBrush;
            RegistrationEmailFormatErrorMessage.Visibility = Visibility.Hidden;
            ClearRegistrationEmailFailed();
        }

        private void RegistrationUsernameTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (RegistrationEmailFormatErrorMessage.Visibility == Visibility.Visible)
            {
                ClearRegistrationEmailFailed();
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
            CheckUsername(UsernameTextBox, EmailFormatErrorMessage);
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
                CheckUsername(UsernameTextBox, EmailFormatErrorMessage);
            }
        }

        private void CheckUsername(TextBox textbox, TextBlock errorMessage)
        {
            if (String.IsNullOrEmpty(textbox.Text))
            {
                textbox.Background = _defaultColorBrush;
                errorMessage.Visibility = Visibility.Hidden;
            }
            else if (Helper.EmailMatchPattern.IsMatch(textbox.Text))
            {
                textbox.Background = _validColorBrush;
                errorMessage.Visibility = Visibility.Hidden;
            }
            else
            {
                textbox.Background = _errorColorBrush;
                errorMessage.Visibility = Visibility.Visible;
            }
        }

        private void Password_OnFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            PasswordBox.Background = _validColorBrush;
            ClearLoginFailed();
            CheckUsername(UsernameTextBox, EmailFormatErrorMessage);
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
            CheckUsername(UsernameTextBox, EmailFormatErrorMessage);

            if (e.Key == Key.Return)
            {
                LoginButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ClearLoginFailed();
            CheckUsername(UsernameTextBox, EmailFormatErrorMessage);
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

            using (var loginBackgroundWorker = new BackgroundWorker())
            {
                loginBackgroundWorker.DoWork += LoginTask;
                loginBackgroundWorker.RunWorkerCompleted += LoginComplete;

                ServiceLocator.LocalStorage.SetCredentials(UsernameTextBox.Text, PasswordBox.Password);

                loginBackgroundWorker.RunWorkerAsync();
            }
        }

        private void LoginTask(object sender, DoWorkEventArgs e)
        {
            e.Result = Helper.RetryAction<bool>(Helper.Authenticate, 3, TimeSpan.FromMilliseconds(200));
        }

        private void LoginComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null && e.Error.GetType() == typeof(WebException))
            {
                var ex = (WebException) e.Error;
                _log.ErrorException("Login exception", ex);
                ViewModel.ShowNetworkProblem(ex);
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
            bool emailIsBad = !ValidateEmail(UsernameTextBox.Text);

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

        private bool ValidateEmail(string text)
        {
            return !String.IsNullOrEmpty(text) &&
                   Helper.EmailMatchPattern.IsMatch(text);
        }

        private void CancelLoginClick(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = string.Empty;
            CloseLogin.Invoke(sender, e);
        }

        public void GoToRegistrationClick(object sender, EventArgs eventArgs)
        {
            var email = UsernameTextBoxRegistration.Text;

            if (!ValidateEmail(email))
            {
                RegistrationEmailFormatErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            try
            {   
                ServiceLocator.CachedAppDirectApi.SendUserEmail(email);
            }
            catch (WebException e)
            {
                var response = (HttpWebResponse)e.Response;

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        EmailFormatErrorMessage.Visibility = Visibility.Visible;
                        return;
                    case HttpStatusCode.Conflict:
                        ViewModel.ErrorMessage = Properties.Resources.RegistrationConflictMessage;
                        return;
                    case HttpStatusCode.Forbidden:
                        ViewModel.ErrorMessage = Properties.Resources.RegistrationForbiddenMessage;
                        return;
                }

                ViewModel.ErrorMessage = Properties.Resources.RegistrationErrorMessage;
            }

            ViewModel.ErrorMessage = String.Format(Properties.Resources.RegistrationEmailSuccessMessage, email);
            UsernameTextBoxRegistration.Text = string.Empty;
            UsernameTextBox.Text = email;
            UsernameTextBox.Focus();
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