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
    public class TaskBarIcon : IDisposable
    {
        private const string NativeDllPath = @"native.dll";
        private const string WindowName = @"AppDirectTaskBarControl";
        private const string MessageNameNativeUpdateOffset = @"AppDirectNativeUpdateOffsetMessage";
        private const string MessageNameManagedReBarUpdated = @"AppDirectManagedReBarUpdatedMessage";

        private uint WM_APPDIRECT_MANAGED_REBAR_UPDATED = 0;
        private uint WM_APPDIRECT_NATIVE_UPDATE_OFFSET = 0;

        private ControlWrapper _controlWrapper;
        private HwndSource _hwndSource;

        public TaskBarIcon(ControlWrapper controlWrapper)
        {
            _controlWrapper = controlWrapper;
            _controlWrapper.DesiredHeightChanged += _controlWrapper_DesiredHeightChanged;
            _controlWrapper.DesiredWidthChanged += _controlWrapper_DesiredWidthChanged;

            WM_APPDIRECT_MANAGED_REBAR_UPDATED = User32Dll.RegisterWindowMessage(MessageNameManagedReBarUpdated);
            WM_APPDIRECT_NATIVE_UPDATE_OFFSET = User32Dll.RegisterWindowMessage(MessageNameNativeUpdateOffset);
        }

        public void Setup()
        {
            TaskBarHelper helper = new TaskBarHelper();
            if (!helper.IsValid())
            {
                throw new InteropException("Failed to get TaskBar details");
            }

            int offset = 0;
            Rectangle rectIcon = CalculateIconRect(helper.TaskBarPosition.IsVertical(), helper.ReBarRect, ref offset);
            Rectangle rectReBar = CalculateRebarRect(helper.TaskBarPosition.IsVertical(), helper.ReBarRect, rectIcon);

            var hwndSourceParams = new HwndSourceParameters(WindowName, rectIcon.Width, rectIcon.Height);
            hwndSourceParams.PositionX = rectIcon.X;
            hwndSourceParams.PositionY = rectIcon.Y;

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

            UpdateReBarPosition(helper, rectReBar);
            UpdateReBarOffset(helper, offset);
        }

        public void TearDown()
        {
            NativeDll dll = new NativeDll(NativeDllPath);
            dll.TearDownSubclass();

            if (_hwndSource != null)
            {
                TaskBarHelper helper = new TaskBarHelper();
                if (!helper.IsValid())
                {
                    throw new InteropException("Failed to get TaskBar details");
                }

                Rectangle rectIcon = helper.GetWindowRectangle(_hwndSource.Handle);
                Rectangle rectReBar = helper.ReBarRect;

                if (helper.TaskBarPosition.IsVertical())
                {
                    int offset = rectReBar.Y - rectIcon.Y;
                    rectReBar.Y -= offset;
                    rectReBar.Height += offset;
                }
                else
                {
                    int offset = rectReBar.X - rectIcon.X;
                    rectReBar.X -= offset;
                    rectReBar.Width += offset;
                }
                UpdateReBarPosition(helper, rectReBar);

                _hwndSource.Dispose();
                _hwndSource = null;
            }
        }

        private void UpdateIconPosition(Rectangle rectIcon)
        {
            var flagsIcon = (uint)(0
                | SetWindowPosConstants.SWP_SHOWWINDOW
                | SetWindowPosConstants.SWP_NOOWNERZORDER
                | SetWindowPosConstants.SWP_NOACTIVATE);
            User32Dll.SetWindowPos(_hwndSource.Handle,
                (IntPtr)WindowZOrderConstants.HWND_TOP,
                rectIcon.X, 
                rectIcon.Y, 
                rectIcon.Width, 
                rectIcon.Height,
                flagsIcon);
        }

        private void UpdateReBarPosition(TaskBarHelper helper, Rectangle rectReBar)
        {
            User32Dll.SetWindowPos(helper.ReBarHwnd,
                (IntPtr)WindowZOrderConstants.HWND_TOP,
                rectReBar.X - helper.TaskBarRect.X,
                rectReBar.Y - helper.TaskBarRect.Y, 
                rectReBar.Width, 
                rectReBar.Height,
                (uint)(SetWindowPosConstants.SWP_NOSENDCHANGING));
        }

        private void UpdateReBarOffset(TaskBarHelper helper, int offset)
        {
            User32Dll.PostMessage(helper.ReBarHwnd, WM_APPDIRECT_NATIVE_UPDATE_OFFSET, new IntPtr(offset), IntPtr.Zero);
        }

        private void UpdateIconSize()
        {
            TaskBarHelper helper = new TaskBarHelper();
            if (!helper.IsValid())
            {
                throw new InteropException("Failed to get TaskBar details");
            }

            Rectangle rectIcon = helper.GetWindowRectangle(_hwndSource.Handle);
            int offset = 0;
            if (helper.TaskBarPosition.IsVertical())
            {
                rectIcon.Height += _controlWrapper.DesiredHeight - rectIcon.Height;
                offset = rectIcon.Height;
            }
            else
            {
                rectIcon.Width += _controlWrapper.DesiredWidth - rectIcon.Width;
                offset = rectIcon.Width;
            }

            UpdateIconPosition(rectIcon);
            UpdateReBarPosition(helper, helper.ReBarRect);
            UpdateReBarOffset(helper, offset);
        }

        private void _controlWrapper_DesiredWidthChanged(object sender, EventArgs e)
        {
            UpdateIconSize();
        }

        private void _controlWrapper_DesiredHeightChanged(object sender, EventArgs e)
        {
            UpdateIconSize();
        }

        private Rectangle CalculateIconRect(bool isVertical, Rectangle rectReBar, ref int offset)
        {
            var rect = rectReBar;

            if (isVertical )
            {
                offset = _controlWrapper.DesiredHeight;
                rect.Height = offset;
            }
            else
            {
                offset = _controlWrapper.DesiredWidth;
                rect.Width = offset;
            }

            return rect;
        }

        private Rectangle CalculateRebarRect(bool isVertical, Rectangle rectReBar, Rectangle rectIcon)
        {
            if (isVertical)
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

        private Rectangle RectWinToRectangle(RectWin rect)
        {
            return new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        private IntPtr TaskBarIconHookProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (message == WM_APPDIRECT_MANAGED_REBAR_UPDATED)
            {
                TaskBarHelper helper = new TaskBarHelper();
                if (!helper.IsValid())
                {
                    throw new InteropException("Failed to get TaskBar details");
                }

                // Switch to absolute coordinates
                Rectangle rectReBar = new CoordsPackager().UnpackParams(wParam, lParam);
                rectReBar.X += helper.TaskBarRect.X;
                rectReBar.Y += helper.TaskBarRect.Y;

                int offset = 0;
                Rectangle rectIcon = CalculateIconRect(helper.TaskBarPosition.IsVertical(), rectReBar, ref offset);
                rectReBar = CalculateRebarRect(helper.TaskBarPosition.IsVertical(), rectReBar, rectIcon);

                UpdateIconPosition(rectIcon);
                UpdateReBarPosition(helper, helper.ReBarRect);
                UpdateReBarOffset(helper, offset);
            }

            return IntPtr.Zero;
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}
