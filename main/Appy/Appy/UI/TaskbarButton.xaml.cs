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

        private const int LargeIconSize = 28;
        private const int SmallIconSize = 18;

        private const int HorizontalMargin = 6;
        private const int VerticalMargin = 6;

        public TaskbarButton()
        {
            InitializeComponent();
        }

        public TaskbarButton(Application application, TaskbarIconsSize iconsSize)
        {
            DataContext = application;
            InitializeComponent();

            Id = application.Id;

            ChangeIconSize(iconsSize);
        }

        private void TaskbarButton_Click(object sender, RoutedEventArgs e)
        {
            Helper.AppButtonClick(sender, e);
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

                    Width = AppButton.Width + HorizontalMargin;
                    Height = AppButton.Height + VerticalMargin;
                });
        }
    }
}
