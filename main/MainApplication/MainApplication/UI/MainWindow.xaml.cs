using System;
using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
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
        private readonly IDictionary<String, ChromiumWindow> _chromiumWindows = new Dictionary<String, ChromiumWindow>();

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
                    SyncButton.Visibility = Visibility.Hidden;
                    LogoutButton.Visibility = Visibility.Visible; 
                    LoginFailedMessage.Visibility = Visibility.Hidden;
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
        }

        private static Application GetApplicationFromButtonSender(object sender)
        {
            return ((Button)sender).DataContext as Application;
        }

        private void AppButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var clickedApp = GetApplicationFromButtonSender(sender);

                if ((clickedApp == null) || (String.IsNullOrEmpty(clickedApp.UrlString)))
                {
                    MessageBox.Show("Application developer didn't set application's URL");
                }
                else
                {
                    ChromiumWindow window;
                    if (!_chromiumWindows.TryGetValue(clickedApp.Id, out window))
                    {
                        window = new ChromiumWindow()
                            {
                                UrlAddress = clickedApp.UrlString,
                                Session = ServiceLocator.CachedAppDirectApi.Session,
                                ApplicationId = clickedApp.Id
                            };

                        window.CloseWindow += new EventHandler(ChromiumWindow_Close);
                        _chromiumWindows[clickedApp.Id] = window;
                        
                    }

                    window.Show();
                    window.Activate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ChromiumWindow_Close(object sender, EventArgs e)
        {
            var chromiumWindow = (ChromiumWindow) sender;
            _chromiumWindows.Remove(chromiumWindow.ApplicationId);
        }
        
        private void InstallAppClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var clickedApp = GetApplicationFromButtonSender(sender);

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
        }

        private void UninstallAppClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var clickedApp = ((MenuItem)sender).DataContext as Application;

                ViewModel.Uninstall(clickedApp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ReloadMyAppsClick(object sender, RoutedEventArgs e)
        {
            ViewModel.RefreshAppsLists();
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

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Logout();
            LogoutButton.Visibility = Visibility.Hidden;
            SyncButton.Visibility = Visibility.Visible;
        }
    }
}
    
