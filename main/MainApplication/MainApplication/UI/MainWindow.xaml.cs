using System.Windows;

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoginWindow _loginWindow;

        public MainViewModel ViewModel
        {
            get { return DataContext as MainViewModel; }
        }

        public MainWindow()
        {
            InitializeComponent();

            Left = SystemParameters.WorkArea.Right * .003;
            Top = SystemParameters.WorkArea.Bottom - Height;
        }
        
        private void ClickLogin(object sender, RoutedEventArgs e)
        {
            YourApps.Visibility = Visibility.Hidden;
            SettingsGridView.Visibility = Visibility.Visible;
            //if (ViewModel.IsLoggedIn)
            //{
            //    ViewModel.Logout();
            //    return;
            //}

            //_loginWindow = new LoginWindow();
            //_loginWindow.Left = Left + Width;
            //_loginWindow.Top = Top;
            //_loginWindow.ShowDialog();
        }

        private void CollapseLogin(object sender, RoutedEventArgs e)
        {
            _loginWindow.Close();
        }

        private void GoToAppStore(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://appcenter.staples.com/home");
        }

        private void ViewMyApps(object sender, RoutedEventArgs e)
        {
            YourApps.Visibility = Visibility.Visible;
            SettingsGridView.Visibility = Visibility.Hidden;
        }

         private void ForgotPassword(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.appdirect.com/forgotPassword");
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

                YourApps.Visibility = Visibility.Visible;
                SettingsGridView.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login Failed: " + ex.Message);
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Application clickedApp = ((Button)sender).DataContext as Application;
         
            if (!clickedApp.IsLocalApp && !ViewModel.IsLoggedIn)
            {
                YourApps.Visibility = Visibility.Hidden;
                SettingsGridView.Visibility = Visibility.Visible;
            }
            else
            {
                ViewModel.Install(clickedApp);
            }
        }

        private void UninstallAppClick(object sender, RoutedEventArgs e)
        {
            Application clickedApp = ((MenuItem)sender).DataContext as Application;

            ViewModel.Uninstall(clickedApp);
        }
    }
}
    
