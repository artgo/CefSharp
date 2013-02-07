using System.Windows.Forms;
using AppDirect.WindowsClient.Common.API;
using Gecko;

namespace AppDirect.WindowsClient.Browser.UI
{
    public partial class BrowserWindow : Form
    {
        private GeckoWebBrowser _browser;

        private string BrowserUrl { get; set; }
        private IAppDirectSession AppDirectSession { get; set; }

        public BrowserWindow(string url, IAppDirectSession session)
        {
            BrowserUrl = url;
            AppDirectSession = session;

            InitializeComponent();

            AddBrowser();
        }

        private void AddBrowser()
        {
            _browser = new GeckoWebBrowser();
            _browser.Name = "_browser";
            _browser.Dock = DockStyle.Fill;
            _browser.DisableWmImeSetContext = true;
            _browser.Navigate(BrowserUrl);
            Controls.Add(_browser);
        }
    }
}
