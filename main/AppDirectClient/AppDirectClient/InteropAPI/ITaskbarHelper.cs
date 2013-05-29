using AppDirect.WindowsClient.InteropAPI.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AppDirect.WindowsClient.InteropAPI
{
    public interface ITaskbarHelper
    {
        bool IsVistaOrUp { get; }
        bool IsWin7OrUp { get; }
        bool IsWin8OrUp { get; }

        Screen TaskbarScreen { get; }
        IntPtr TaskBarHwnd { get; }
        IntPtr ReBarHwnd { get; }
        Rectangle TaskBarRect { get; }
        Rectangle ReBarRect { get; }
        TaskbarPosition TaskBarPosition { get; }
        TaskbarIconsSize TaskBarIconsSize { get; }
        double DpiScalingFactor { get; }

        Rectangle GetWindowRectangle(IntPtr hwnd);
        Rectangle RectWinToRectangle(RECT rectWin);
    }
}
