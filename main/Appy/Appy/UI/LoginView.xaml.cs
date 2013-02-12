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

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public EventHandler RegistrationClick;

        private static int minimumPasswordLength = 4;
        private static int maximumPasswordLength = 18;
        private static readonly Regex EmailMatchPattern = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
        private static readonly Regex PasswordMatchPattern = new Regex(@"^(.{" + minimumPasswordLength + "," + maximumPasswordLength + "})$");
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
            UsernameTextBox.Background = _validColorBrush;
            PasswordBox.Background = _validColorBrush;
            LoginFailedMessage.Visibility = Visibility.Hidden;
        }

        private void Email_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (LoginFailedMessage.Visibility == Visibility.Visible)
            {
                ClearLoginFailed();
            }

            EmailFormatErrorMessage.Visibility = Visibility.Hidden;

            if (e.Key == Key.Return)
            {
                LoginButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void UsernameTextBox_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var emailBox = (TextBox)sender;

            if (String.IsNullOrEmpty(emailBox.Text))
            {
                emailBox.Background = _defaultColorBrush;
            }

            else if (EmailMatchPattern.IsMatch(emailBox.Text))
            {
                emailBox.Background = _validColorBrush;
            }
            else
            {
                emailBox.Background = _errorColorBrush;
                EmailFormatErrorMessage.Visibility = Visibility.Visible;
            }

            SetLoginButtonState();
        }

        private void PasswordBox_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;

            if (String.IsNullOrEmpty(passwordBox.Password))
            {
                passwordBox.Background = _defaultColorBrush;
            }

            else if (PasswordMatchPattern.IsMatch(passwordBox.Password))
            {
                passwordBox.Background = _validColorBrush;
            }

            SetLoginButtonState();
        }

        private void SetLoginButtonState()
        {
            if (PasswordBox.Background.Equals(_validColorBrush) && UsernameTextBox.Background.Equals(_validColorBrush))
            {
                LoginButton.IsEnabled = true;
            }
            else
            {
                LoginButton.IsEnabled = false;
            }
        }

        private void PasswordBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LoginButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void Email_OnFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (LoginFailedMessage.Visibility == Visibility.Visible)
            {
                ClearLoginFailed();
            }

            if (EmailFormatErrorMessage.Visibility == Visibility.Visible)
            {
                EmailFormatErrorMessage.Visibility = Visibility.Hidden;
            }

            SetLoginButtonState();
        }

        private void Password_OnFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (LoginFailedMessage.Visibility == Visibility.Visible)
            {
                ClearLoginFailed();
            }

            SetLoginButtonState();
        }

        private void ForgotPassword(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Resources.ForgotPasswordUrlString);
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.Login(UsernameTextBox.Text, PasswordBox.Password))
                {
                //    YourAppsTab.IsSelected = true;
                //    SyncButton.Visibility = Visibility.Hidden;
                //    LogoutButton.Visibility = Visibility.Visible;
                    LoginFailedMessage.Visibility = Visibility.Hidden;
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


        private void CancelLoginClick(object sender, RoutedEventArgs e)
        {
            this.Visibility= Visibility.Hidden;
        }

        public void GoToRegistrationClick(object sender, EventArgs eventArgs)
        {
            RegistrationClick.Invoke(sender, eventArgs);

            //var emailAddress = NewCustomerEmail.Text;

            //var serviceAddress = Properties.Resources.BaseAppStoreUrl + Properties.Resources.RegisterEmailUrl;

            //var request = HttpWebRequest.Create(String.Format(serviceAddress, emailAddress));

            //WebResponse webResponse = request.GetResponse();

            //MessageArea.Text =
            //    "Thanks for registering. Please check your inbox and click the link to activate your account.";
        }
    }
}
