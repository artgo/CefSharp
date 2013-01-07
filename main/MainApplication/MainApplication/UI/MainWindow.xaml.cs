using System;
using System.Windows;
using System.Windows.Controls;
using AppDirect.WindowsClient.Models;
using AppDirect.WindowsClient.Storage;
using Application = AppDirect.WindowsClient.Models.Application;

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string MustLoginMessage = "You must log in to access ";


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
            try
            {
                if (ViewModel.Login(UsernameTextBox.Text, PasswordTextBox.Text))
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
        }

        private void InstallAppClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application clickedApp = ((Button)sender).DataContext as Application;

                if (!clickedApp.IsLocalApp && LocalStorage.Instance.LoginInfo == null)
                {
                    YouMustBeLoggedInMessage.Text = MustLoginMessage + clickedApp.Name;
                    YouMustBeLoggedInMessage.Visibility = Visibility.Visible;
                    SettingsTab.IsSelected = true;
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
                Application clickedApp = ((MenuItem)sender).DataContext as Application;

                ViewModel.Uninstall(clickedApp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
    
