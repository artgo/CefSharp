using AppDirect.WindowsClient.Common.Log;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class TaskbarHelper : ITaskbarHelper
    {
        private const string SmallIconsPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
        private const string SmallIconsFiledName = @"TaskbarSmallIcons";
        private const string DpiSettingPath = @"HKEY_CURRENT_USER\Control Panel\Desktop\Windowmetrics";
        private const string DpiSettingName = @"AppliedDPI";
        private const double StandardDpi = 96;

        public TaskbarHelper()
        {
            var osVersion = Environment.OSVersion.Version;
            var ver6OrUp = osVersion.Major >= 6;
            IsVistaOrUp = ver6OrUp;
            IsWin7OrUp = ver6OrUp && (osVersion.Minor >= 1);
            IsWin8OrUp = ver6OrUp && (osVersion.Minor >= 2);
        }

        public bool IsVistaOrUp { get; private set; }

        public bool IsWin7OrUp { get; private set; }

        public bool IsWin8OrUp { get; private set; }

        private IntPtr _taskBarHwnd;
        private IntPtr _reBarHwnd;
        private bool _taskBarRectLoaded = false;
        private Rectangle _taskBarRect;
        private bool _reBarRectLoaded = false;
        private Rectangle _reBarRect;
        private bool _taskBarPositionLoaded = false;
        private TaskbarPosition _taskBarPosition;
        private bool _taskBarIconsSizeLoaded = false;
        private TaskbarIconsSize _taskBarIconsSize;
        private Double _dpiScalingFactor;

        public Screen TaskbarScreen { get { return Screen.FromHandle(TaskBarHwnd); } }

        public IntPtr TaskBarHwnd
        {
            get
            {
                if (_taskBarHwnd == IntPtr.Zero || !User32Dll.IsWindow(_taskBarHwnd))
                {
                    _taskBarHwnd = FindTaskBar();

                    if (_taskBarHwnd == IntPtr.Zero)
                    {
                        throw new Exception("TaskBar couldn't be found");
                    }
                }
                return _taskBarHwnd;
            }
        }

        public IntPtr ReBarHwnd
        {
            get
            {
                if (_reBarHwnd == IntPtr.Zero || !User32Dll.IsWindow(_reBarHwnd))
                {
                    _reBarHwnd = FindReBar(TaskBarHwnd);
                    if (_reBarHwnd == IntPtr.Zero)
                    {
                        throw new Exception("ReBar couldn't be found");
                    }
                }
                return _reBarHwnd;
            }
        }

        public Rectangle TaskBarRect
        {
            get
            {
                if (!_taskBarRectLoaded)
                {
                    _taskBarRect = GetWindowRectangle(TaskBarHwnd);
                    _taskBarRectLoaded = true;
                }
                return _taskBarRect;
            }
        }

        public Rectangle ReBarRect
        {
            get
            {
                if (!_reBarRectLoaded)
                {
                    _reBarRect = GetWindowRectangle(ReBarHwnd);
                    _reBarRectLoaded = true;
                }
                return _reBarRect;
            }
        }

        public TaskbarPosition TaskBarPosition
        {
            get
            {
                if (!_taskBarPositionLoaded)
                {
                    var appbar = new APPBARDATA();
                    appbar.hWnd = TaskBarHwnd;
                    appbar.cbSize = (uint)Marshal.SizeOf(appbar);
                    if (Shell32Dll.SHAppBarMessage((uint)AppBarMessages.ABM_GETTASKBARPOS, ref appbar) == IntPtr.Zero)
                    {
                        throw new Exception("Failed to get TaskBar position");
                    }
                    else
                    {
                        _taskBarPosition = (TaskbarPosition)appbar.uEdge;
                    }
                    _taskBarPositionLoaded = true;
                }
                return _taskBarPosition;
            }
        }

        public TaskbarIconsSize TaskBarIconsSize
        {
            get
            {
                if (!_taskBarIconsSizeLoaded)
                {
                    _taskBarIconsSize = TaskbarIconsSize.Small;

                    if (IsWin7OrUp)
                    {
                        // Defaulting to large icons on Windows 7 and 8
                        _taskBarIconsSize = TaskbarIconsSize.Large;

                        var val = Registry.GetValue(SmallIconsPath, SmallIconsFiledName, _taskBarIconsSize);

                        if ((val is int) && ((int)val) == IconsSize.SMALL)
                        {
                            _taskBarIconsSize = TaskbarIconsSize.Small;
                        }
                    }
                    _taskBarIconsSizeLoaded = true;
                }
                return _taskBarIconsSize;
            }
        }

        public double DpiScalingFactor
        {
            get
            {
                if (_dpiScalingFactor == 0)
                {
                    var strVal = Registry.GetValue(DpiSettingPath, DpiSettingName, StandardDpi);
                    double dblVal;
                    if (Double.TryParse(strVal.ToString(), out dblVal))
                    {
                        _dpiScalingFactor = dblVal / StandardDpi;
                    }
                    else
                    {
                        throw new InteropException("Failed to get monitor DPI");
                    }
                }

                return _dpiScalingFactor;
            }
        }

        public Rectangle GetWindowRectangle(IntPtr hwnd)
        {
            RECT rect = new RECT();
            if (!User32Dll.GetWindowRect(hwnd, rect))
            {
                throw new Exception("Task Bar couldn't be found");
            }
            return RectWinToRectangle(rect);
        }

        public Rectangle RectWinToRectangle(RECT rectWin)
        {
            return new Rectangle(rectWin.Left, rectWin.Top, rectWin.Width, rectWin.Height);
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

        public Rectangle ScreenToClient(IntPtr hWnd, Rectangle screenRect)
        {
            Point pt = new Point(screenRect.X, screenRect.Y);
            if (User32Dll.ScreenToClient(hWnd, ref pt))
            {
                screenRect.X = pt.X;
                screenRect.Y = pt.Y;
            }
            return screenRect;
        }

        public Rectangle ClientToScreen(IntPtr hWnd, Rectangle clientRect)
        {
            Point pt = new Point(clientRect.X, clientRect.Y);
            if (User32Dll.ClientToScreen(hWnd, ref pt))
            {
                clientRect.X = pt.X;
                clientRect.Y = pt.Y;
            }
            return clientRect;
        }

        public void WaitForRebar(ILogger logger)
        {
            var _reBarRect = Rectangle.Empty;
            while (_reBarRect.IsEmpty || _reBarRect.Width < 5 || _reBarRect.Height < 5)
            {
                try
                {
                    FindReBar();
                    _reBarRect = GetWindowRectangle(ReBarHwnd);
                }
                catch (Exception e)
                {
                    logger.Info(String.Format("RebarRect was not set yet because exception was thrown: {0}", e));
                    ServiceLocator.UiHelper.Sleep(200);
                }
            }
        }
    }
}