﻿using Accessibility;
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
            p.WindowStyle = (int)(WindowsStyleConstants.WS_VISIBLE | WindowsStyleConstants.WS_CHILD);
            p.UsesPerPixelOpacity = true;
            _hSrc = new HwndSource(p);
            _hSrc.RootVisual = wnd;
            //_hSrc.AddHook(WndProc);		// handle custom WM_

            DoChangeWidth(initialWidth, true);

            NativeDll.InjectExplrorerExe();
   
            var taskberHwnd = NativeDll.FindTaskBar();
            uint taskbarProcessId;
            var taskbarThreadId = User32Dll.GetWindowThreadProcessId(taskberHwnd, out taskbarProcessId);

            // Hook resize event catcher
            _hWinEventHook = User32Dll.SetWinEventHook((uint)EventConstants.EVENT_SYSTEM_MOVESIZEEND, 
                (uint)EventConstants.EVENT_SYSTEM_MOVESIZEEND, NULL, WinEventDelegate,
                taskbarProcessId, taskbarThreadId, (uint)(WinEventHookFlags.WINEVENT_OUTOFCONTEXT | WinEventHookFlags.WINEVENT_SKIPOWNPROCESS));
        }

        private TaskbarPosition GetTaskbarPosition()
        {
            switch (NativeDll.GetTaskbarEdge())
            {
                case TaskbarPlacement.ABE_BOTTOM:
                    return TaskbarPosition.Bottom;
                case TaskbarPlacement.ABE_LEFT:
                    return TaskbarPosition.Left;
                case TaskbarPlacement.ABE_RIGHT:
                    return TaskbarPosition.Right;
                case TaskbarPlacement.ABE_TOP:
                    return TaskbarPosition.Top;
            }
            throw new Exception("Error in return position value");
        }

        private TaskbarIconsSize GetTaskbarIconSize()
        {
            var isSmall = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarSmallIcons", -1);
            switch (isSmall)
            {
                case 0:
                    return TaskbarIconsSize.Large;
                case 1:
                    return TaskbarIconsSize.Small;
            }
            throw new Exception("Failed to access registry");
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

                    var newTaskbarIconSize = GetTaskbarIconSize();
                    if (newTaskbarIconSize != _taskbarIconsSize)
                    {
                        _notifyee.TaskbarIconsSizeChanged(newTaskbarIconSize);
                        _taskbarIconsSize = newTaskbarIconSize;
                    }

                    break;
            }
        }

        private System.Drawing.Size GetCurrentButtonSize()
        {
            // TODO: -0
            return GetInitialWndSize();
        }

        public void Remove()
        {
            System.Drawing.Size newSize;	// calculate correct coords first
            System.Drawing.Point newTopLeft = RebarCoords(out newSize, false);

            NativeDll.DetachHooks();					// detach - can cause reposition by Rebar itself

            // Unhook resize event catcher
            User32Dll.UnhookWinEvent(_hWinEventHook);

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
			if (edge == TaskbarPlacement.ABE_LEFT || edge == TaskbarPlacement.ABE_RIGHT)		// if vertical
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
			if (edge == TaskbarPlacement.ABE_LEFT || edge == TaskbarPlacement.ABE_RIGHT)
			{
			    offset = new Point(0, szStart.Height);
			}
			else
			{
			    offset = new Point(szStart.Width, 0);
			}

            User32Dll.SetWindowPos(_hSrc.Handle, (IntPtr)WindowZOrderConstants.HWND_TOP,
								   offset.X, offset.Y,
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
    }
}
