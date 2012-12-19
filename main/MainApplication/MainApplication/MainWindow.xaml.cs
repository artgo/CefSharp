using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isWindowExpanded = false;

        public MainWindow()
        {
            InitializeComponent();

            Left = SystemParameters.WorkArea.Right * .003;
            Top = SystemParameters.WorkArea.Bottom - Height;
        }
        
        private void LaunchLogin(object sender, RoutedEventArgs e)
        {
            if (!_isWindowExpanded)
            {
                LoginButton.Visibility = Visibility.Hidden;
                CancelLoginButton.Visibility = Visibility.Visible;
                Width += 220;
                _isWindowExpanded = true;
            }
        }

        private void CollapseLogin(object sender, RoutedEventArgs e)
        {
            if (_isWindowExpanded)
            {
                LoginButton.Visibility = Visibility.Visible;
                CancelLoginButton.Visibility = Visibility.Hidden;
                Width -= 220;
                _isWindowExpanded = false;
            }
        }

        private void GoToAppStore(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://appcenter.staples.com/home");
        }
    }
}
    
