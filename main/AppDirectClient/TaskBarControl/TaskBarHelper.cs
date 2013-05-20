using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TaskBarControl
{
    public class TaskBarHelper
    {
        public TaskBarHelper()
        {
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
        }

        public IntPtr TaskBarHwnd { get; private set; }
        public IntPtr ReBarHwnd { get; private set; }
        public Rectangle TaskBarRect { get; private set; }
        public Rectangle ReBarRect { get; private set; }
        public TaskbarPosition TaskBarPosition { get; private set; }

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
