using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

// A user control is technically a normal content control which you can extend in some parts in the code but usually it is extended by placing other controls inside it. So as Kent mentioned a UserControl is an aggregation of other controls. This limits what you can do with a user control considerably. It's easier to use but more limited than a full custom control.
namespace AppDirect.WindowsClient.UI
{
    public delegate void DoExitEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Interaction logic for AllButtons.xaml
    /// </summary>
    public partial class AllButtons : UserControl
    {
        public AllButtons()
        {
            InitializeComponent();
            _IsPressed = false;
            _IsHover = false;
            InitImg();
        }

        private void InitImg()
        {
            BmpNormal = new BitmapImage(new Uri(@"Images\back.png", UriKind.Relative));
            BmpPressed = new BitmapImage(new Uri(@"Images\back.png", UriKind.Relative));
            BmpHover = new BitmapImage(new Uri(@"Images\back.png", UriKind.Relative));
            BmpPressedHover = new BitmapImage(new Uri(@"Images\back.png", UriKind.Relative));
            _TheImg.Source = BmpNormal;
        }

        #region events-commands to native C++ part
        public event DoExitEventHandler DoExit;
        protected virtual void OnDoExit(EventArgs e)
        {
            if (DoExit != null) DoExit(this, e);
        }
        #endregion	// events-commands to native C++ part


        private bool _MenuVisible = false;

        private Image TheImg { get { return _TheImg; } set { _TheImg = value; } }
        private BitmapImage BmpNormal { get; set; }
        private BitmapImage BmpPressed { get; set; }
        private BitmapImage BmpHover { get; set; }
        private BitmapImage BmpPressedHover { get; set; }

        #region properties
        private bool _IsPressed;
        private bool IsPressed
        {
            get { return _IsPressed; }
            set
            {
                if (value != _IsPressed)
                {
                    _IsPressed = value;
                    SetBmp();
                    ToggleMenu();
                }
            }
        }
        private bool _IsHover;
        private bool IsHover
        {
            get { return _IsHover; }
            set
            {
                if (value != _IsHover)
                {
                    _IsHover = value;
                    SetBmp();
                }
            }
        }
        #endregion // properties

        #region Main Button Menu
        private MainWindow _menu = null;
        private void ToggleMenu()
        {
            if (_IsPressed)
            {
                if (_menu == null) { _menu = new MainWindow(); }
                _menu.Show();
            }
            else
            {
                if (_menu != null) { _menu.Hide(); }
            }
        }
        #endregion //Main Button Menu

        private void SetBmp()
        {
            if (IsHover)
            {
                _TheImg.Source = IsPressed ? BmpPressedHover : BmpHover;
            }
            else
            {
                _TheImg.Source = IsPressed ? BmpPressed : BmpNormal;
            }
        }

        #region win message event handlers

        private void UserControl_MouseLeave_1(object sender, MouseEventArgs e)
        {
            IsHover = false;
        }

        private void UserControl_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            // change state of the Main Button with onle left mouse button
            if (e.ChangedButton == MouseButton.Left
                && e.MiddleButton != MouseButtonState.Pressed && e.RightButton != MouseButtonState.Pressed
                && e.XButton1 != MouseButtonState.Pressed && e.XButton2 != MouseButtonState.Pressed)
            {
                IsPressed = !IsPressed;
                return;
            }
        }

        private void UserControl_MouseEnter_1(object sender, MouseEventArgs e)
        {
            IsHover = true;
        }

        private void UserControl_MouseRightButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed)
            {
                ShowPopupMenu();
            }
        }
        #endregion // win message event handlers

        private void ShowPopupMenu()
        {
            if (!_MenuVisible)
            {
                IsPressed = false;
                _MenuVisible = true;
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)	// exit of context menu
        {
            OnDoExit(EventArgs.Empty);
        }
    }
}
