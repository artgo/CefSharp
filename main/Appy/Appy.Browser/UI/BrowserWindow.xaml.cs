using System;
using System.Windows;
using System.Windows.Input;

namespace AppDirect.WindowsClient.Browser.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class BrowserWindow : Window
    {
        private BrowserViewModel ViewModel { get; set; }

        public BrowserWindow(BrowserViewModel browserViewModel)
        {
            ViewModel = browserViewModel;

            InitializeComponent();

            if ((ViewModel != null) && (ViewModel.Application != null))
            {
                var url = ViewModel.Application.UrlString;
                addressTextBox.Text = url;
                browser.StartUrl = url;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            browser.Dispose();
            base.OnClosed(e);
        }

        private void addressTextBox_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                browser.NavigateTo(addressTextBox.Text);
            }
        }
    }
}
