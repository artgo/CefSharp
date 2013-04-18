using System;
using System.Windows;
using System.Windows.Controls;

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

        public void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceLocator.CachedAppDirectApi.RegisterUser(FirstNameTextBox.Text, LastNameTextBox.Text, 
                RegisterPasswordBox.Password, ConfirmRegisterPasswordBox.Password, NewCustomerEmail.Text, 
                ConfirmEmailTextBox.Text, CompanyTextBox.Text);
        }
    }
}
