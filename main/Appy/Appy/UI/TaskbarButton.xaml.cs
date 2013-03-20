using System;
using System.Windows;
using System.Windows.Controls;
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

            ChangeIconSize(iconsSize);
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

        public void ChangeIconSize(TaskbarIconsSize newIconsSize)
        {
            Helper.PerformInUiThread(() =>
                {
                    if (newIconsSize == TaskbarIconsSize.Large)
                    {
                        AppButton.Width = LargeIconSize;
                        AppButton.Height = LargeIconSize;
                    }
                    else
                    {
                        AppButton.Width = SmallIconSize;
                        AppButton.Height = SmallIconSize;
                    }

                    Width = AppButton.Width + MarginSize;
                    Height = AppButton.Height + MarginSize;
                });
        }
    }
}
