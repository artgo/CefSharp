using System.Windows;
using AppDirect.WindowsClient.Browser.UI;

namespace AppDirect.WindowsClient.Browser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Window BrowserWindow { get; set; }

        public App() : base()
        {
        }

        public App(Window browserWindow)
        {
            BrowserWindow = browserWindow;
        }

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            BrowserWindow.Show();
        }
    }
}
