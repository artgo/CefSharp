using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.InteropAPI;
using Application = AppDirect.WindowsClient.Common.API.Application;

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for TaskbarButton.xaml
    /// </summary>
    public partial class TaskbarButton : UserControl
    {
        public string Id { get; set; }

        private const int LargeIconSize = 30;
        private const int SmallIconSize = 20;

        private const int MarginSize = 6;

        public EventHandler PinToTaskbarClickNotifier;
        public EventHandler UninstallClickNotifier;

        public TaskbarButton()
        {
            InitializeComponent();
        }

        public TaskbarButton(ApplicationViewModel applicationViewModel, TaskbarIconsSize iconsSize)
        {
            DataContext = applicationViewModel;
            InitializeComponent();

            Id = applicationViewModel.Application.Id;

            SetIconSize(iconsSize);
        }

        private void TaskbarButton_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                e.Handled = true;
            }
        }

        private void TaskbarButton_Click(object sender, RoutedEventArgs e)
        {
            Helper.AppButtonClick(sender, e);
        }

        public void PinToTaskBarClick(object sender, RoutedEventArgs e)
        {
            PinToTaskbarClickNotifier.Invoke(sender, e);
        }

        public void UninstallAppClick(object sender, RoutedEventArgs e)
        {
            UninstallClickNotifier.Invoke(sender, e);
        }

        public void SetIconSize(TaskbarIconsSize newIconsSize)
        {
            int newSize = newIconsSize == TaskbarIconsSize.Large ? LargeIconSize : SmallIconSize;
            if (AppButton.Width != newSize)
            {
                AppButton.Width = newSize;
                Width = AppButton.Width + MarginSize;
            }

            if (AppButton.Height != newSize)
            {
                AppButton.Height = newSize;
                Height = AppButton.Height + MarginSize;
            }
        }
    }
}
