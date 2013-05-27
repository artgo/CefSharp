using AppDirect.WindowsClient.InteropAPI;
using AppDirect.WindowsClient.InteropAPI.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace AppDirect.WindowsClient.InteropAPI
{
    public class TaskBarIcon : IDisposable, ITaskBarHost
    {
        private const string NativeDllPath = @"native.dll";
        private const string WindowName = @"AppDirectTaskBarControl";
        private const string MessageNameNativeUpdateOffset = @"AppDirectNativeUpdateOffsetMessage";
        private const string MessageNameManagedReBarUpdated = @"AppDirectManagedReBarUpdatedMessage";
        private const string MessageNameManagedTaskBarUpdated = @"AppDirectManagedTaskBarUpdatedMessage";

        private uint WM_APPDIRECT_NATIVE_UPDATE_OFFSET = 0;
        private uint WM_APPDIRECT_MANAGED_REBAR_UPDATED = 0;
        private uint WM_APPDIRECT_MANAGED_TASKBAR_UPDATED = 0;

        private Control _control;
        private ITaskBarControl _taskBarControl;
        private int _desiredOffset;
        private HwndSource _hwndSource;

        public Control Wrapper { get { return _control; } }

        public TaskBarIcon(Control control)
        {
            _control = control;

            TaskBarHelper helper = new TaskBarHelper();
            _desiredOffset = (int)(helper.TaskBarPosition.IsVertical() ? _control.Height : _control.Width);

            WM_APPDIRECT_NATIVE_UPDATE_OFFSET = User32Dll.RegisterWindowMessage(MessageNameNativeUpdateOffset);
            WM_APPDIRECT_MANAGED_REBAR_UPDATED = User32Dll.RegisterWindowMessage(MessageNameManagedReBarUpdated);
            WM_APPDIRECT_MANAGED_TASKBAR_UPDATED = User32Dll.RegisterWindowMessage(MessageNameManagedTaskBarUpdated);
        }

        ~TaskBarIcon()
        {
            Dispose(false);
        }

        public void SetTaskBarControl(ITaskBarControl control)
        {
            _taskBarControl = control;
        }

        public void Setup()
        {
            TaskBarHelper helper = new TaskBarHelper();

            Rectangle rectIcon = CalculateIconRectFromReBar(helper.TaskBarPosition.IsVertical(), helper.ReBarRect, _desiredOffset);
            Rectangle rectReBar = CalculateRebarRectWithIcon(helper.TaskBarPosition.IsVertical(), helper.ReBarRect, rectIcon);

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
            _hwndSource.RootVisual = _control;

            NativeDll dll = new NativeDll(NativeDllPath);
            dll.SetupSubclass(_hwndSource.Handle);

            UpdateReBarOffset(helper.ReBarHwnd, _desiredOffset);
            UpdateReBarPosition(helper.TaskBarRect, helper.ReBarHwnd, rectReBar);
            _taskBarControl.SetAllowedSize(rectIcon.Width, rectIcon.Height);
        }

        public void TearDown()
        {
            NativeDll dll = new NativeDll(NativeDllPath);
            dll.TearDownSubclass();

            if (_hwndSource != null)
            {
                TaskBarHelper helper = new TaskBarHelper();

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
                UpdateReBarPosition(helper.TaskBarRect, helper.ReBarHwnd, rectReBar);

                _hwndSource.Dispose();
                _hwndSource = null;
            }
        }

        private void UpdateIconPosition(Rectangle rectIcon, Rectangle rectScreen)
        {
            User32Dll.SetWindowRgn(_hwndSource.Handle, IntPtr.Zero, true);

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
            System.Drawing.Point topLeft = new System.Drawing.Point(rectScreen.Left, rectScreen.Top);
            User32Dll.ScreenToClient(_hwndSource.Handle, ref topLeft);
            rectScreen.Y = topLeft.Y;
            rectScreen.X = topLeft.X;
            IntPtr hRgn = Gdi32Dll.CreateRectRgn(rectScreen.Left, rectScreen.Top, rectScreen.Right, rectScreen.Bottom);
            if (hRgn == IntPtr.Zero)
            {
                throw new Exception("Failed to create HRGN");
            }
            User32Dll.SetWindowRgn(_hwndSource.Handle, hRgn, true);
            Gdi32Dll.DeleteObject(hRgn);

            _taskBarControl.SetAllowedSize(rectIcon.Width, rectIcon.Height);
        }

        private void UpdateReBarPosition(Rectangle taskBarRect, IntPtr reBarHwnd, Rectangle rectReBar)
        {
            User32Dll.SetWindowPos(reBarHwnd,
                (IntPtr)WindowZOrderConstants.HWND_TOP,
                rectReBar.X - taskBarRect.X,
                rectReBar.Y - taskBarRect.Y, 
                rectReBar.Width, 
                rectReBar.Height,
                (uint)(SetWindowPosConstants.SWP_NOSENDCHANGING));
        }

        private void UpdateReBarOffset(IntPtr reBarHwnd, int offset)
        {
            User32Dll.SendMessage(reBarHwnd, WM_APPDIRECT_NATIVE_UPDATE_OFFSET, new IntPtr(offset), IntPtr.Zero);
        }

        private void UpdateIconSize()
        {
            TaskBarHelper helper = new TaskBarHelper();

            Rectangle rectIcon = helper.GetWindowRectangle(_hwndSource.Handle);
            Rectangle rectReBar = helper.ReBarRect;
            int oldOffset = 0;
            if (helper.TaskBarPosition.IsVertical())
            {
                oldOffset = rectIcon.Height;
                rectIcon.Height = _desiredOffset;

                int delta = _desiredOffset - oldOffset;
                rectReBar.Y += delta;
                rectReBar.Height -= delta;
            }
            else
            {
                oldOffset = rectIcon.Width;
                rectIcon.Width = _desiredOffset;

                int delta = _desiredOffset - oldOffset;
                rectReBar.X += delta;
                rectReBar.Width -= delta;
            }

            UpdateReBarOffset(helper.ReBarHwnd, _desiredOffset);
            UpdateIconPosition(rectIcon, helper.TaskbarScreen.Bounds);
            UpdateReBarPosition(helper.TaskBarRect, helper.ReBarHwnd, rectReBar);
        }

        private Rectangle CalculateIconRectFromReBar(bool isVertical, Rectangle rectReBar, int offset)
        {
            var rect = rectReBar;

            if (isVertical )
            {
                rect.Height = offset;
            }
            else
            {
                rect.Width = offset;
            }

            return rect;
        }

        private Rectangle CalculateRebarRectWithIcon(bool isVertical, Rectangle rectReBar, Rectangle rectIcon)
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

        private IntPtr TaskBarIconHookProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (message == WM_APPDIRECT_MANAGED_REBAR_UPDATED)
            {
                TaskBarHelper helper = new TaskBarHelper();

                // Switch to absolute coordinates
                Rectangle rectReBar = helper.RectWinToRectangle(new CoordsPackager().UnpackParams(wParam, lParam));
                rectReBar.X += helper.TaskBarRect.X;
                rectReBar.Y += helper.TaskBarRect.Y;

                Rectangle rectIcon = CalculateIconRectFromReBar(helper.TaskBarPosition.IsVertical(), rectReBar, _desiredOffset);
                rectReBar = CalculateRebarRectWithIcon(helper.TaskBarPosition.IsVertical(), rectReBar, rectIcon);

                UpdateReBarOffset(helper.ReBarHwnd, _desiredOffset);
                UpdateIconPosition(rectIcon, helper.TaskbarScreen.Bounds);
                UpdateReBarPosition(helper.TaskBarRect, helper.ReBarHwnd, rectReBar);
            }
            else if (message == WM_APPDIRECT_MANAGED_TASKBAR_UPDATED)
            {
                TaskBarHelper helper = new TaskBarHelper();

                Rectangle rectIcon = helper.GetWindowRectangle(_hwndSource.Handle);
                rectIcon.X = helper.ReBarRect.X;
                rectIcon.Y = helper.ReBarRect.Y;
                if (helper.TaskBarPosition.IsVertical())
                {
                    rectIcon.Y -= rectIcon.Height;
                }
                else
                {
                    rectIcon.X -= rectIcon.Width;
                }
                UpdateIconPosition(rectIcon, helper.TaskbarScreen.Bounds);
            }

            return IntPtr.Zero;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            TearDown();
        }

        public void SetDesiredOffset(int offset)
        {
            if (_desiredOffset != offset)
            {
                _desiredOffset = offset;
                UpdateIconSize();
            }
        }
    }
}
