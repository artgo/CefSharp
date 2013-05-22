using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;
using System.Windows.Interop;
using Control = System.Windows.Controls.Control;
using DWORD = System.UInt32;
using HWND = System.IntPtr;

using LPARAM = System.IntPtr;

using Point = System.Drawing.Point;

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
        private const uint NotHideWindow = ~(uint)SetWindowPosConstants.SWP_HIDEWINDOW;
        private const int FailedValue = -1;

        // win 7  default with large buttons
        private const int DefaultStartButtonWidth = 54;

        private int _taskbarMargins = 0;

        private const int DefaultStartButtonHeight = 40;
        // NB! MAKE SURE THAT THESE VALUES STAY IN SYNC WITH THE APPLICATION AND THE INSTALLER!!!
        private const string UpdatePositionMessageName = @"AppDirectButtonPositionUpdateMessage";
        private const string CloseMessageName = @"AppDirectForceApplicationCloseMessage";
        private const string WindowName = @"AppDirectTaskbarButtonsWindow";
        private readonly static bool IsVistaOrUp;
        private readonly static bool IsWin7OrUp;
        private readonly static bool IsWin8OrUp;

        private readonly CoordsPackager _coordsPackager = new CoordsPackager();

        private volatile HwndSource _hwndSource = null;
        private volatile ITaskbarInterop _notifyee = null;
        private System.Drawing.Size _buttonsWindowSize;
        private volatile HwndSourceHook _wndProcHook = null;
        private volatile int _taskbarHeight = 0;
        private volatile int _buttonsWidth = 0;
        private volatile TaskbarPosition _taskbarPosition = TaskbarPosition.Bottom;
        private volatile TaskbarIconsSize _taskbarIconsSize = TaskbarIconsSize.Large;
        private Screen _taskbarScreen;
        private volatile HWND _taskbarHwnd = NULL;
        private volatile HWND _rebarHwnd = NULL;
        private volatile HWND _startButtonHwnd = NULL;
        private volatile uint _updateMessageId = 0;
        private volatile uint _closeMessageId = 0;
        private volatile bool _isShutdown = false;
        private volatile bool _shutdownStarted = false;
        private volatile SubclassProc _subclassProc = null;
        private double _dpiScalingFactor;

        public int TaskbarHeight { get { return _taskbarHeight; } }

        public TaskbarPosition TaskbarPosition { get { return _taskbarPosition; } }

        public TaskbarIconsSize TaskbarIconsSize { get { return _taskbarIconsSize; } }

        public Screen TaskbarScreen { get { return _taskbarScreen; } }

        public Double DpiScalingFactor { get { return _dpiScalingFactor; } }

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

            _taskbarMargins = GetTaskbarHeight() - GetRebarHeight();
            _buttonsWindowSize = GetButtonsWindowSize(true);
            _taskbarHeight = GetTaskbarHeight();
            _taskbarScreen = Screen.FromHandle(_taskbarHwnd);
        }

        public void Place(Control wnd, ITaskbarInterop notifyee, int initialWidth)
        {
            if (_hwndSource != null)
            {
                _hwndSource.Dispose();
                _hwndSource = null;
            }

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
            _updateMessageId = User32Dll.RegisterWindowMessage(UpdatePositionMessageName);

            var pos = CalculateButtonPosition();

            var hwndSourceParams = new HwndSourceParameters(
                WindowName, // NAME
                _buttonsWindowSize.Width,
                _buttonsWindowSize.Height // size of WPF window inside usual Win32 window
                );
            hwndSourceParams.PositionX = pos.X;
            hwndSourceParams.PositionY = pos.Y;
            hwndSourceParams.ParentWindow = _startButtonHwnd;

            // 94000C00 is from the Start button
            hwndSourceParams.WindowStyle = (int)(0

                // popup is prohibiting child style on Win8
                //| (uint)WindowsStyleConstants.WS_POPUP							// 80000000
                                                  | (uint)WindowsStyleConstants.WS_VISIBLE // 10000000
                                                  | (uint)WindowsStyleConstants.WS_CLIPSIBLINGS // 04000000
                                                  | (uint)WindowsStyleConstants.RBS_BANDBORDERS // 00000400
                                                  | (uint)WindowsStyleConstants.RBS_FIXEDORDER // 00000800
                                                 );

            // 00080088 is from the Start button
            hwndSourceParams.ExtendedWindowStyle = 0
                                                   | (int)ExtendedWindowsStyleConstants.WS_EX_TOOLWINDOW //0x00000080
                                                   | (int)ExtendedWindowsStyleConstants.WS_EX_LAYERED //0x00080000
                                                   | (int)ExtendedWindowsStyleConstants.WS_EX_TOPMOST //0x00000008
                ;

            hwndSourceParams.UsesPerPixelOpacity = true; // set the WS_EX_LAYERED extended window style

            _hwndSource = new HwndSource(hwndSourceParams) { RootVisual = wnd };

            _wndProcHook = WndProc;
            _hwndSource.AddHook(_wndProcHook);
            _subclassProc = PfnSubclass;

            if (!Comctl32Dll.SetWindowSubclass(_hwndSource.Handle, _subclassProc, NULL, NULL))
            {
                throw new InteropException("Failed to install sublass proc on HwndSource");
            }

            DoChangeWidth(_buttonsWidth, true);

            lock (_lockObject)
            {
                _shutdownStarted = false;
                _isShutdown = false;
            }

            if (IsVistaOrUp)
            {
                var availableMessages = new[] { _closeMessageId, _updateMessageId };

                foreach (var availableMessage in availableMessages)
                {
                    // Allow this window to receive the message
                    User32Dll.ChangeWindowMessageFilter(availableMessage, ChangeWindowMessageFilterFlags.Add);
                }
            }

            NativeDll.SetupSubclass(_hwndSource.Handle);

            _taskbarPosition = GetTaskbarEdge();

            PostPositionToHook();

            UpdatePosition();

            if (IsWin7OrUp)
            {
                if (DwmapiDll.DwmIsCompositionEnabled())
                {
                    var status = Marshal.AllocHGlobal(sizeof(int));
                    Marshal.WriteInt32(status, 1); // true

                    DwmapiDll.DwmSetWindowAttribute(_hwndSource.Handle, DwmWindowAttribute.DWMWA_EXCLUDED_FROM_PEEK,
                                                    status, sizeof(int));
                }
            }
        }

        private IntPtr PfnSubclass(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, IntPtr dwRefData)
        {
            if (uMsg == (uint)WindowsMessages.WM_SHOWWINDOW)
            {
                // wParam == FALSE (hide window)
                if (wParam == NULL)
                {
                    // return 0 to indicate that message was processed
                    return NULL;
                }
            }
            else if (uMsg == (uint)WindowsMessages.WM_WINDOWPOSCHANGING)
            {
                var winPos = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                winPos.flags = winPos.flags & NotHideWindow;
                Marshal.StructureToPtr(winPos, lParam, true);
            }

            return Comctl32Dll.DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }

        private HWND FindTaskBar()
        {
            return User32Dll.FindWindow("Shell_TrayWnd", null);
        }

        private HWND FindReBar(IntPtr hwndTaskBar)
        {
            return User32Dll.FindWindowEx(hwndTaskBar, IntPtr.Zero, "ReBarWindow32", null);
        }

        private void UpdateHandles()
        {
            lock (_lockObject)
            {
                _taskbarHwnd = FindTaskBar();
                _rebarHwnd = FindReBar(_taskbarHwnd);

                // Start button musto go after taskbar
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

        private TaskbarIconsSize GetTaskbarIconSize()
        {
            // Default icon size for XP and Vista is small
            var iconsSize = TaskbarIconsSize.Small;

            if (IsWin7OrUp)
            {
                // Defaulting to large icons on Windows 7 and 8
                iconsSize = TaskbarIconsSize.Large;

                try
                {
                    var val = Registry.GetValue(SmallIconsPath, SmallIconsFiledName, FailedValue);

                    if ((val is int) && ((int)val) == IconsSize.SMALL)
                    {
                        iconsSize = TaskbarIconsSize.Small;
                    }
                }
                catch (SecurityException)
                {
                    // Refactor: log
                }
                catch (IOException)
                {
                    // Refactor: log
                }
            }

            return iconsSize;
        }

        private double GetDpiScaleFactor()
        {
            var dpiSetting = (double)(int)(Registry.GetValue(DpiSettingPath, DpiSettingName, StandardDpi) ?? StandardDpi);
            return dpiSetting / StandardDpi;
        }

        private void ReactToSizeMove(bool forceUpdatePos = false)
        {
            UpdateHandles();

            var updatePos = forceUpdatePos;
            var oldHeight = _buttonsWindowSize.Height;
            var oldWidth = _buttonsWindowSize.Width;
            _buttonsWindowSize = GetButtonsWindowSize();

            _taskbarScreen = Screen.FromHandle(_taskbarHwnd);

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

        private int GetRebarHeight()
        {
            var rebarRect = GetRebarRect();
            int newHeight;
            if (_taskbarPosition.IsVertical())
            {
                newHeight = rebarRect.Width;
            }
            else
            {
                newHeight = rebarRect.Height;
            }
            return newHeight;
        }

        private void UpdatePosition()
        {
            var offset = GetTopLeftCoords();

            var flags = (uint)(0
                | SetWindowPosConstants.SWP_SHOWWINDOW
                | SetWindowPosConstants.SWP_NOOWNERZORDER
                | SetWindowPosConstants.SWP_NOACTIVATE
                );
            User32Dll.SetWindowPos(_hwndSource.Handle, (IntPtr)WindowZOrderConstants.HWND_TOP,
                                   offset.X, offset.Y,
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

        public bool Remove(TaskbarApi.ShutdownCallback shutdownCallback)
        {
            if (_shutdownStarted || _isShutdown)
            {
                return false;
            }

            // Flag to dispose HwndSource after everything is complete.
            // We can't dispose HwndSource in this method because then we won't get
            //   message from hook that unhooking is complete. So we do it on that unhooking complete message.
            _shutdownStarted = true;

            // is called with assumption that it is called from the GUI message pump thread; otherwise race conditions
            //NativeDll.TearDownSubclass(); // detach - can cause reposition by Rebar itself

            var newRebarCoords = CalculateRebarCoords(false);


            if (!User32Dll.SetWindowPos(FindReBar(FindTaskBar()), IntPtr.Zero,
                                        newRebarCoords.Left, newRebarCoords.Top, newRebarCoords.Width,
                                        newRebarCoords.Height,
                                        0)) // move to correct coords
            {
                throw new InteropException("Cannot move Rebar back");
            }

            if (_subclassProc != null)
            {
                Comctl32Dll.RemoveWindowSubclass(_hwndSource.Handle, _subclassProc, NULL);
                _subclassProc = null;
            }

            _isShutdown = true;
            _hwndSource.Dispose();
            _hwndSource = null;
            _wndProcHook = null;

            if (shutdownCallback != null)
            {
                shutdownCallback();
            }

            return true;
        }

        // return global coords of left top corner of our window = placeholder of buttons

        private System.Drawing.Point CalculateButtonPosition()
        {
            var p = GetTaskbarRect();
            var s = GetStartButtonRect();
            var orientation = GetTaskbarEdge();
            System.Drawing.Point pos;

            if (orientation.IsVertical())
            {
                pos = new System.Drawing.Point(p.Left, s.Top + s.Height);//place from the bottom of start button
            }
            else
            {
                pos = new System.Drawing.Point(s.Left + s.Width, p.Top);//place from the right of start button
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
            var newRebarCoords = GetRebarRect();

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
                var heightDiff = (taskbarPos.Height - newRebarCoords.Height) / 2;

                newRebarCoords.Top -= heightDiff;
                newRebarCoords.Bottom -= heightDiff;
                newRebarCoords.Left += diff;

                // Do not do this since we are already trimming in C++ part
                //newRebarCoords.Width -= diff;
            }

            return newRebarCoords;
        }

        private RectWin GetRebarRect()
        {
            var newRebarCoords = new RectWin();
            if (!User32Dll.GetWindowRect(_rebarHwnd, newRebarCoords))
            {
                throw new InteropException("Cannot calculate new Rebar position");
            }
            return newRebarCoords;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (_isShutdown)
            {
                return NULL;
            }

            if (msg == _updateMessageId)
            {
                if (!_shutdownStarted && !_isShutdown)
                {
                    ReactToSizeMove(wParam.ToInt32() != 0);
                    handled = true;
                }
            }
            else if (msg == _closeMessageId)
            {
                if (!_shutdownStarted && !_isShutdown)
                {
                    ShutdownHelper.Instance.Shutdown();
                    handled = true;
                }
            }
            else if (msg == (int)WindowsMessages.WM_DISPLAYCHANGE)
            {
                if (!_shutdownStarted && !_isShutdown)
                {
                    ReactToSizeMove(true);
                }
            }
            else if (msg == (int)WindowsMessages.WM_SHOWWINDOW)
            {
                if (!_shutdownStarted && !_isShutdown && (wParam == NULL))
                {
                    handled = true;
                }
            }

            return NULL;
        }

        // return one of 4 possible edge
        private TaskbarPosition GetTaskbarEdge(HWND taskBar, ref RectWin taskbarRect)
        {
            var appbar = new APPBARDATA() { hWnd = taskBar };
            appbar.cbSize = (uint)Marshal.SizeOf(appbar);
            Shell32Dll.SHAppBarMessage((uint)AppBarMessages.ABM_GETTASKBARPOS, ref appbar);
            if (taskbarRect != null)
            {
                if (!User32Dll.GetWindowRect(this._taskbarHwnd, taskbarRect))
                {
                    throw new InteropException("Cannot calculate taskbar rect");
                }
            }

            return (TaskbarPosition)appbar.uEdge;
        }

        private TaskbarPosition GetTaskbarEdge()
        {
            RectWin taskbarRect = new RectWin();
            return GetTaskbarEdge(_taskbarHwnd, ref taskbarRect);
        }

        private RectWin GetTaskbarRect()
        {
            RectWin taskbarRect = new RectWin();
            GetTaskbarEdge(_taskbarHwnd, ref taskbarRect);
            return taskbarRect;
        }

        private DWORD GetTaskbarThread()
        {
            return User32Dll.GetWindowThreadProcessId(_taskbarHwnd, IntPtr.Zero);
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
                _foundStartButtonHwnd = User32Dll.FindWindowEx(_taskbarHwnd, NULL, StartButtonClass, null);
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
            if (firstTime)
            {
                _buttonsWindowSize = new System.Drawing.Size(DefaultStartButtonWidth, DefaultStartButtonHeight);
            }

            var taskbarRect = new RectWin();
            var edge = GetTaskbarEdge(_taskbarHwnd, ref taskbarRect);

            if (!IsWin8OrUp)
            {
                var startButtonRect = GetStartButtonRect();

                if (edge.IsVertical())
                {
                    if (firstTime)
                    {
                        _buttonsWindowSize.Height = startButtonRect.Height;
                    }
                    _buttonsWindowSize.Width = taskbarRect.Width - _taskbarMargins;
                }
                else
                {
                    // limit height: horizontal taskbar with small icons has Start button window out of the screen
                    _buttonsWindowSize.Height = taskbarRect.Height - _taskbarMargins;
                    if (firstTime)
                    {
                        _buttonsWindowSize.Width = startButtonRect.Width;
                    }
                }
            }
            else // Windows 8
            {
                if (edge.IsVertical())
                {
                    _buttonsWindowSize.Width = taskbarRect.Width;
                }
                else
                {
                    _buttonsWindowSize.Height = taskbarRect.Height - _taskbarMargins;
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

            var offset = GetTopLeftCoords();
            PostPositionToHook();

            if (!SetRebarPos(newRebarCoords))
            {
                return false;
            }

            User32Dll.SetWindowPos(_hwndSource.Handle, (IntPtr)WindowZOrderConstants.HWND_TOP,
                                   offset.X, offset.Y,
                                   _buttonsWindowSize.Width, _buttonsWindowSize.Height,
                                   (uint)
                                   (SetWindowPosConstants.SWP_SHOWWINDOW | SetWindowPosConstants.SWP_NOOWNERZORDER |
                                    SetWindowPosConstants.SWP_NOACTIVATE));

            return true;
        }

        private Point GetTopLeftCoords()
        {
            var szStart = GetStartButtonRect();
            System.Drawing.Point offset;
            var edge = GetTaskbarEdge();
            var taskbar = GetTaskbarRect();
            if (edge.IsVertical())
            {
                offset = new Point(taskbar.Left + (taskbar.Width / 2) - (_buttonsWindowSize.Width / 2), szStart.Top + szStart.Height);
            }
            else
            {
                offset = new Point(szStart.Left + szStart.Width, taskbar.Top + (taskbar.Height / 2) - (_buttonsWindowSize.Height / 2));
            }

            return offset;
        }

        private bool SetRebarPos(RectWin newRebarCoords)
        {
            return User32Dll.SetWindowPos(_rebarHwnd, IntPtr.Zero, newRebarCoords.Left, newRebarCoords.Top,
                newRebarCoords.Width, newRebarCoords.Height, (uint)(SetWindowPosConstants.SWP_NOSENDCHANGING));
        }

        private Point ScreenFromWpf(Point local, IntPtr hwnd)
        {
            var p = new Point() { X = local.X, Y = local.Y };
            var ok = User32Dll.ClientToScreen(hwnd, ref p);
            if (!ok)
            {
                var err = Kernel32Dll.GetLastError();
                if (err != 0)
                {
                    throw new InteropException("Error converting points + " + err);
                }
            }

            return p;
        }
    }
}