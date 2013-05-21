using System.Threading;
using System.Windows.Forms;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.InteropAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<UIElement> WindowPanels = new List<UIElement>();
        public EventHandler PinToTaskbarClickNotifier;
        public EventHandler UninstallClickNotifier;

        public MainViewModel ViewModel { get; set; }

        private static readonly ILogger _log = new NLogLogger("MainWindow");

        public MainWindow(MainViewModel mainViewModel, TaskbarPanel taskbarPanel)
        {
            try
            {
                DataContext = mainViewModel;
                ViewModel = mainViewModel;
            }
            catch (Exception e)
            {
                _log.ErrorException("Error setting context", e);

                MessageBox.Show(e.Message);
            }

            InitializeComponent();

            SetPosition();

            WindowPanels.Add(LoginViewControl);
            WindowPanels.Add(RegistrationViewControl);

            RegisterTaskbarCallbacks(taskbarPanel);

            LoginViewControl.RegistrationClick += Login_OnRegistrationClick;
            LoginViewControl.CloseLogin += Login_Close;
            LoginViewControl.LoginSuccessfulNotifier += ViewModel.LoginSuccessful;

            LoginViewControl.DataContext = ViewModel.LoginViewModel;

            RegistrationViewControl.ClosePanel += Registration_Close;
        }

        public void RegisterTaskbarCallbacks(TaskbarPanel taskbarPanel)
        {
            ViewModel.ApplicationAddedNotifier += taskbarPanel.AddAppButton;
            ViewModel.ApplicationRemovedNotifier += taskbarPanel.RemoveAppButton;
            ViewModel.LogoutNotifier += taskbarPanel.RemoveAllAppButtons;
            PinToTaskbarClickNotifier += taskbarPanel.PinToTaskbarClickHandler;
            UninstallClickNotifier += taskbarPanel.UninstallAppClickHandler;
        }

        public void SetPosition()
        {
            var screen = TaskbarApi.Instance.TaskbarScreen;
            var scalingFactor = 1.0/TaskbarApi.Instance.DpiScalingFactor;
            switch (TaskbarApi.Instance.TaskbarPosition)
            {
                case TaskbarPosition.Bottom:
                    Left = screen.WorkingArea.Left * scalingFactor;
                    Top = ((screen.Bounds.Bottom - TaskbarApi.Instance.TaskbarHeight ) * scalingFactor) - Height;
                    break;

                case TaskbarPosition.Left:
                    Left = (screen.Bounds.Left + TaskbarApi.Instance.TaskbarHeight) * scalingFactor;
                    Top = screen.WorkingArea.Top * scalingFactor;
                    break;

                case TaskbarPosition.Right:
                    Left = ((screen.Bounds.Right - TaskbarApi.Instance.TaskbarHeight) * scalingFactor) - Width;
                    Top = screen.WorkingArea.Top * scalingFactor;
                    break;

                case TaskbarPosition.Top:
                    Left = screen.WorkingArea.Left * scalingFactor;
                    Top = (screen.Bounds.Top + TaskbarApi.Instance.TaskbarHeight) * scalingFactor;
                    break;
            }
        }

        private void Login_Close(object sender, EventArgs e)
        {
            ViewModel.CollapseLogin();
            LoginViewControl.PasswordBox.Password = string.Empty;
        }

        private void Registration_Close(object sender, EventArgs e)
        {
            LoginViewControl.PasswordBox.Password = string.Empty;
            RegistrationViewControl.Visibility = Visibility.Hidden;
        }

        private void DoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                e.Handled = true;
            }
        }

        private void AppButtonClick(object sender, RoutedEventArgs e)
        {
            Helper.AppButtonClick(sender, e);
        }

        private void InstallApp_OnClick(object sender, RoutedEventArgs e)
        {
            var clickedApp = Helper.GetApplicationFromButtonSender(sender);

            try
            {
                ViewModel.AddApp(clickedApp);
            }
            catch (Exception ex)
            {
                _log.ErrorException("Error during installation of app", ex);

                MessageBox.Show(ex.Message);
            }
        }

        public void UninstallAppClick(object sender, EventArgs e)
        {
            UninstallClickNotifier.Invoke(sender, e);
        }

        private void LogInLogOutButtonClick(object sender, RoutedEventArgs e)
        {
            ViewModel.LogInLogOutClicked();
        }

        private void UpdateButtonOnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdateClick();
        }

        private void Login_OnRegistrationClick(object o, EventArgs e)
        {
            RegistrationViewControl.Visibility = Visibility.Visible;
        }

        private void PinToTaskBarClick(object sender, RoutedEventArgs e)
        {
            PinToTaskbarClickNotifier.Invoke(sender, e);
        }

        private void Settings_OnSubmenuClosed(object sender, RoutedEventArgs e)
        {
            ViewModel.ResetUpdateText();
        }

        private void AboutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.GetAboutDialog();
        }
    }
}