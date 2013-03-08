using Accessibility;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls;
using System.Windows.Interop;
using DWORD = System.UInt32;
using HMONITOR = System.IntPtr;
using HWND = System.IntPtr;
using LPARAM = System.IntPtr;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class InteractionsObject : ITaskbarInteropCallback
    {
        #region field members

        private static readonly IntPtr NULL = IntPtr.Zero;
        private readonly object _lockObject = new object();
        private const string SmallIconsPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
        private const string SmallIconsFiledName = @"TaskbarSmallIcons";
        private const string DpiSettingPath = @"HKEY_CURRENT_USER\Control Panel\Desktop\Windowmetrics";
        private const string DpiSettingName = @"AppliedDPI";
        private const int BuffSize = 256;
        private const double StandardDpi = 96;
        private const string StartButtonClass = @"Button";

        // win 7  default with large buttons
        private const int DefaultStartButtonWidth = 54;

        private const int DefaultStartButtonHeight = 40;
        private const string CloseMessageName = @"AppDirectForceApplicationCloseMessage";
        private const string WindowName = @"AppDirectTaskbarButtonsWindow";
        private readonly static bool IsVistaOrUp;
        private readonly static bool IsWin7OrUp;
        private readonly static bool IsWin8OrUp;

        private readonly CoordsPackager _coordsPackager = new CoordsPackager();

        private volatile HwndSource _hSrc;
        private volatile ITaskbarInterop _notifyee = null;
        private System.Drawing.Size _buttonsWindowSize;
        private volatile IntPtr _hWinEventHook = IntPtr.Zero;
        private volatile WinEventDelegate _winEventProc;
        private volatile RegistryChangeHandler _smallIconsChangeProc;
        private volatile RegistryChangeHandler _smallIconsChangeProcError;
        private volatile HwndSourceHook _wndProcHook;
        private volatile int _taskbarHeight = 0;
        private volatile int _buttonsWidth = 0;
        private volatile TaskbarPosition _taskbarPosition = TaskbarPosition.Bottom;
        private volatile TaskbarIconsSize _taskbarIconsSize = TaskbarIconsSize.Large;
        private volatile RegistryChangeMonitor _smallIconsRegistryMonitor;
        private volatile HWND _taskbarHwnd = NULL;
        private volatile HWND _rebarHwnd = NULL;
        private volatile HWND _startButtonHwnd = NULL;
        private volatile uint _updateMessageId = 0;
        private volatile uint _closeMessageId = 0;
        private double _dpiScalingFactor;

        public int TaskbarHeight { get { return _taskbarHeight; } }

        public TaskbarPosition TaskbarPosition { get { return _taskbarPosition; } }

        public TaskbarIconsSize TaskbarIconsSize { get { return _taskbarIconsSize; } }

        #endregion field members

        static InteractionsObject()
        {
            var osVersion = Environment.OSVersion.Version;
            var ver6OrUp = osVersion.Major >= 6;
            IsVistaOrUp = ver6OrUp;
            IsWin7OrUp = ver6OrUp && (osVersion.Minor >= 1);
            IsWin8OrUp = ver6OrUp && (osVersion.Minor >= 2);
        }

        public void LoadInitialValues()
        {
            UpdateHandles();

            _dpiScalingFactor = GetDpiScaleFactor();

            _taskbarPosition = GetTaskbarEdge();
            _taskbarIconsSize = GetTaskbarIconSize();

            _buttonsWindowSize = GetButtonsWindowSize(true);
            _taskbarHeight = GetTaskbarHeight();
        }

        public void Place(Control wnd, ITaskbarInterop notifyee, int initialWidth)
        {
            if (wnd == null)
            {
                throw new ArgumentNullException("wnd");
            }

            if (notifyee == null)
            {
                throw new ArgumentNullException("notifyee");
            }

            notifyee.TaskbarCallbackEvents = this;
            _notifyee = notifyee;

            if (_taskbarPosition.IsVertical())
            {
                _buttonsWindowSize.Height = initialWidth;
            }
            else
            {
                _buttonsWindowSize.Width = initialWidth;
            }
            _buttonsWidth = initialWidth;
            UpdateHandles();
            _closeMessageId = User32Dll.RegisterWindowMessage(CloseMessageName);

            var pos = CalculateButtonPosition();

            var p = new HwndSourceParameters(
                    WindowName,			// NAME
                    _buttonsWindowSize.Width,
                    _buttonsWindowSize.Height		// size of WPF window inside usual Win32 window
                );
            p.PositionX = pos.X;
            p.PositionY = pos.Y;
            p.ParentWindow = (IsWin8OrUp ? _taskbarHwnd : _startButtonHwnd);

            unchecked
            {
                p.WindowStyle = (int)(0													// TODO: -2 consts from magic numbers

                    //| (uint)0x94000C00												// 94000C00 is from the Start button
                    // | (uint)0x0C00U													// BS_

                    // popup is prohibiting child style on Win8
                    //| (uint)WindowsStyleConstants.WS_POPUP							// 80000000
                    | (uint)WindowsStyleConstants.WS_VISIBLE							// 10000000

                    | (uint)WindowsStyleConstants.WS_CLIPSIBLINGS						// 04000000
                    | (IsWin8OrUp ? (uint)WindowsStyleConstants.WS_CHILD : 0U)		// Since Win8 child can be transparent		0x40000000U
                );

                p.ExtendedWindowStyle = 0
                    | 0x00080088														// 00080088 is from the Start button
                    | 0x00080000														// WS_EX_LAYERED
                    | 0x00000008														// WS_EX_TOPMOST
                ;
            }
            p.UsesPerPixelOpacity = true;		// set the WS_EX_LAYERED extended window style
            _hSrc = new HwndSource(p);
            _hSrc.RootVisual = wnd;

            _wndProcHook = WndProc;
            _hSrc.AddHook(_wndProcHook);

            DoChangeWidth(_buttonsWidth, true);

            NativeDll.InjectExplrorerExe();

            uint taskbarProcessId;
            var taskbarThreadId = User32Dll.GetWindowThreadProcessId(_taskbarHwnd, out taskbarProcessId);

            // Hook resize event catcher
            _winEventProc = WinEventDelegateImpl;
            _hWinEventHook = User32Dll.SetWinEventHook((uint)EventConstants.EVENT_SYSTEM_MOVESIZEEND,
                (uint)EventConstants.EVENT_SYSTEM_MOVESIZEEND, NULL, _winEventProc,
                taskbarProcessId, taskbarThreadId, (uint)(WinEventHookFlags.WINEVENT_OUTOFCONTEXT));

            _smallIconsChangeProc = RegistryChangeHandler;
            _smallIconsChangeProcError = RegistryChangeErrorHandler;

            _smallIconsRegistryMonitor = new RegistryChangeMonitor(SmallIconsPath);
            _smallIconsRegistryMonitor.Changed += _smallIconsChangeProc;
            _smallIconsRegistryMonitor.Error += _smallIconsChangeProcError;
            _smallIconsRegistryMonitor.Start();

            _updateMessageId = NativeDll.GetUpdatePositionMsg();
            _taskbarPosition = GetTaskbarEdge();

            PostPositionToHook();
            UpdatePosition();
        }

        private void UpdateHandles()
        {
            lock (_lockObject)
            {
                _rebarHwnd = NativeDll.FindRebar();
                _taskbarHwnd = NativeDll.FindTaskBar();
                _startButtonHwnd = FindStartButton();
            }
        }

        private void PostPositionToHook()
        {
            var pos = CalculateButtonPosition();

            var taskbarPos = GetTaskbarRect();
            if (_taskbarPosition.IsVertical())
            {
                pos.X -= taskbarPos.Left; // to relative
            }
            else
            {
                pos.Y -= taskbarPos.Top; // to relative
            }

            var coords = new RectWin()
                {
                    Left = pos.X,
                    Top = pos.Y,
                    Width = _buttonsWindowSize.Width,
                    Height = _buttonsWindowSize.Height
                };

            var postParams = _coordsPackager.PackParams(coords);

            User32Dll.PostMessage(_rebarHwnd, _updateMessageId, postParams.WParam, postParams.LParam);
        }

        private void RegistryChangeHandler(object sender, RegistryChangeEventArgs e)
        {
            bool updatePos = CheckIconSize();

            if (updatePos)
            {
                UpdatePosition();
            }

            var newRebarCoords = CalculateRebarCoords(false);

            if (!User32Dll.SetWindowPos(NativeDll.FindRebar(), IntPtr.Zero,
                                        newRebarCoords.Left, newRebarCoords.Top, newRebarCoords.Width, newRebarCoords.Height,
                                        0)) // move to correct coords
            {
                throw new InteropException("Cannot move Rebar back");
            }
        }

        private void RegistryChangeErrorHandler(object sender, RegistryChangeEventArgs e)
        {
            _notifyee.Error(e);
        }

        private TaskbarIconsSize GetTaskbarIconSize()
        {
            var sz = TaskbarIconsSize.Small;	// old versions use small icons
            var regSmall = Registry.GetValue(SmallIconsPath, SmallIconsFiledName, -1);

            if (regSmall != null)				// since Win7
            {
                if ((int)regSmall == IconsSize.LARGE)
                {
                    sz = TaskbarIconsSize.Large;
                }
            }

            return sz;
        }

        private double GetDpiScaleFactor()
        {
            var dpiSetting = (double)(int)(Registry.GetValue(DpiSettingPath, DpiSettingName, StandardDpi) ?? StandardDpi);
            return dpiSetting / StandardDpi;
        }

        private void WinEventDelegateImpl(IntPtr hWinEventHook, uint eventType,
                                      IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            switch ((EventConstants)eventType)
            {
                case EventConstants.EVENT_SYSTEM_MOVESIZEEND:
                    IAccessible accWindow;
                    object varChild;

                    var hr = OleaccDll.AccessibleObjectFromEvent(hwnd, (uint)idObject, (uint)idChild, out accWindow, out varChild);
                    if (hr != HresultCodes.S_OK || accWindow == null)
                    {
                        return;
                    }

                    var newRect = new RectWin();
                    accWindow.accLocation(out newRect.Left, out newRect.Top, out newRect.Right, out newRect.Bottom, varChild);

                    //ReactToSizeMove();

                    break;
            }
        }

        private void ReactToSizeMove()
        {
            UpdateHandles();

            var updatePos = false;
            var oldHeight = _buttonsWindowSize.Height;
            var oldWidth = _buttonsWindowSize.Width;
            _buttonsWindowSize = GetButtonsWindowSize();

            var newTaskbarPosition = GetTaskbarEdge();
            if (newTaskbarPosition != _taskbarPosition)
            {
                var orientationChanged = (newTaskbarPosition.IsVertical() != _taskbarPosition.IsVertical());
                if (orientationChanged)
                {
                    if (newTaskbarPosition.IsVertical())
                    {
                        _buttonsWindowSize.Height = oldWidth;
                        _buttonsWidth = oldWidth;
                    }
                    else
                    {
                        _buttonsWindowSize.Width = oldHeight;
                        _buttonsWidth = oldHeight;
                    }
                }
                _taskbarPosition = newTaskbarPosition;
                // We should reinsert buttons only if going from horizontal to vertical mode or vica versa
                // it is because shift in C++ code is calculated in relative coordinates, so it will keep exactly
                // the same shift for us moving from top to bottom or from left to right edges.
                DoChangeWidth(_buttonsWidth, orientationChanged);
                _notifyee.PositionChanged(newTaskbarPosition);
                updatePos = true;
            }

            if (CheckIconSize())
            {
                updatePos = true;
            }

            if (CheckUpdatePos())
            {
                updatePos = true;
            }

            if (updatePos)
            {
                UpdatePosition();
            }
        }

        private bool CheckUpdatePos()
        {
            var newHeight = GetTaskbarHeight();
            if (newHeight != _taskbarHeight)
            {
                _taskbarHeight = newHeight;
                _notifyee.HeightChanged(newHeight);
                return true;
            }
            return false;
        }

        private int GetTaskbarHeight()
        {
            var taskbarRect = GetTaskbarRect();
            int newHeight;
            if (_taskbarPosition.IsVertical())
            {
                newHeight = taskbarRect.Width;
            }
            else
            {
                newHeight = taskbarRect.Height;
            }
            return newHeight;
        }

        private void UpdatePosition()
        {
            // reposition the window
            var szStart = GetStartButtonRect();
            System.Drawing.Point offset;
            var edge = GetTaskbarEdge();
            if (edge.IsVertical())
            {
                offset = new Point(0, szStart.Height);
            }
            else
            {
                offset = new Point(szStart.Width, 0);
            }

            var offset2 = (IsWin8OrUp ? offset : ScreenFromWpf(offset, _taskbarHwnd));
            var flags = (uint)(0
                | SetWindowPosConstants.SWP_SHOWWINDOW
                | SetWindowPosConstants.SWP_NOOWNERZORDER
                | SetWindowPosConstants.SWP_NOACTIVATE
                );
            User32Dll.SetWindowPos(_hSrc.Handle, (IntPtr)WindowZOrderConstants.HWND_TOP,
                                   offset2.X, offset2.Y,
                                   _buttonsWindowSize.Width, _buttonsWindowSize.Height,
                                   flags);
        }

        private bool CheckIconSize()
        {
            var newTaskbarIconSize = GetTaskbarIconSize();
            if (newTaskbarIconSize != _taskbarIconsSize)
            {
                _notifyee.TaskbarIconsSizeChanged(newTaskbarIconSize);
                _taskbarIconsSize = newTaskbarIconSize;
                UpdatePosition();
                return true;
            }

            return false;
        }

        public void Remove()
        {
            if (_smallIconsRegistryMonitor != null)
            {
                _smallIconsRegistryMonitor.Stop();
                _smallIconsRegistryMonitor = null;
            }

            var newRebarCoords = CalculateRebarCoords(false);

            NativeDll.DetachHooks();					// detach - can cause reposition by Rebar itself

            // Unhook resize event catcher
            if (_hWinEventHook != IntPtr.Zero)
            {
                User32Dll.UnhookWinEvent(_hWinEventHook);
            }

            if (!User32Dll.SetWindowPos(NativeDll.FindRebar(), (IntPtr)0,
                                        newRebarCoords.Left, newRebarCoords.Top, newRebarCoords.Width, newRebarCoords.Height,
                                        0)) // move to correct coords
            {
                throw new InteropException("Cannot move Rebar back");
            }

            // TODO: -1    -= event delegates
            _hSrc.Dispose();
        }

        // return global coords of left top conner of our window = placeholder of buttons
        private System.Drawing.Point CalculateButtonPosition()
        {
            var p = GetTaskbarRect();
            var s = GetStartButtonRect();
            var orientation = GetTaskbarEdge();
            System.Drawing.Point pos;

            if (orientation.IsVertical())
            {
                pos = new System.Drawing.Point(p.Left, p.Top + s.Height);
            }
            else
            {
                pos = new System.Drawing.Point(p.Left + s.Width, p.Top);
            }

            return pos;
        }

        private RectWin GetStartButtonRect()
        {
            var startButtonRect = new RectWin();
            if (IsWin8OrUp)
            {
                return startButtonRect;
            }
            User32Dll.GetWindowRect(_startButtonHwnd, startButtonRect);
            return startButtonRect;
        }

        /// <summary>
        /// Calculate rebar coordinates
        /// </summary>
        /// <param name="isInsert">if false: then calculate coords while removing our buttons and moving Rebar back</param>
        /// <returns>return top-left corner and new size</returns>
        private RectWin CalculateRebarCoords(bool isInsert = true)
        {
            // TODO: -1
            var d = isInsert ? 1 : -1;
            int diff;
            _taskbarPosition = GetTaskbarEdge();
            if (_taskbarPosition.IsVertical())
            {
                diff = d * _buttonsWindowSize.Height;
            }
            else
            {
                diff = d * _buttonsWindowSize.Width;
            }

            return CalculateShiftedRebarCoords(diff);
        }

        private RectWin CalculateShiftedRebarCoords(int diff)
        {
            var newRebarCoords = new RectWin();
            if (!User32Dll.GetWindowRect(_rebarHwnd, newRebarCoords))
            {
                throw new InteropException("Cannot calculate new Rebar position");
            }

            var taskbarPos = GetTaskbarRect();

            // Recalculate position relative to taskbar
            newRebarCoords.Left -= taskbarPos.Left;
            newRebarCoords.Right -= taskbarPos.Left;
            newRebarCoords.Top -= taskbarPos.Top;
            newRebarCoords.Bottom -= taskbarPos.Top;

            if (_taskbarPosition.IsVertical())
            {
                newRebarCoords.Top += diff;

                // Do not do this since we are already trimming in C++ part
                //newRebarCoords.Height -= diff;
            }
            else
            {
                newRebarCoords.Left += diff;

                // Do not do this since we are already trimming in C++ part
                //newRebarCoords.Width -= diff;
            }

            return newRebarCoords;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == _closeMessageId)
            {
                _notifyee.Shutdown();
            }
            else
            {
                if (msg == _updateMessageId)
                {
                    //var rebarRect = _coordsPackager.UnpackParams(lParam, wParam);

                    ReactToSizeMove();
                }
            }

            return NULL;
        }

        // return one of 4 possible edge
        private TaskbarPosition GetTaskbarEdge(HWND taskBar, ref MonitorInfo monitorInfo, ref HMONITOR hMonitor, ref RectWin taskbarRect)
        {
            if (!User32Dll.IsWindow(taskBar))
            {
                throw new InteropException("TaskBar must not be a window");
            }

            APPBARDATA appbar = new APPBARDATA() { hWnd = taskBar };
            appbar.cbSize = (uint)Marshal.SizeOf(appbar);
            Shell32Dll.SHAppBarMessage((uint)AppBarMessages.ABM_GETTASKBARPOS, ref appbar);
            if (taskbarRect != null)
            {
                taskbarRect = new RectWin(appbar.rc);
            }
            if (hMonitor != NULL)
            {
                monitorInfo.Size = Marshal.SizeOf(typeof(MonitorInfo));
                HMONITOR monitor = User32Dll.MonitorFromRect(ref appbar.rc, MonitorConstants.MONITOR_DEFAULTTONEAREST);
                bool result = User32Dll.GetMonitorInfo(monitor, ref monitorInfo);
                if (!result)
                {
                    throw new InteropException("Can't get monitor info");
                }
                if (hMonitor != null)
                {
                    hMonitor = monitor;
                }
            }
            return (TaskbarPosition)appbar.uEdge;
        }

        private TaskbarPosition GetTaskbarEdge()
        {
            MonitorInfo mi = new MonitorInfo();
            RectWin r = new RectWin();
            HMONITOR monitor = NULL;
            return GetTaskbarEdge(_taskbarHwnd, ref mi, ref monitor, ref r);
        }

        private RectWin GetTaskbarRect()
        {
            MonitorInfo mi = new MonitorInfo();
            RectWin taskbarRect = new RectWin();
            HMONITOR monitor = NULL;
            GetTaskbarEdge(_taskbarHwnd, ref mi, ref monitor, ref taskbarRect);
            return taskbarRect;
        }

        private DWORD GetTaskbarThread()
        {
            return User32Dll.GetWindowThreadProcessId(NativeDll.FindTaskBar(), IntPtr.Zero);
        }

        private HWND _foundStartButtonHwnd = NULL;

        private bool StartButtonEnumFunc(HWND hwnd, LPARAM lParam)
        {
            var buffString = new StringBuilder(BuffSize);
            User32Dll.GetClassName(hwnd, buffString, BuffSize);
            var currentClassName = buffString.ToString();
            if (String.IsNullOrEmpty(currentClassName) ||
                !String.Equals(StartButtonClass, currentClassName, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            _foundStartButtonHwnd = hwnd;
            return false;
        }

        private HWND FindStartButton()
        {
            var threadExplorer = GetTaskbarThread();
            _foundStartButtonHwnd = NULL;
            if (!IsVistaOrUp)
            {
                _foundStartButtonHwnd = User32Dll.FindWindowEx(NativeDll.FindTaskBar(), NULL, StartButtonClass, null);
            }
            else
            {
                User32Dll.EnumThreadWindows(threadExplorer, StartButtonEnumFunc, NULL); // find Start button
            }
            if (IsWin8OrUp || (_foundStartButtonHwnd != NULL))
            {
                return _foundStartButtonHwnd;
            }
            throw new InteropException("Didn't find StartButton!");
        }

        private System.Drawing.Size GetButtonsWindowSize(bool firstTime = false)
        {
            // TODO: -2 save/load
            // TODO: -2 accomodate to the Taskbar actual size
            MonitorInfo mi = new MonitorInfo();
            RectWin taskbarRect = new RectWin();
            HMONITOR monitor = NULL;

            if (firstTime)
            {
                _buttonsWindowSize = new System.Drawing.Size(DefaultStartButtonWidth, DefaultStartButtonHeight);
            }

            TaskbarPosition edge = GetTaskbarEdge(_taskbarHwnd, ref mi, ref monitor, ref taskbarRect);
            if (!IsWin8OrUp)
            {
                RectWin startButtonRect = GetStartButtonRect();

                if (edge.IsVertical())
                {
                    if (firstTime)
                    {
                        _buttonsWindowSize.Height = startButtonRect.Height;
                    }
                    _buttonsWindowSize.Width = taskbarRect.Width;

                    // vertical taskbar
                    //s2.Height = DefaultStartButtonHeight;

                    // TODO: -2 implement code block bellow if we need some locig for small icons
                    //if (IsTaskbarSmallIcons())
                    //{
                    //	s2.Height =
                    //}
                }
                else
                {
                    // limit height: horizontal taskbar with small icons has Start button window out of the screen
                    _buttonsWindowSize.Height = taskbarRect.Height;
                    if (firstTime)
                    {
                        _buttonsWindowSize.Width = startButtonRect.Width;
                    }
                }
            }
            else	// win8, 9, ...
            {
                if (edge.IsVertical())
                {
                    _buttonsWindowSize.Width = taskbarRect.Width;
                }
                else
                {
                    _buttonsWindowSize.Height = taskbarRect.Height;
                }
            }

            return _buttonsWindowSize;
        }

        public bool ChangeWidth(int newWidth)
        {
            UpdateHandles();
            return DoChangeWidth(newWidth, false);
        }

        private bool DoChangeWidth(int newWidth, bool firstTime)
        {
             newWidth = (int)Math.Round(newWidth * _dpiScalingFactor);

            int delta;
            if (_taskbarPosition.IsVertical())
            {
                delta = newWidth - _buttonsWindowSize.Height;
                _buttonsWindowSize.Height = newWidth;
            }
            else
            {
                delta = newWidth - _buttonsWindowSize.Width;
                _buttonsWindowSize.Width = newWidth;
            }
            _buttonsWidth = newWidth;

            RectWin newRebarCoords;
            if (firstTime)
            {
                newRebarCoords = CalculateRebarCoords();
            }
            else
            {
                newRebarCoords = CalculateShiftedRebarCoords(delta);
            }

            // reposition the window
            var szStart = GetStartButtonRect();
            System.Drawing.Point offset;
            if (_taskbarPosition.IsVertical())
            {
                offset = new Point(0, szStart.Height);
            }
            else
            {
                offset = new Point(szStart.Width, 0);
            }

            var offset2 = ScreenFromWpf(offset, NativeDll.FindTaskBar());

            PostPositionToHook();

            if (!SetRebarPos(newRebarCoords))
            {
                return false;
            }

            User32Dll.SetWindowPos(_hSrc.Handle, (IntPtr)WindowZOrderConstants.HWND_TOP,
                                   offset2.X, offset2.Y,
                                   _buttonsWindowSize.Width, _buttonsWindowSize.Height,
                                   (uint)
                                   (SetWindowPosConstants.SWP_SHOWWINDOW | SetWindowPosConstants.SWP_NOOWNERZORDER |
                                    SetWindowPosConstants.SWP_NOACTIVATE));

            return true;
        }

        private bool SetRebarPos(RectWin newRebarCoords)
        {
            return User32Dll.SetWindowPos(_rebarHwnd, IntPtr.Zero, newRebarCoords.Left, newRebarCoords.Top,
                newRebarCoords.Width, newRebarCoords.Height, (uint)(SetWindowPosConstants.SWP_NOSENDCHANGING));
        }

        private Point ScreenFromWpf(Point local, IntPtr hwnd)
        {
            var p = new POINT() { x = local.X, y = local.Y };
            User32Dll.MapWindowPoints(hwnd, IntPtr.Zero, ref p, 1);
            if (Kernel32Dll.GetLastError() != 0)
            {
                throw new InteropException("Error converting points");
            }

            return new Point(p.x, p.y);
        }
    }
}