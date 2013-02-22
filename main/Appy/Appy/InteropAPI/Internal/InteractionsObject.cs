using Accessibility;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Interop;
using DWORD = System.UInt32;
using HANDLE = System.IntPtr;
using HHOOK = System.IntPtr;
using HINSTANCE = System.IntPtr;
using HWND = System.IntPtr;
using UINT = System.UInt32;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class InteractionsObject : ITaskbarInteropCallback
    {
        private static readonly IntPtr NULL = IntPtr.Zero;
        private const int MAX_LOADSTRING = 100;
        private const int WM_OPEN = (int)(WindowsMessages.WM_USER + 10);
        private const int TIMER_HOOK = 1;
        private const int UOI_NAME = 2;
        private HINSTANCE hInst;											// current instance
        private char[] szTitle = new char[MAX_LOADSTRING];					// The title bar text
        private char[] szWindowClass = new char[MAX_LOADSTRING];			// the main window class name
        private UINT g_TaskbarCreatedMsg; // the "TaskbarCreated" message
        private HHOOK g_StartHook = NULL;
        private HWND g_hWPFWnd = NULL;
        private bool DoExit = false;
        private const int HC_ACTION = 0;
        private const string SmallIconsPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
        private const string SmallIconsFiledName = "TaskbarSmallIcons";
        private HookProc hookProc;
        private IntPtr ExplorerHook;
        private static bool bInitDone = false;
        private static IntPtr hDll;
        private HwndSource _hSrc;
        private volatile ITaskbarInterop _notifyee = null;
        private System.Drawing.Size _buttonsWindowSize;
        private volatile IntPtr _hWinEventHook = IntPtr.Zero;
        private volatile RectWin _taskbarPos = null;
        private volatile int _taskbarHeight = 0;
        private volatile TaskbarPosition _taskbarPosition = TaskbarPosition.Bottom;
        private volatile TaskbarIconsSize _taskbarIconsSize = TaskbarIconsSize.Large;
        private volatile RegistryChangeMonitor _smallIconsRegirstyMonitor;

        public int TaskbarHeight { get { return _taskbarHeight; } }
        public TaskbarPosition TaskbarPosition { get { return _taskbarPosition; } }
        public TaskbarIconsSize TaskbarIconsSize { get { return _taskbarIconsSize; } }

        public void LoadInitialValues()
        {
            _taskbarHeight = 40;
            _taskbarPosition = GetTaskbarPosition();
            _taskbarIconsSize = GetTaskbarIconSize();
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

            _buttonsWindowSize = GetInitialWndSize();
            _buttonsWindowSize.Width = initialWidth;
            var p = new HwndSourceParameters(
                    "adButton.WPF",			// NAME
                    _buttonsWindowSize.Width, _buttonsWindowSize.Height		// size of WPF window inside usual Win32 window
                );
            p.PositionX = 0; p.PositionY = 0;
            p.ParentWindow = NativeDll.FindTaskBar();
			p.UsesPerPixelOpacity = true;		// set the WS_EX_LAYERED extended window style
			unchecked
			{
				// 94000C00
				p.WindowStyle = (int)0x94000C00;
				//p.WindowStyle =
				//	(int)(0
				//	| WindowsStyleConstants.WS_VISIBLE		// 10000000
				//	| WindowsStyleConstants.WS_POPUP		// 80000000
				//	//| WindowsStyleConstants.WS_CHILD
				//	)
				
				//;
			
				// 00080088
				p.ExtendedWindowStyle = 0x00080088;
				//p.ExtendedWindowStyle = 0
				//	| 0x00080000	// WS_EX_LAYERED
				//	| 0x00000008	//WS_EX_TOPMOST
				//;

			}
			
            _hSrc = new HwndSource(p);
            _hSrc.RootVisual = wnd;

			// TODO: -1 remove if unneeded
            //_hSrc.AddHook(WndProc);		// handle custom WM_

            DoChangeWidth(initialWidth, true);

            NativeDll.InjectExplrorerExe();
   
            var taskberHwnd = NativeDll.FindTaskBar();
            uint taskbarProcessId;
            var taskbarThreadId = User32Dll.GetWindowThreadProcessId(taskberHwnd, out taskbarProcessId);

            // Hook resize event catcher
            // EVENT_OBJECT_DRAGCOMPLETE
            _hWinEventHook = User32Dll.SetWinEventHook((uint)EventConstants.EVENT_SYSTEM_MOVESIZEEND, 
                (uint)EventConstants.EVENT_SYSTEM_MOVESIZEEND, NULL, WinEventDelegate,
                taskbarProcessId, taskbarThreadId, (uint)(WinEventHookFlags.WINEVENT_OUTOFCONTEXT | WinEventHookFlags.WINEVENT_SKIPOWNPROCESS));

            _smallIconsRegirstyMonitor = new RegistryChangeMonitor(SmallIconsPath);
            _smallIconsRegirstyMonitor.Changed += RegistryChangeHandler;
            _smallIconsRegirstyMonitor.Start();
        }

        private void RegistryChangeHandler(object sender, RegistryChangeEventArgs e)
        {
            CheckIconSize();
        }

        private TaskbarPosition GetTaskbarPosition()
        {
            return NativeDll.GetTaskbarEdge();
        }

        private TaskbarIconsSize GetTaskbarIconSize()
        {
			var sz = TaskbarIconsSize.Small;	// old versions use small icons
			var regSmall = Registry.GetValue(SmallIconsPath, SmallIconsFiledName, -1);
			
            if (regSmall != null)				// since Win7
			{
				if ((int) regSmall == IconsSize.LARGE)
				{
				    sz = TaskbarIconsSize.Large;
				}
			}

			return sz;
        }

        private void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
                                      IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            switch ((EventConstants)eventType)
            {
                case EventConstants.EVENT_SYSTEM_MOVESIZEEND:
                    IAccessible accWindow;
                    object varChild;

                    var hr = OleaccDll.AccessibleObjectFromEvent(hwnd, (uint) idObject, (uint) idChild, out accWindow, out varChild);
                    if (hr != HresultCodes.S_OK || accWindow == null)
                    {
                        return;
                    }

                    var rect = new RectWin();
                    accWindow.accLocation(out rect.Left, out rect.Top, out rect.Right, out rect.Bottom, varChild);

                    var newTaskbarPosition = GetTaskbarPosition();
                    if (newTaskbarPosition != _taskbarPosition)
                    {
                        _notifyee.PositionChanged(newTaskbarPosition);
                        _taskbarPosition = newTaskbarPosition;
                    }

                    CheckIconSize();

                    break;
            }
        }

        private void CheckIconSize()
        {
            var newTaskbarIconSize = GetTaskbarIconSize();
            if (newTaskbarIconSize != _taskbarIconsSize)
            {
                _notifyee.TaskbarIconsSizeChanged(newTaskbarIconSize);
                _taskbarIconsSize = newTaskbarIconSize;
            }
        }

        private System.Drawing.Size GetCurrentButtonSize()
        {
            // TODO: -0
            return GetInitialWndSize();
        }

        public void Remove()
        {
            if (_smallIconsRegirstyMonitor != null)
            {
                _smallIconsRegirstyMonitor.Stop();
                _smallIconsRegirstyMonitor = null;
            }

            System.Drawing.Size newSize;	// calculate correct coords first
            System.Drawing.Point newTopLeft = RebarCoords(out newSize, false);

            NativeDll.DetachHooks();					// detach - can cause reposition by Rebar itself

            // Unhook resize event catcher
            if (_hWinEventHook != IntPtr.Zero)
            {
                User32Dll.UnhookWinEvent(_hWinEventHook);
            }

            if (!User32Dll.SetWindowPos(NativeDll.FindRebar(), (IntPtr) 0,
                                        newTopLeft.X, newTopLeft.Y, newSize.Width, newSize.Height,
                                        0)) // move to correct coords
            {
                throw new Exception("Cannot move Rebar back");
            }

            _hSrc.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newSize"></param>
        /// <param name="isInsert">if false: then calculate coords while removing our buttons and moving Rebar back</param>
        /// <returns>return top-left corner and new size</returns>
        private System.Drawing.Point RebarCoords(out System.Drawing.Size newSize, bool isInsert = true)
        {
            var szButton = _buttonsWindowSize;		// TODO: -1
            int d = isInsert ? 1 : -1;

            var rebarOld = new RectWin();
            if (!User32Dll.GetWindowRect(NativeDll.FindRebar(), rebarOld))
            {
                throw new Exception("Cannot calculate new Rebar position");
            }

            System.Drawing.Point ltTaskBar = NativeDll.GetTaskbarPos();
			System.Drawing.Point rebarNewTopLeft; 
            System.Drawing.Size rebarNewSize;
			var edge = NativeDll.GetTaskbarEdge();
			if (edge.IsVertical())		// if vertical
            {
				rebarNewSize = new System.Drawing.Size(rebarOld.Width, rebarOld.Height - d * szButton.Height);
				rebarNewTopLeft = new System.Drawing.Point(
					rebarOld.Left - ltTaskBar.X,		// to relative
					rebarOld.Top + d * szButton.Height);
			}
			else	// horizontal
			{
				rebarNewSize = new System.Drawing.Size(rebarOld.Width - d * szButton.Width, rebarOld.Height);
				rebarNewTopLeft = new System.Drawing.Point(rebarOld.Left + d * szButton.Width, 
					rebarOld.Top - ltTaskBar.Y);            // to relative
            }
			newSize = rebarNewSize;
            return rebarNewTopLeft;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle messages...

            return NULL;
        }

        #region helpers
        private System.Drawing.Size GetInitialWndSize()
        {
            return NativeDll.GetInitialADButtonSize();
        }
        #endregion helpers

        public bool ChangeWidth(int newWidth)
        {
            return DoChangeWidth(newWidth, false);
        }

        private bool DoChangeWidth(int newWidth, bool firstTime)
        {
            int delta = newWidth - _buttonsWindowSize.Width;
            _buttonsWindowSize.Width = newWidth;
            System.Drawing.Size newSize;
            System.Drawing.Point newTopLeft;
            if (firstTime)
            {
                newTopLeft = RebarCoords(out newSize);
            }
            else
            {
                var rebarOld = new RectWin();
                if (!User32Dll.GetWindowRect(NativeDll.FindRebar(), rebarOld))
                {
                    throw new Exception("Cannot calculate new Rebar position");
                }

                newSize = new System.Drawing.Size(rebarOld.Width - delta, rebarOld.Height);
                System.Drawing.Point ltTaskBar = NativeDll.GetTaskbarPos();
                newTopLeft = new System.Drawing.Point(rebarOld.Left + delta, rebarOld.Top - ltTaskBar.Y);
            }

			// reposition the window
            var szStart = NativeDll.GetStartButtonSize();
			System.Drawing.Point offset;
			var edge = NativeDll.GetTaskbarEdge();
			if (edge.IsVertical())
			{
			    offset = new Point(0, szStart.Height);
			}
			else
			{
			    offset = new Point(szStart.Width, 0);
			}

			var offset2 = ScreenFromWpf(offset, NativeDll.FindTaskBar());

            User32Dll.SetWindowPos(_hSrc.Handle, (IntPtr)WindowZOrderConstants.HWND_TOP,
								   offset2.X, offset2.Y,
                                   _buttonsWindowSize.Width, _buttonsWindowSize.Height,
                                   (uint)
                                   (SetWindowPosConstants.SWP_SHOWWINDOW | SetWindowPosConstants.SWP_NOOWNERZORDER |
                                    SetWindowPosConstants.SWP_NOACTIVATE));

            if (!User32Dll.SetWindowPos(NativeDll.FindRebar(), (IntPtr)0,
                                        newTopLeft.X, newTopLeft.Y,
                                        newSize.Width, newSize.Height,
                                        (uint)(SetWindowPosConstants.SWP_NOSENDCHANGING)))
            {
                return false;
            }

            return true;
        }

		private Point ScreenFromWpf(Point local, IntPtr hwnd)
		{
			var p = new POINT() { x = local.X, y = local.Y};
			if (User32Dll.MapWindowPoints(hwnd, IntPtr.Zero, ref p, 1) == 0) throw new Exception("Error converting points");
			return new Point(p.x, p.y);
		}
    }
}
