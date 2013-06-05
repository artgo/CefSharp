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

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class TaskbarHost : IDisposable, ITaskbarHost
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
        private ITaskbarControl _taskBarControl;
        private int _desiredOffset;
        private HwndSource _hwndSource;

        public Control Wrapper { get { return _control; } }

        public TaskbarHost(Control control)
        {
            _control = control;

            var helper = ServiceLocator.GetTaskbarHelper();
            _desiredOffset = (int)(helper.TaskBarPosition.IsVertical() ? _control.Height : _control.Width);

            WM_APPDIRECT_NATIVE_UPDATE_OFFSET = User32Dll.RegisterWindowMessage(MessageNameNativeUpdateOffset);
            WM_APPDIRECT_MANAGED_REBAR_UPDATED = User32Dll.RegisterWindowMessage(MessageNameManagedReBarUpdated);
            WM_APPDIRECT_MANAGED_TASKBAR_UPDATED = User32Dll.RegisterWindowMessage(MessageNameManagedTaskBarUpdated);
        }

        ~TaskbarHost()
        {
            Dispose(false);
        }

        public void SetTaskBarControl(ITaskbarControl control)
        {
            _taskBarControl = control;
        }

        public void Setup()
        {
            ITaskbarHelper helper = ServiceLocator.GetTaskbarHelper();

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

            var dll = ServiceLocator.GetNativeDll(NativeDllPath);
            dll.SetupSubclass(_hwndSource.Handle);

            UpdateReBarOffset(helper.ReBarHwnd, _desiredOffset);
            UpdateReBarPosition(helper.ReBarHwnd, helper.ScreenToClient(helper.TaskBarHwnd, rectReBar));
            _taskBarControl.SetAllowedSize(rectIcon.Width, rectIcon.Height);
        }

        public void TearDown()
        {
            var dll = ServiceLocator.GetNativeDll(NativeDllPath);
            dll.TearDownSubclass();

            if (_hwndSource != null)
            {
                _hwndSource.Dispose();
                _hwndSource = null;
            }
        }

        private void UpdateIconPosition(Rectangle rectIcon, Rectangle rectScreen)
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

            _taskBarControl.SetAllowedSize(rectIcon.Width, rectIcon.Height);
        }

        private void UpdateReBarPosition(IntPtr reBarHwnd, Rectangle rectReBar)
        {
            User32Dll.SetWindowPos(reBarHwnd,
                (IntPtr)WindowZOrderConstants.HWND_TOP,
                rectReBar.X,
                rectReBar.Y, 
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
            var helper = ServiceLocator.GetTaskbarHelper();

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
            UpdateReBarPosition(helper.ReBarHwnd, helper.ScreenToClient(helper.TaskBarHwnd, rectReBar));
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
                var helper = ServiceLocator.GetTaskbarHelper();

                // Switch to absolute coordinates
                Rectangle rectReBar = helper.RectWinToRectangle(new CoordsPackager().UnpackParams(wParam, lParam));
                rectReBar = helper.ClientToScreen(helper.TaskBarHwnd, rectReBar);

                Rectangle rectIcon = CalculateIconRectFromReBar(helper.TaskBarPosition.IsVertical(), rectReBar, _desiredOffset);
                rectReBar = CalculateRebarRectWithIcon(helper.TaskBarPosition.IsVertical(), rectReBar, rectIcon);

                UpdateReBarOffset(helper.ReBarHwnd, _desiredOffset);
                UpdateIconPosition(rectIcon, helper.TaskbarScreen.Bounds);
                UpdateReBarPosition(helper.ReBarHwnd, helper.ScreenToClient(helper.TaskBarHwnd, rectReBar));
            }
            else if (message == WM_APPDIRECT_MANAGED_TASKBAR_UPDATED)
            {
                var helper = ServiceLocator.GetTaskbarHelper();

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
