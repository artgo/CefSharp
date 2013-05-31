using System.Windows.Media.Imaging;
using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Common.API;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace AppDirect.WindowsClient.Browser.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class BrowserWindow : IBrowserWindow
    {
        private volatile bool _firstTime = true;
        private BrowserViewModel ViewModel { get; set; }

        public BrowserWindow()
        {
        }

        public BrowserWindow(BrowserViewModel browserViewModel)
        {
            if (browserViewModel == null)
            {
                throw new ArgumentNullException("browserViewModel");
            }

            ViewModel = browserViewModel;
            DataContext = ViewModel;

            InitializeComponent();

            if (browserViewModel.Application != null)
            {
                if ((browserViewModel.Session != null) && (browserViewModel.Session.Cookies != null) && (browserViewModel.Session.Cookies.Count > 0) && !string.IsNullOrEmpty(browserViewModel.Application.UrlString))
                {
                    browser.StartUrl = browserViewModel.Application.UrlString;
                    _firstTime = false;
                }

                if (!string.IsNullOrEmpty(browserViewModel.Application.Name))
                {
                    Title = browserViewModel.Application.Name;
                    TitleTextBlock.Text = browserViewModel.Application.Name;
                }

                if (browserViewModel.Application.BrowserWidth > 0)
                {
                    Width = browserViewModel.Application.BrowserWidth;
                }

                if (browserViewModel.Application.BrowserHeight > 0)
                {
                    Height = browserViewModel.Application.BrowserHeight;
                }
            }
        }

        public bool Visible { get { return IsVisible; } }

        public virtual void PreInitializeWindow()
        {
            if ((ViewModel.Session != null) && (ViewModel.Session.Cookies != null) && (ViewModel.Session.Cookies.Count > 0))
            {
                if (_firstTime)
                {
                    browser.NavigateTo(ViewModel.Application.UrlString);
                    _firstTime = false;
                }
                else
                {
                    browser.Refresh();
                }
            }
        }

        public void Display()
        {
            if (!IsVisible)
            {
                Show();
            }

            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }

            Activate();

            PreInitializeWindow();
        }

        public void Navigate()
        {
            browser.NavigateTo(ViewModel.Application.UrlString);
        }

        public void SetSession(IAppDirectSession session)
        {
            ViewModel.Session = session;
            PreInitializeWindow();
        }

        protected override void OnClosed(EventArgs e)
        {
            browser.Dispose();
            base.OnClosed(e);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            browser.GoBack();
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            browser.GoForward();
        }

        private void Minimize_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;

            Maximize.Visibility = Visibility.Hidden;
            RestoreDown.Visibility = Visibility.Visible;
        }

        private void RestoreDown_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;

            RestoreDown.Visibility = Visibility.Hidden;
            Maximize.Visibility = Visibility.Visible;
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void UIElement_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void BrowserWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
    }
}