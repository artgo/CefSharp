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
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : UserControl
    {
        public MainViewModel MainViewModel
        {
            get { return (MainViewModel) DataContext; }
        }

        public CustomMessageBox()
        {
            InitializeComponent();
        }

        private void MessageOK_OnClick(object sender, RoutedEventArgs e)
        {
            CloseMessageBox();
        }

        private void CloseMessageBox()
        {
            MainViewModel.Message = null;
        }
    }
}
