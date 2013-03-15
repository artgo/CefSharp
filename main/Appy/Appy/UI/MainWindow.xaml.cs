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
using AppDirect.WindowsClient.InteropAPI;
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
        public MainViewModel ViewModel { get; set; }

        public MainWindow(MainViewModel mainViewModel)
        {
            try
            {
                DataContext = mainViewModel;
                ViewModel = mainViewModel;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            InitializeComponent();

            SetPosition();

            WindowPanels.Add(LoginViewControl);
            WindowPanels.Add(RegistrationViewControl);

            LoginViewControl.RegistrationClick += Login_OnRegistrationClick;
            LoginViewControl.CloseLogin += Login_Close;
            LoginViewControl.LoginSuccessfulNotifier += ViewModel.LoginSuccessful;

            LoginViewControl.DataContext = ViewModel.LoginViewModel;

            RegistrationViewControl.ClosePanel += Login_Close;
        }

        public void SetPosition()
        {
            switch (TaskbarApi.Instance.TaskbarPosition)
            {
                case TaskbarPosition.Bottom:
                    Left = SystemParameters.WorkArea.Left;
                    Top = SystemParameters.WorkArea.Bottom - Height;
                    break;
                case TaskbarPosition.Left:
                    Left = SystemParameters.WorkArea.Left;
                    Top = SystemParameters.WorkArea.Top;
                    break;
                case TaskbarPosition.Right:
                    Left = SystemParameters.WorkArea.Right - Width;
                    Top = SystemParameters.WorkArea.Top;
                    break;
                case TaskbarPosition.Top:
                    Left = SystemParameters.WorkArea.Left;
                    Top = SystemParameters.WorkArea.Top;
                    break;
            }
        }

        private void Login_Close(object sender, EventArgs e)
        {
            ViewModel.CollapseLogin();
            LoginViewControl.PasswordBox.Password = string.Empty;
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

        private void AppButtonClick(object sender, RoutedEventArgs e)
        {
            Helper.AppButtonClick(sender, e);
        }

        private void InstallApp_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                e.Handled = true;
            }
        }

        private void InstallApp_OnClick(object sender, RoutedEventArgs e)
        {
            if (!e.Handled)
            {
                var clickedApp = Helper.GetApplicationFromButtonSender(sender);

                try
                {
                    if (!clickedApp.Application.IsLocalApp && ServiceLocator.LocalStorage.LoginInfo == null)
                    {
                        ViewModel.LoginViewModel.LoginHeaderText = String.Format(Properties.Resources.LoginHeader, clickedApp.Application.Name);
                        ViewModel.LoginViewModel.IsVisible = Visibility.Visible;
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

            e.Handled = true;
        }

        public void UninstallAppClick(object sender, EventArgs e)
        {
            var clickedApp = Helper.GetApplicationViewModelFromContextMenuClick(sender);

            try
            {
                clickedApp.PinnedToTaskbarNotifier = false;
                ViewModel.Uninstall(clickedApp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LogInButtonClick(object sender, RoutedEventArgs e)
        {
            ViewModel.LogInClicked();
        }

        public void UpdateAvailable(bool updateAvailable)
        {
            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => ViewModel.UpdateString = Properties.Resources.InstallUpdateString));
            }
        }

        private void UpdateButtonOnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdateClick();           
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

        private void Settings_OnSubmenuClosed(object sender, RoutedEventArgs e)
        {
            ViewModel.ResetUpdateText();
        }

        private void AboutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.ShowAboutDialog();
        }
    }
}
    
