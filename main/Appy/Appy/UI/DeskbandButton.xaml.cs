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
using Application = AppDirect.WindowsClient.Common.API.Application;

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for TaskbarButton.xaml
    /// </summary>
    public partial class DeskbandButton : UserControl
    {
        public DeskbandButton()
        {
            InitializeComponent();
        }

        public DeskbandButton(Application application)
        {
            DataContext = application;
            InitializeComponent();

            Name = application.Id;
        }

        private void TaskbarButton_Click(object sender, RoutedEventArgs e)
        {
            Helper.AppButtonClick(sender, e);
        }
    }
}
