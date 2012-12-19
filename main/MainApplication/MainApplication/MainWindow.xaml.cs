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
        private LoginWindow _loginWindow;

        public MainWindow()
        {
            InitializeComponent();

            Left = SystemParameters.WorkArea.Right * .003;
            Top = SystemParameters.WorkArea.Bottom - Height;
        }

        private void LaunchLogin(object sender, RoutedEventArgs e)
        {
            _loginWindow = new LoginWindow();
            _loginWindow.Left = Left + Width;
            _loginWindow.Top = Top;
            _loginWindow.Show();

            LoginButton.Visibility = Visibility.Hidden;
            CancelLoginButton.Visibility = Visibility.Visible;
        }

        private void CollapseLogin(object sender, RoutedEventArgs e)
        {
            _loginWindow.Close();

            LoginButton.Visibility = Visibility.Visible;
            CancelLoginButton.Visibility = Visibility.Hidden;

        }

        private void GoToAppStore(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://appcenter.staples.com/home");
        }
    }
}
    
