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

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for RegistrationView.xaml
    /// </summary>
    public partial class RegistrationView : UserControl
    {
        public EventHandler ClosePanel;

        public RegistrationView()
        {
            InitializeComponent();
        }

        public void GoBackClick(object sender, RoutedEventArgs e)
        {
            ClosePanel(sender, e);
        }
    }
}
