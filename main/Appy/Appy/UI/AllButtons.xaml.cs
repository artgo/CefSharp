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
    /// Interaction logic for AllButtons.xaml
    /// </summary>
    public partial class AllButtons : Window
    {
        private MainWindow _applicationWindow;

        private MainWindow ApplicationWindow
        {
            get
            {
                if (_applicationWindow == null)
                {
                    _applicationWindow = new MainWindow();
                }
                return _applicationWindow;
            }
            set { _applicationWindow = value; }
        }

        public AllButtons()
        {
            InitializeComponent();

            Left = SystemParameters.WorkArea.Right * .5;
            Top = SystemParameters.WorkArea.Bottom - Height;

            foreach (var application in ServiceLocator.LocalStorage.GetPinnedApps())
            {
                AddButton(application);
            }

            if (ApplicationWindow == null)
            {
                ApplicationWindow = new MainWindow();
            }
            
            ServiceLocator.LocalStorage.NotifyPinnedAppAdded += AddAppButton;
            ServiceLocator.LocalStorage.NotifyPinnedAppRemoved += RemoveAppButton;
        }

        private void AddButton(Application application)
        {
            ButtonContainer.Children.Add(new TaskbarButton(application));
            Width += 40;
        }

        private void RemoveButton(Application application)
        {
            var btn = ButtonContainer.Children.OfType<TaskbarButton>().First(b => b.Name == application.Id);

            ButtonContainer.Children.Remove(btn);
            Width -= 40;
        }

        private void RemoveAppButton(object sender, EventArgs e)
        {
            var application = sender as Application;

            RemoveButton(application);
        }

        private void AddAppButton(object sender, EventArgs e)
        {
            var application = sender as Application;

            AddButton(application);
        }

        private void AppButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationWindow.Visibility == Visibility.Visible)
            {
                ApplicationWindow.Visibility = Visibility.Collapsed;
            }
            else
            {
                ApplicationWindow.Show();
            }
        }
    }
}
