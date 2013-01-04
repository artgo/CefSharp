using System;
using System.Windows;
using AppDirect.WindowsClient.Models;

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        private const string ForgotPasswordUrl = "https://www.appdirect.com/forgotPassword";

        public MainViewModel ViewModel
        {
            get { return DataContext as MainViewModel; }
        }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void ForgotPassword(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(ForgotPasswordUrl);
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            var loginObject = new LoginObject
                {
                    Password = PasswordTextBox.Text,
                    UserName = UsernameTextBox.Text
                };

            try
            {
                ViewModel.Login(loginObject);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login Failed: " + ex.Message);
            }
        }
    }
}
