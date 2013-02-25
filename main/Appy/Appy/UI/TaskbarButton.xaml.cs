using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        public TaskbarButton()
        {
            InitializeComponent();
        }

        public TaskbarButton(Application application)
        {
            DataContext = application;
            InitializeComponent();

            Id = application.Id;
        }

        private void TaskbarButton_Click(object sender, RoutedEventArgs e)
        {
            Helper.AppButtonClick(sender, e);
        }

        public void ChangeIconSize(TaskbarIconsSize newIconsSize)
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
        }
    }
}
