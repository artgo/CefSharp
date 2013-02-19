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
using System.Windows.Media;
using AppDirect.WindowsClient.API;
using Application = AppDirect.WindowsClient.Common.API.Application;

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<UIElement> WindowPanels = new List<UIElement>();

        public EventHandler PinToTaskbarClickNotifier;
        public EventHandler ApplicationAddedNotifier;
        public EventHandler ApplicationRemovedNotifier;
        
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

            WindowPanels.Add(LoginViewControl);
            WindowPanels.Add(RegistrationViewControl);

            LoginViewControl.RegistrationClick += Login_OnRegistrationClick;
            LoginViewControl.CloseLogin += Login_Close;

            RegistrationViewControl.ClosePanel += Login_Close;

        }

        private void Login_Close(object sender, EventArgs e)
        {
            SetVisibleGrid(MainViewGrid);
        }

        private void SetVisibleGrid(UIElement visibleControl)
        {
            foreach (var windowPanel in WindowPanels)
            {
                if (windowPanel.Equals(visibleControl))
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
            while (true)
            {
                bool updateAvailable = ServiceLocator.Updater.GetUpdates(Helper.ApplicationVersion);

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
       

        private void AppButtonClick(object sender, RoutedEventArgs e)
        {
            Helper.AppButtonClick(sender, e);
        }

        private void InstallAppClick(object sender, RoutedEventArgs e)
        {
            var clickedApp = Helper.GetApplicationFromButtonSender(sender);

            try
            {
                if (!clickedApp.IsLocalApp && ServiceLocator.LocalStorage.LoginInfo == null)
                {
                    ViewModel.LoginHeaderText = String.Format(Properties.Resources.LoginHeader, clickedApp.Name);

                    SetVisibleGrid(LoginViewControl);
                    LoginViewControl.UsernameTextBox.Focus();
                }
                else
                {
                    ViewModel.Install(clickedApp);
                    ApplicationAddedNotifier.Invoke(clickedApp, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UninstallAppClick(object sender, RoutedEventArgs e)
        {
            var clickedApp = Helper.GetClickedAppFromContextMenuClick(sender);

            try
            {
                clickedApp.PinnedToTaskbar = false;
                ViewModel.Uninstall(clickedApp);
                ApplicationRemovedNotifier.Invoke(clickedApp, e);
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

                SetVisibleGrid(LoginViewControl);
                LoginViewControl.UsernameTextBox.Focus();
            }
        }

        private void CancelRegistrationClick(object sender, RoutedEventArgs e)
        {
            SetVisibleGrid(MainViewGrid);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Logout();
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

        private void Login_OnRegistrationClick(object o, EventArgs e)
        {
            SetVisibleGrid(RegistrationViewControl);
        }

        private void PinToTaskBarClick(object sender, RoutedEventArgs e)
        {
            PinToTaskbarClickNotifier.Invoke(sender,e);
        }
    }
}
    
