using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MainApplication
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
            Left = (SystemParameters.WorkArea.Right *.003) + 640;
            Top = SystemParameters.WorkArea.Bottom - 390;
        }

        private void ForgotPassword(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.appdirect.com/forgotPassword");
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            MainViewModel model = (MainViewModel) LoginGrid.DataContext;

            var loginObject = new LoginObject
                {
                    Password = PasswordTextBox.Text,
                    UserName = UsernameTextBox.Text
                };

            try
            {
                model.Login(loginObject);
                Close();

                model.LoginButtonDisplayText = "Log Out";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login Failed: " + ex.Message);
            }
        }
    }
}
