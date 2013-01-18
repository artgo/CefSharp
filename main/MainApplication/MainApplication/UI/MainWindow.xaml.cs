using System;
using System.Net;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using Application = AppDirect.WindowsClient.Models.Application;

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel
        {
            get { return DataContext as MainViewModel; }
        }

        public MainWindow()
        {
            try
            {
                DataContext = new MainViewModel();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            InitializeComponent();
            
            Left = SystemParameters.WorkArea.Right * .003;
            Top = SystemParameters.WorkArea.Bottom - Height;
        }
      
        private void GoToAppStore(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Resources.AppStoreUrlString);
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
                    YourAppsTab.IsSelected = true;
                }
                else
                {
                    LoginFailedMessage.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (!String.IsNullOrEmpty(ViewModel.MyAppsLoadError))
            {
                ReloadMyAppsButton.Visibility = Visibility.Visible;
            }
        }

        private void AppButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var clickedApp = ((Button)sender).DataContext as Application;

                var window = new ChromiumWindow() { UrlAddress = clickedApp.UrlString, Session = ServiceLocator.CachedAppDirectApi.Session };
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InstallAppClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application clickedApp = ((Button)sender).DataContext as Application;

                if (!clickedApp.IsLocalApp && ServiceLocator.LocalStorage.LoginInfo == null)
                {
                    ViewModel.LoginHeaderText = String.Format(Properties.Resources.LoginHeader, clickedApp.Name);
                    LoginTab.IsSelected = true;
                }
                else
                {
                    ViewModel.Install(clickedApp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (!String.IsNullOrEmpty(ViewModel.MyAppsLoadError))
            {
                ReloadMyAppsButton.Visibility = Visibility.Visible;
            }
        }

        private void UninstallAppClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application clickedApp = ((MenuItem)sender).DataContext as Application;

                ViewModel.Uninstall(clickedApp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (!String.IsNullOrEmpty(ViewModel.MyAppsLoadError))
            {
                ReloadMyAppsButton.Visibility = Visibility.Visible;
            }
        }

        private void ReloadMyAppsClick(object sender, RoutedEventArgs e)
        {
            ViewModel.RefreshAppsLists();

            if (String.IsNullOrEmpty(ViewModel.MyAppsLoadError))
            {
                ReloadMyAppsButton.Visibility = Visibility.Hidden;
            }
        }

        private void SyncButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (ServiceLocator.LocalStorage.HasCredentials)
            {
                ViewModel.RefreshAppsLists();
            }
            else
            {
                ViewModel.LoginHeaderText = "Please Login to View Your Apps";
                LoginTab.IsSelected = true;
            }
        }

        private void CancelLoginClick(object sender, RoutedEventArgs e)
        {
            YourAppsTab.IsSelected = true;
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            var emailAddress = NewCustomerEmail.Text;

            var serviceAddress = Properties.Resources.BaseAppStoreUrl + Properties.Resources.RegisterEmailUrl;
            
            var request = HttpWebRequest.Create(String.Format(serviceAddress, emailAddress));

            WebResponse webResponse = request.GetResponse();

            MessageArea.Text =
                "Thanks for registering. Please check your inbox and click the link to activate your account.";
        }
    }
}
    
