using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.InteropAPI.Internal;
using Gecko;
using Screen = System.Windows.Forms.Screen;

namespace AppDirect.WindowsClient.Browser.UI
{
    public partial class BrowserWindow : Form
    {
        private GeckoWebBrowser _browser;
        //private const int WM_NCLBUTTONDOWN = 0xA1;
        //private const int HT_CAPTION = 0x2;
        private int LastNonMaxHeight;
        private int LastNonMaxWidth;
        private int LastNonMaxTop;
        private int LastNonMaxLeft;

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
            if (Width == Screen.PrimaryScreen.WorkingArea.Width && Height == Screen.PrimaryScreen.WorkingArea.Height)
            {
                Left = LastNonMaxLeft;
                Top = LastNonMaxTop;
                Width = LastNonMaxWidth;
                Height = LastNonMaxHeight;
                button.Image = global::AppDirect.WindowsClient.Browser.Properties.Resources.fullScreen;
            }
            else
            {
                LastNonMaxLeft = Left;
                LastNonMaxTop = Top;
                Left = 0;
                Top = 0;
                LastNonMaxWidth = Width;
                LastNonMaxHeight = Height;
                Width = Screen.PrimaryScreen.WorkingArea.Width;
                Height = Screen.PrimaryScreen.WorkingArea.Height;
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

        private void TopMenu_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                User32Dll.ReleaseCapture();
                User32Dll.SendMessage(Handle, (int)WM.NCLBUTTONDOWN, (int)WindowsHitTestConstants.HTCAPTION, 0);
            }
        }
    }
}
