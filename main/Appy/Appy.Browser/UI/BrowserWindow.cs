using System.Drawing;
using System.Windows.Forms;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.API;
using Gecko;

namespace AppDirect.WindowsClient.Browser.UI
{
    public partial class BrowserWindow : Form
    {
        private GeckoWebBrowser _browser;

        public string BrowserUrl { get; set; }
        private IAppDirectSession AppDirectSession { get; set; }

        public BrowserWindow(string url, IAppDirectSession session, int windowWidth = 0, int windowHeight = 0, bool isResizable = true)
        {
            BrowserUrl = url;
            AppDirectSession = session;
            InitializeComponent();
            Width = windowWidth != 0 ? windowWidth : Helper.DefaultBrowserWidth;
            Height = windowHeight != 0 ? windowHeight : Helper.DefaultBrowserHeight;
            
            AddBrowser();
        }

        private void AddBrowser()
        {
            _browser = new GeckoWebBrowser();
            _browser.Name = "_browser";
            _browser.Dock = DockStyle.Fill;
            _browser.DisableWmImeSetContext = true;
            _browser.Navigate(BrowserUrl);
            ContentPanel.Controls.Add(_browser);
        }

        private void closeBtn_MouseClick(object sender, System.EventArgs e)
        {
            Dispose(true);
        }

        private void maximizeBtn_Click(object sender, System.EventArgs e)
        {
            var button = sender as Button;
            if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;
                button.Image = global::AppDirect.WindowsClient.Browser.Properties.Resources.fullScreen;
            }
            else
            {
                WindowState = FormWindowState.Maximized;
                button.Image = global::AppDirect.WindowsClient.Browser.Properties.Resources.restoreDown;
            }
        }

        private void minimizeBtn_MouseClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            _browser.GoBack();
        }

        private void forwardBtn_Click(object sender, System.EventArgs e)
        {
            _browser.GoForward();
        }
    }
}
