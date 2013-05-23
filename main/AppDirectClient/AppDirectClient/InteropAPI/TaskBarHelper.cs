using AppDirect.WindowsClient.InteropAPI;
using AppDirect.WindowsClient.InteropAPI.Internal;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AppDirect.WindowsClient.InteropAPI
{
    public class TaskBarHelper
    {
        private const string SmallIconsPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
        private const string SmallIconsFiledName = @"TaskbarSmallIcons";
        private const string DpiSettingPath = @"HKEY_CURRENT_USER\Control Panel\Desktop\Windowmetrics";
        private const string DpiSettingName = @"AppliedDPI";
        private const double StandardDpi = 96;

        public TaskBarHelper()
        {
            var osVersion = Environment.OSVersion.Version;
            var ver6OrUp = osVersion.Major >= 6;
            IsVistaOrUp = ver6OrUp;
            IsWin7OrUp = ver6OrUp && (osVersion.Minor >= 1);
            IsWin8OrUp = ver6OrUp && (osVersion.Minor >= 2);

            if (!User32Dll.IsWindow(TaskBarHwnd))
            {
                TaskBarHwnd = FindTaskBar();
            }
            if (!User32Dll.IsWindow(ReBarHwnd))
            {
                ReBarHwnd = FindReBar(TaskBarHwnd);
            }

            if (TaskBarHwnd == IntPtr.Zero)
            {
                throw new Exception("TaskBar couldn't be found");
            } 
            else 
            {
                TaskBarRect = GetWindowRectangle(TaskBarHwnd);
            }

            if (ReBarHwnd == IntPtr.Zero)
            {
                throw new Exception("ReBar couldn't be found");
            }
            else
            {
                ReBarRect = GetWindowRectangle(ReBarHwnd);
            }

            var appbar = new APPBARDATA();
            appbar.hWnd = TaskBarHwnd;
            appbar.cbSize = (uint)Marshal.SizeOf(appbar);
            if (Shell32Dll.SHAppBarMessage((uint)AppBarMessages.ABM_GETTASKBARPOS, ref appbar) == IntPtr.Zero)
            {
                throw new Exception("Failed to get TaskBar position");
            }
            else
            {
                TaskBarPosition = (TaskbarPosition)appbar.uEdge;
            };

            // Default icon size for XP and Vista is small
            TaskBarIconSize = TaskbarIconsSize.Small;

            if (IsWin7OrUp)
            {
                // Defaulting to large icons on Windows 7 and 8
                TaskBarIconSize = TaskbarIconsSize.Large;

                try
                {
                    var val = Registry.GetValue(SmallIconsPath, SmallIconsFiledName, TaskBarIconSize);

                    if ((val is int) && ((int)val) == IconsSize.SMALL)
                    {
                        TaskBarIconSize = TaskbarIconsSize.Small;
                    }
                }
                catch (Exception)
                {
                    // Refactor: log
                }
            }

            DpiScalingFactor = (double)(int)(Registry.GetValue(DpiSettingPath, DpiSettingName, StandardDpi) ?? StandardDpi) / StandardDpi;
        }

        public bool IsVistaOrUp { get; private set; }
        public bool IsWin7OrUp { get; private set; }
        public bool IsWin8OrUp { get; private set; }
        public IntPtr TaskBarHwnd { get; private set; }
        public IntPtr ReBarHwnd { get; private set; }
        public Rectangle TaskBarRect { get; private set; }
        public Rectangle ReBarRect { get; private set; }
        public TaskbarPosition TaskBarPosition { get; private set; }
        public TaskbarIconsSize TaskBarIconSize { get; private set; }
        public double DpiScalingFactor { get; private set; }
        
        public bool TaskBarIsVertical { get { return TaskBarPosition.IsVertical(); } }

        public bool IsValid() 
        {
            return User32Dll.IsWindow(TaskBarHwnd) && User32Dll.IsWindow(ReBarHwnd); 
        }

        public Rectangle GetWindowRectangle(IntPtr hwnd)
        {
            RectWin rect = new RectWin();
            if (!User32Dll.GetWindowRect(hwnd, rect))
            {
                throw new Exception("Task Bar couldn't be found");
            }
            return RectWinToRectangle(rect);
        }

        public Rectangle RectWinToRectangle(RectWin rectWin)
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
    }
}
