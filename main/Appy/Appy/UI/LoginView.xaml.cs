using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AppDirect.WindowsClient.API;

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public EventHandler RegistrationClick;
        public EventHandler CloseLogin;

        private readonly SolidColorBrush _errorColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#de2b2b"));
        private readonly SolidColorBrush _defaultColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#36454d"));
        private readonly SolidColorBrush _validColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#4aa0ce"));

        public MainViewModel ViewModel
        {
            get { return DataContext as MainViewModel; }
        }

        public LoginView()
        {
            InitializeComponent();
        }

        private void LoginFailed()
        {
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

            PasswordBox.Background = _validColorBrush;
            LoginFailedMessage.Visibility = Visibility.Hidden;
        }

        private void Email_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && LoginButton.IsEnabled)
            {
                LoginButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void UsernameTextBox_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var emailBox = (TextBox)sender;

            if (EmailFormatErrorMessage.Visibility != Visibility.Visible && String.IsNullOrEmpty(emailBox.Text))
            {
                emailBox.Background = _defaultColorBrush;
            }

            else if (Helper.EmailMatchPattern.IsMatch(emailBox.Text))
            {
                emailBox.Background = _validColorBrush;
            }
            else
            {
                emailBox.Background = _errorColorBrush;
                EmailFormatErrorMessage.Visibility = Visibility.Visible;
            }
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
            //if ( PasswordMatchPattern.IsMatch(passwordBox.Password))
            //{
            //    passwordBox.Background = _validColorBrush;
            //}
            
            if (e.Key == Key.Return)
            {
                LoginButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }

        }

        private void Email_OnFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (LoginFailedMessage.Visibility != Visibility.Visible && EmailFormatErrorMessage.Visibility != Visibility.Visible)
            {
                UsernameTextBox.Background = _validColorBrush;
            }
        }

        private void Password_OnFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (LoginFailedMessage.Visibility != Visibility.Visible)
            {
                PasswordBox.Background = _validColorBrush;
            }
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (LoginFailedMessage.Visibility == Visibility.Visible)
            {
                ClearLoginFailed();
            }
        }

        private void UsernameTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var emailBox = (TextBox)sender;

            if (LoginFailedMessage.Visibility == Visibility.Visible && Helper.EmailMatchPattern.IsMatch(emailBox.Text))
            {
                ClearLoginFailed();
            }

            if (EmailFormatErrorMessage.Visibility == Visibility.Visible && Helper.EmailMatchPattern.IsMatch(emailBox.Text))
            {
                emailBox.Background = _validColorBrush;
                EmailFormatErrorMessage.Visibility = Visibility.Hidden;
            }
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

            try
            {
                if (ViewModel.Login(UsernameTextBox.Text, PasswordBox.Password))
                {
                    LoginFailedMessage.Visibility = Visibility.Hidden;
                    CloseLogin.Invoke(sender, e);
                }
                else
                {
                    LoginFailed();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool ValidateRequiredFields()
        {
            bool allFieldsValid = true;

            if (String.IsNullOrEmpty(UsernameTextBox.Text))
            {
                UsernameTextBox.Background = _errorColorBrush;
                allFieldsValid = false;
            }
            else if (!Helper.EmailMatchPattern.IsMatch(UsernameTextBox.Text))
            {
                allFieldsValid = false;
            }
           
            if (String.IsNullOrEmpty(PasswordBox.Password))
            {
                PasswordBox.Background = _errorColorBrush;
                allFieldsValid = false;
            }

            return allFieldsValid;
        }

        private void CancelLoginClick(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        public void GoToRegistrationClick(object sender, EventArgs eventArgs)
        {
            RegistrationClick.Invoke(sender, eventArgs);
        }
    }
}
