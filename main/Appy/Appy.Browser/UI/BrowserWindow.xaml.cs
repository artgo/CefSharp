using AppDirect.WindowsClient.Common.API;
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
            Title = browserViewModel.Application.Name;
            if (browserViewModel == null)
            {
                throw new ArgumentNullException("browserViewModel");
            }

            ViewModel = browserViewModel;

            DataContext = ViewModel;
            InitializeComponent();
            if (browserViewModel.Application != null)
            {
                if ((browserViewModel.Session != null) && (browserViewModel.Session.Cookies.Count > 0))
                {
                    browser.StartUrl = browserViewModel.Application.UrlString;
                }

                Width = browserViewModel.Application.BrowserWidth;
                Height = browserViewModel.Application.BrowserHeight;
                TitleTextBlock.Text = browserViewModel.Application.Name;
            }
        }

        public virtual void PreInitializeWindow()
        {
            if ((ViewModel.Session != null) && (ViewModel.Session.Cookies.Count > 0))
            {
                browser.NavigateTo(ViewModel.Application.UrlString);
            }
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
    }
}