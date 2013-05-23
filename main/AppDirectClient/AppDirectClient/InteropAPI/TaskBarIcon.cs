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
    public class TaskBarIcon : IDisposable
    {
        private const string NativeDllPath = @"native.dll";
        private const string WindowName = @"AppDirectTaskBarControl";
        private const string MessageNameNativeUpdateOffset = @"AppDirectNativeUpdateOffsetMessage";
        private const string MessageNameManagedReBarUpdated = @"AppDirectManagedReBarUpdatedMessage";
        private const string MessageNameManagedTaskBarUpdated = @"AppDirectManagedTaskBarUpdatedMessage";

        private uint WM_APPDIRECT_NATIVE_UPDATE_OFFSET = 0;
        private uint WM_APPDIRECT_MANAGED_REBAR_UPDATED = 0;
        private uint WM_APPDIRECT_MANAGED_TASKBAR_UPDATED = 0;

        private ControlWrapper _controlWrapper;
        private HwndSource _hwndSource;

        public ControlWrapper Wrapper { get { return _controlWrapper; } }

        public TaskBarIcon(ControlWrapper controlWrapper)
        {
            _controlWrapper = controlWrapper;
            _controlWrapper.DesiredOffsetChanged += _controlWrapper_DesiredOffsetChanged;

            WM_APPDIRECT_NATIVE_UPDATE_OFFSET = User32Dll.RegisterWindowMessage(MessageNameNativeUpdateOffset);
            WM_APPDIRECT_MANAGED_REBAR_UPDATED = User32Dll.RegisterWindowMessage(MessageNameManagedReBarUpdated);
            WM_APPDIRECT_MANAGED_TASKBAR_UPDATED = User32Dll.RegisterWindowMessage(MessageNameManagedTaskBarUpdated);
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

            UpdateReBarOffset(helper, offset);
            UpdateReBarPosition(helper, rectReBar);
            _controlWrapper.AllowedSize = rectIcon.Size;
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

            _controlWrapper.AllowedSize = rectIcon.Size;
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
            User32Dll.SendMessage(helper.ReBarHwnd, WM_APPDIRECT_NATIVE_UPDATE_OFFSET, new IntPtr(offset), IntPtr.Zero);
        }

        private void UpdateIconSize()
        {
            TaskBarHelper helper = new TaskBarHelper();
            if (!helper.IsValid())
            {
                throw new InteropException("Failed to get TaskBar details");
            }

            Rectangle rectIcon = helper.GetWindowRectangle(_hwndSource.Handle);
            Rectangle rectReBar = helper.ReBarRect;
            int oldOffset = 0;
            int offset = _controlWrapper.DesiredOffset;
            if (helper.TaskBarPosition.IsVertical())
            {
                oldOffset = rectIcon.Height;
                rectIcon.Height = offset;

                int delta = offset - oldOffset;
                rectReBar.Y += delta;
                rectReBar.Height -= delta;
            }
            else
            {
                oldOffset = rectIcon.Width;
                rectIcon.Width = offset;

                int delta = offset - oldOffset;
                rectReBar.X += delta;
                rectReBar.Width -= delta;
            }

            UpdateReBarOffset(helper, offset);
            UpdateIconPosition(rectIcon);
            UpdateReBarPosition(helper, rectReBar);
        }

        private void _controlWrapper_DesiredOffsetChanged(object sender, EventArgs e)
        {
            UpdateIconSize();
        }

        private Rectangle CalculateIconRect(bool isVertical, Rectangle rectReBar, ref int offset)
        {
            var rect = rectReBar;

            if (isVertical )
            {
                offset = _controlWrapper.DesiredOffset;
                rect.Height = offset;
            }
            else
            {
                offset = _controlWrapper.DesiredOffset;
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
                Rectangle rectReBar = helper.RectWinToRectangle(new CoordsPackager().UnpackParams(wParam, lParam));
                rectReBar.X += helper.TaskBarRect.X;
                rectReBar.Y += helper.TaskBarRect.Y;

                int offset = 0;
                Rectangle rectIcon = CalculateIconRect(helper.TaskBarPosition.IsVertical(), rectReBar, ref offset);
                rectReBar = CalculateRebarRect(helper.TaskBarPosition.IsVertical(), rectReBar, rectIcon);

                UpdateReBarOffset(helper, offset);
                UpdateIconPosition(rectIcon);
                UpdateReBarPosition(helper, helper.ReBarRect);
            }
            else if (message == WM_APPDIRECT_MANAGED_TASKBAR_UPDATED)
            {
                TaskBarHelper helper = new TaskBarHelper();
                if (!helper.IsValid())
                {
                    throw new InteropException("Failed to get TaskBar details");
                }
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
                UpdateIconPosition(rectIcon);
            }

            return IntPtr.Zero;
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}
