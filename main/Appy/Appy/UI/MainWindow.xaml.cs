using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using AppDirect.WindowsClient.API;
using Application = AppDirect.WindowsClient.Models.Application;

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Regex EmailMatchPattern = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
        public event EventHandler CloseWindow;
        public List<Grid> WindowPanels = new List<Grid>();

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

            Left = SystemParameters.WorkArea.Right*.003;
            Top = SystemParameters.WorkArea.Bottom - Height;

            BackgroundWorker getUpdateThread = new BackgroundWorker();

            getUpdateThread.DoWork += DownloadAvailableUpdates;
            getUpdateThread.RunWorkerAsync();

            WindowPanels.Add(MainViewGrid);
            WindowPanels.Add(LoginViewGrid);
            WindowPanels.Add(RegistrationViewGrid);
        }

        private void SetVisibleGrid(Grid visibleGrid)
        {
            foreach (var windowPanel in WindowPanels)
            {
                if (windowPanel.Equals(visibleGrid))
                {
                    windowPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    windowPanel.Visibility = Visibility.Hidden;
                }
            }
        }

        private void DownloadAvailableUpdates(object sender, DoWorkEventArgs e)
        {
            string currentVersionString = Helper.ApplicationVersion;

            while (true)
            {
                bool updateAvailable = ServiceLocator.Updater.GetUpdates(currentVersionString);

                if (updateAvailable)
                {
                    if (System.Windows.Application.Current != null)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                                                                                        UpdateAvailableButton.Visibility =
                                                                                        Visibility.Visible));
                    }
                }

                Thread.Sleep(TimeSpan.FromDays(1));
            }
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
            return ((Button) sender).DataContext as Application;
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
                    ServiceLocator.BrowserWindowsCommunicator.OpenApp(clickedApp);
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
                var clickedApp = GetApplicationFromButtonSender(sender);

                if (!clickedApp.IsLocalApp && ServiceLocator.LocalStorage.LoginInfo == null)
                {
                    ViewModel.LoginHeaderText = String.Format(Properties.Resources.LoginHeader, clickedApp.Name);
                    SetVisibleGrid(LoginViewGrid);
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
                var clickedApp = ((MenuItem) sender).DataContext as Application;

                ViewModel.Uninstall(clickedApp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                SetVisibleGrid(LoginViewGrid);
            }
        }

        private void CancelLoginClick(object sender, RoutedEventArgs e)
        {
            SetVisibleGrid(MainViewGrid);
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            SetVisibleGrid(RegistrationViewGrid);

            //var emailAddress = NewCustomerEmail.Text;

            //var serviceAddress = Properties.Resources.BaseAppStoreUrl + Properties.Resources.RegisterEmailUrl;

            //var request = HttpWebRequest.Create(String.Format(serviceAddress, emailAddress));

            //WebResponse webResponse = request.GetResponse();

            //MessageArea.Text =
            //    "Thanks for registering. Please check your inbox and click the link to activate your account.";
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Logout();
            LogoutButton.Visibility = Visibility.Hidden;
            SyncButton.Visibility = Visibility.Visible;
        }

        private void NewCustomerEmail_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                RegisterButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            
            else if (EmailMatchPattern.IsMatch(NewCustomerEmail.Text))
            {
                MessageBox.Show("Valid Email Now");
            }
        }

        private void PasswordBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LoginButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void LoginInfo_OnChange(object sender,
                                        DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            LoginFailedMessage.Visibility = Visibility.Hidden;
            EmailFormatErrorMessage.Visibility = Visibility.Hidden;
        }

        private void UpdateButtonOnClick(object sender, RoutedEventArgs e)
        {
            BackgroundWorker getUpdateThread = new BackgroundWorker();

            getUpdateThread.DoWork += InstallUpdates;
            getUpdateThread.RunWorkerAsync();
        }

        private void InstallUpdates(object sender, DoWorkEventArgs e)
        {
            ServiceLocator.Updater.InstallUpdates();

            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => Close()));
            }
        }

        private void MainWindow_OnClosing(object o, CancelEventArgs e)
        {
            Process[] processes = Process.GetProcessesByName(Helper.ApplicationName + Helper.BrowserProjectExt);
            foreach (Process process in processes)
            {
                Helper.RetryAction(() =>process.Kill(), 5, TimeSpan.FromMilliseconds(500));
            }
        }


    }
}
    
