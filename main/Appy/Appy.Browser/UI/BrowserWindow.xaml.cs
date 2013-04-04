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
        private Rect _restoreLocation;

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
            if ((ViewModel != null) && (ViewModel.Application != null))
            {
                browser.StartUrl = ViewModel.Application.UrlString;
            }

            Width = browserViewModel.Application.BrowserWidth;
            Height = browserViewModel.Application.BrowserHeight;
            TitleTextBlock.Text = browserViewModel.Application.Name;
        }

        public virtual void PreInitializeWindow()
        {
            browser.NavigateTo(ViewModel.Application.UrlString);
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
            _restoreLocation = new Rect { Width = Width, Height = Height, X = Left, Y = Top };
            Height = SystemParameters.WorkArea.Height + OuterBorder.BorderThickness.Top + OuterBorder.BorderThickness.Bottom;
            Width = SystemParameters.WorkArea.Width + OuterBorder.BorderThickness.Right + OuterBorder.BorderThickness.Left;
            Left = SystemParameters.WorkArea.Left - OuterBorder.BorderThickness.Left;
            Top = SystemParameters.WorkArea.Top - OuterBorder.BorderThickness.Top;

            Maximize.Visibility = Visibility.Hidden;
            RestoreDown.Visibility = Visibility.Visible;
        }

        private void RestoreDown_OnClick(object sender, RoutedEventArgs e)
        {
            Height = _restoreLocation.Height;
            Width = _restoreLocation.Width;
            Left = _restoreLocation.X;
            Top = _restoreLocation.Y;

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
    }
}