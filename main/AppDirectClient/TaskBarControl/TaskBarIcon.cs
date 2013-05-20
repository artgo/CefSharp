using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace TaskBarControl
{
    public class TaskBarIcon
    {
        private const string NativeDllPath = @"native.dll";
        private const string WindowName = @"AppDirectTaskBarControl";
        private const string MessageNameUpdate = @"AppDirectButtonPositionUpdateMessage";

        private uint WM_APPDIRECT_UPDATE = 0;

        private ControlWrapper _controlWrapper;
        private HwndSource _hwndSource;
        private int _offset = 0;

        public TaskBarIcon(ControlWrapper controlWrapper)
        {
            _controlWrapper = controlWrapper;
            _controlWrapper.DesiredHeightChanged += _controlWrapper_DesiredHeightChanged;
            _controlWrapper.DesiredWidthChanged += _controlWrapper_DesiredWidthChanged;
            WM_APPDIRECT_UPDATE = User32Dll.RegisterWindowMessage(MessageNameUpdate);
        }

        public void Setup()
        {
            var hwndSourceParams = new HwndSourceParameters(WindowName);
            hwndSourceParams.PositionX = 0;
            hwndSourceParams.PositionY = 0;

            // 94000C00 is from the Start button
            hwndSourceParams.WindowStyle = 0
                | (int)WindowsStyleConstants.WS_VISIBLE			        // 10000000
                | (int)WindowsStyleConstants.WS_CLIPSIBLINGS            // 04000000
                | (int)WindowsStyleConstants.RBS_BANDBORDERS            // 00000400
                | (int)WindowsStyleConstants.RBS_FIXEDORDER             // 00000800
            ;

            hwndSourceParams.ExtendedWindowStyle = 0
                | (int)ExtendedWindowsStyleConstants.WS_EX_TOOLWINDOW   //0x00000080
                | (int)ExtendedWindowsStyleConstants.WS_EX_LAYERED      //0x00080000
                | (int)ExtendedWindowsStyleConstants.WS_EX_TOPMOST      //0x00000008
            ;

            hwndSourceParams.UsesPerPixelOpacity = true;

            _hwndSource = new HwndSource(hwndSourceParams);
            _hwndSource.AddHook(TaskBarIconHookProc);
            _hwndSource.RootVisual = _controlWrapper.Control;

            NativeDll dll = new NativeDll(NativeDllPath);
            dll.SetupSubclass(_hwndSource.Handle);

            UpdatePosition();
        }

        private void UpdatePosition(Rectangle rectIcon, Rectangle rectReBar, Rectangle rectTaskBar)
        {
            var flagsIcon = (uint)(0
                | SetWindowPosConstants.SWP_SHOWWINDOW
                | SetWindowPosConstants.SWP_NOOWNERZORDER
                | SetWindowPosConstants.SWP_NOACTIVATE
                );
            User32Dll.SetWindowPos(_hwndSource.Handle,
                (IntPtr)WindowZOrderConstants.HWND_TOP,
                rectIcon.X, rectIcon.Y, rectIcon.Width, rectIcon.Height,
                flagsIcon);

            User32Dll.SetWindowPos(FindReBar(),
                (IntPtr)WindowZOrderConstants.HWND_TOP,
                rectReBar.X - rectTaskBar.X, rectReBar.Y - rectTaskBar.Y, rectReBar.Width, rectReBar.Height,
                (uint)(SetWindowPosConstants.SWP_NOSENDCHANGING));

            // To relative coords
            rectIcon.X -= rectTaskBar.X;
            rectIcon.Y -= rectTaskBar.Y;
            var p = new CoordsPackager().PackParams(rectIcon);
            User32Dll.PostMessage(FindReBar(), WM_APPDIRECT_UPDATE, p.WParam, p.LParam);
        }

        private void UpdatePosition(Rectangle rectReBar)
        {
            if (_hwndSource == null)
            {
                throw new InvalidOperationException("TaskBarIcon has not been Setup yet");
            }

            TaskbarPosition taskbarPosition = new TaskbarPosition();
            var rectTaskBar = GetTaskBarRectAndPosition(ref taskbarPosition);

            Rectangle rectIcon = CalculateIconRect(rectReBar, taskbarPosition, _offset);
            rectReBar = CalculateRebarRect(rectIcon, rectReBar, taskbarPosition);

            var flagsIcon = (uint)(0
                | SetWindowPosConstants.SWP_SHOWWINDOW
                | SetWindowPosConstants.SWP_NOOWNERZORDER
                | SetWindowPosConstants.SWP_NOACTIVATE
                );
            User32Dll.SetWindowPos(_hwndSource.Handle, 
                (IntPtr)WindowZOrderConstants.HWND_TOP,
                rectIcon.X, rectIcon.Y, rectIcon.Width, rectIcon.Height, 
                flagsIcon);

            User32Dll.SetWindowPos(FindReBar(),
                (IntPtr)WindowZOrderConstants.HWND_TOP,
                rectReBar.X - rectTaskBar.X, rectReBar.Y - rectTaskBar.Y, rectReBar.Width, rectReBar.Height,
                (uint)(SetWindowPosConstants.SWP_NOSENDCHANGING));

            // To relative coords
            rectIcon.X -= rectTaskBar.X;
            rectIcon.Y -= rectTaskBar.Y;
            var p = new CoordsPackager().PackParams(rectIcon);
            User32Dll.PostMessage(FindReBar(), WM_APPDIRECT_UPDATE, p.WParam, p.LParam);
        }

        public void UpdatePosition()
        {
            Rectangle rectReBar = GetReBarRect();
            UpdatePosition(rectReBar);
        }

        public void TearDown()
        {
            if (_hwndSource != null)
            {
                _hwndSource.Dispose();
                _hwndSource = null;
            }

            NativeDll dll = new NativeDll(NativeDllPath);
            dll.TearDownSubclass();
        }

        private void _controlWrapper_DesiredWidthChanged(object sender, EventArgs e)
        {
            UpdatePosition();
        }

        private void _controlWrapper_DesiredHeightChanged(object sender, EventArgs e)
        {
            UpdatePosition();
        }

        private Rectangle GetTaskBarRectAndPosition(ref TaskbarPosition taskbarPosition)
        {
            IntPtr hwndTaskBar = FindTaskBar();
            if (hwndTaskBar == IntPtr.Zero)
            {
                throw new InteropException("ReBar not found");
            }

            RectWin rect = new RectWin();
            if (!User32Dll.GetWindowRect(hwndTaskBar, rect))
            {
                throw new InteropException("Failed to retrieve TaskBar coordinates");
            }

            taskbarPosition = GetTaskbarPosition(hwndTaskBar);
            return RectWinToRectangle(rect);
        }

        private Rectangle GetReBarRect()
        {
            IntPtr hwndTaskBar = FindTaskBar();
            IntPtr hwndReBar = FindReBar(hwndTaskBar);
            if (hwndReBar == IntPtr.Zero)
            {
                throw new InteropException("ReBar not found");
            }

            RectWin rect = new RectWin();
            if (!User32Dll.GetWindowRect(hwndReBar, rect))
            {
                throw new InteropException("Failed to retrieve ReBar coordinates");
            }

            return RectWinToRectangle(rect);
        }

        private Rectangle CalculateIconRect(Rectangle rectReBar, TaskbarPosition taskbarPosition, int offsetReBar)
        {
            var rect = new Rectangle();
            rect.X = rectReBar.Left;
            rect.Y = rectReBar.Top;

            if (taskbarPosition.IsVertical())
            {
                rect.Y -= offsetReBar;
                rect.Width = rectReBar.Width;
                rect.Height = _controlWrapper.DesiredHeight;
            }
            else
            {
                rect.X -= offsetReBar;
                rect.Width = _controlWrapper.DesiredWidth;
                rect.Height = rectReBar.Height;
            }

            return rect;
        }

        private Rectangle CalculateRebarRect(Rectangle rectIcon, Rectangle rectReBar, TaskbarPosition taskbarPosition)
        {
            if (taskbarPosition.IsVertical())
            {
                int delta = rectIcon.Bottom - rectReBar.Top;
                rectReBar.Height -= delta;
                rectReBar.Y += delta;
            }
            else
            {
                int delta = rectIcon.Right - rectReBar.Left;
                rectReBar.Width -= delta;
                rectReBar.X += delta;
            }

            return rectReBar;
        }

        private IntPtr FindTaskBar()
        {
            return User32Dll.FindWindow("Shell_TrayWnd", null);
        }

        private IntPtr FindReBar()
        {
            return FindReBar(FindTaskBar());
        }

        private IntPtr FindReBar(IntPtr hwndTaskBar)
        {
            return User32Dll.FindWindowEx(hwndTaskBar, IntPtr.Zero, "ReBarWindow32", null);
        }

        private TaskbarPosition GetTaskbarPosition(IntPtr hwndTaskBar)
        {
            if (hwndTaskBar == IntPtr.Zero)
            {
                throw new ArgumentException("TaskBar HWND is Zero", "hwndTaskBar");
            }

            var appbar = new APPBARDATA() { hWnd = hwndTaskBar };
            appbar.cbSize = (uint)Marshal.SizeOf(appbar);
            Shell32Dll.SHAppBarMessage((uint)AppBarMessages.ABM_GETTASKBARPOS, ref appbar);

            return (TaskbarPosition)appbar.uEdge;
        }
        private Rectangle RectWinToRectangle(RectWin rect)
        {
            return new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        private IntPtr TaskBarIconHookProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (message == WM_APPDIRECT_UPDATE)
            {
                Rectangle rect = new CoordsPackager().UnpackParams(wParam, lParam);

                TaskbarPosition taskbarPosition = new TaskbarPosition();
                Rectangle taskBarRect = GetTaskBarRectAndPosition(ref taskbarPosition);

                // Make coordinates absolute
                rect.X += taskBarRect.X;
                rect.Y += taskBarRect.Y;

                UpdatePosition(rect);
            }

            return IntPtr.Zero;
        }
    }
}
