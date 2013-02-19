﻿using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Interop;
using HINSTANCE = System.IntPtr;
using UINT = System.UInt32;
using DWORD = System.UInt32;
using HHOOK = System.IntPtr;
using HWND = System.IntPtr;
using HANDLE = System.IntPtr;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class InteractionsObject : ITaskbarInteropCallback
    {
        private static readonly IntPtr NULL = IntPtr.Zero;
        private const int MAX_LOADSTRING = 100;
        private const int WM_OPEN = (int)(WindowsMessages.WM_USER + 10);
        private const int TIMER_HOOK = 1;
        private const int UOI_NAME = 2;
        private HINSTANCE hInst;								// current instance
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
        private HwndSource hSrc;
        private ITaskbarInterop _notifyee = null;
        private System.Drawing.Size _buttonsWindowSize;

        //static IntPtr HookProc_SetupWndHooks(int nCode, IntPtr wParam, IntPtr lParam)
        //{
        //	if (nCode == HC_ACTION && !bInitDone)
        //	{
        //		bInitDone = true;
        //		hDll = Kernel32Dll.LoadLibrary("native.dll");	// prevent dll from unloading
        //		//bool b = Comctl32Dll.SetWindowSubclass(FindRebar(), SubclassRebarProc, 0, GetExitMsg());
        //	}

        //	return User32Dll.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        //}

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
            hSrc = new HwndSource(p);
            hSrc.RootVisual = wnd;

            //            hSrc.AddHook(WndProc);		// handle custom WM_

            // reposition the window
            DoChangeWidth(initialWidth, true);

            NativeDll.InjectExplrorerExe();
        }

        System.Drawing.Size GetCurrentButtonSize()
        {
            // TODO: -0
            return GetInitialWndSize();
        }

        public void Remove()
        {
            System.Drawing.Size NewSize;	// calculate correct coords first
            System.Drawing.Point NewTopLeft = RebarCoords(out NewSize, false);

            NativeDll.DetachHooks();					// detach - can cause reposition by Rebar itself

            if (!User32Dll.SetWindowPos(NativeDll.FindRebar(), (IntPtr)0,
                NewTopLeft.X, NewTopLeft.Y, NewSize.Width, NewSize.Height,
                0
                ))							// move to correct coords
            { throw new Exception("Cannot move Rebar back"); }

            hSrc.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NewSize"></param>
        /// <param name="IsInsert">if false: then calculate coords while removing our buttons and moving Rebar back</param>
        /// <returns>return top-left corner and new size</returns>
        private System.Drawing.Point RebarCoords(out System.Drawing.Size NewSize, bool IsInsert = true)
        {
            var szButton = _buttonsWindowSize;
            int d = IsInsert ? 1 : -1;

            var RebarOld = new RectWin();
            if (!User32Dll.GetWindowRect(NativeDll.FindRebar(), RebarOld))
            {
                throw new Exception("Cannot calculate new Rebar position");
            }

            //System.Drawing.Size szStart = NativeDll.GetStartButtonSize();

            System.Drawing.Point ltTaskBar = NativeDll.GetTaskbarPos();
            int newX = RebarOld.Left + d * szButton.Width;
            System.Drawing.Point RebarNewTopLeft = new System.Drawing.Point(newX, RebarOld.Top - ltTaskBar.Y);

            NewSize = new System.Drawing.Size(RebarOld.Width - d * szButton.Width, RebarOld.Height);

            // TODO: -0
            //SIZE RebarNewSize = { RebarOld.right - RebarOld.left - szInitial.cx, RebarOld.bottom - RebarOld.top };
            //if (edge == ABE_LEFT || edge == ABE_RIGHT)
            //{
            //	RebarNewTopLeft.x = RebarOld.left;
            //	uint buttonHeight = rc2.bottom - rc2.top;
            //	RebarNewTopLeft.y = RebarOld.top + buttonHeight;
            //	RebarNewSize.cx = RebarOld.right - RebarOld.left;
            //	RebarNewSize.cy = RebarOld.bottom - RebarOld.top - buttonHeight;
            //	::MapWindowPoints(NULL, g_TaskBar, &RebarNewTopLeft, 1);
            //}
//{
//var sb = new System.Text.StringBuilder();
//sb.AppendFormat("RebarOld = ({0},{1}) , size = ({2},{3})\n", RebarOld.Left, RebarOld.Top, RebarOld.Width, RebarOld.Height);
//sb.AppendFormat("szButton = ({0},{1}) \n", szButton.Width, szButton.Height);
//sb.AppendFormat("szStart = ({0},{1}) \n", szStart.Width, szStart.Height);
//sb.AppendFormat("RebarNewTopLeft = ({0},{1}) , NewSize = ({2},{3})\n", RebarNewTopLeft.X, RebarNewTopLeft.Y, NewSize.Width, NewSize.Height);
//System.Windows.MessageBox.Show(sb.ToString());
//}
            return RebarNewTopLeft;
        }

        // TODO: -0 rewrite in C#
        //private IntPtr FindTaskbar()
        //{
        //	throw new NotImplementedException();
        //}

        //		private void InjectHooks()
        //		{
        //			InjectExplrorerExe();

        //			//IntPtr hHookModule = Kernel32Dll.GetModuleHandle("native.dll");		// reference was not counted: do not release	
        //			//ExplorerHook = User32Dll.SetWindowsHookEx(HookType.WH_GETMESSAGE, SetupHooks2, hHookModule, GetRebarThread());
        //			//if (ExplorerHook != NULL) { throw new Exception("Cannot setup Explorer hook"); }
        ////			::PostMessage(FindRebar(), WM_NULL, 0, 0); // make sure there is one message in the queue


        //			//var hookProc = new HookProc(HookProc_SetupWndHooks);
        //			//ExplorerHook = User32Dll.SetWindowsHookEx(HookType.WH_GETMESSAGE, hookProc, Kernel32Dll.GetModuleHandle(null), 0);	
        //			//if (ExplorerHook == (IntPtr)0) { throw new Exception("Cannot inject hook in explorer.exe"); }
        //		}

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle messages...

            return NULL;
        }

        #region helpers
        // TODO: -0 rewrite in C#
        //private uint GetRebarThread()
        //{
        //	throw new NotImplementedException();
        //	return 0;
        //}

        private System.Drawing.Size GetInitialWndSize()
        {
            //return new System.Drawing.Size(54, 40);
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
            System.Drawing.Size NewSize;
            System.Drawing.Point NewTopLeft;
            if (firstTime)
            {
                NewTopLeft = RebarCoords(out NewSize);
            }
            else
            {
                var RebarOld = new RectWin();
                if (!User32Dll.GetWindowRect(NativeDll.FindRebar(), RebarOld))
                {
                    throw new Exception("Cannot calculate new Rebar position");
                }

                NewSize = new System.Drawing.Size(RebarOld.Width - delta, RebarOld.Height);
                System.Drawing.Point ltTaskBar = NativeDll.GetTaskbarPos();
                NewTopLeft = new System.Drawing.Point(RebarOld.Left + delta, RebarOld.Top - ltTaskBar.Y);
            }

            System.Drawing.Size szStart = NativeDll.GetStartButtonSize();
            User32Dll.SetWindowPos(hSrc.Handle, (IntPtr)WindowZOrderConstants.HWND_TOP,
                                   szStart.Width, 0, // offset
                                   _buttonsWindowSize.Width, _buttonsWindowSize.Height,
                                   (uint)
                                   (SetWindowPosConstants.SWP_SHOWWINDOW | SetWindowPosConstants.SWP_NOOWNERZORDER |
                                    SetWindowPosConstants.SWP_NOACTIVATE)
                );

            if (!User32Dll.SetWindowPos(NativeDll.FindRebar(), (IntPtr)0,
                                        NewTopLeft.X, NewTopLeft.Y,
                                        NewSize.Width, NewSize.Height,
                                        (uint)(SetWindowPosConstants.SWP_NOSENDCHANGING)
                     ))
            {
                return false;
            }

            return true;
        }
    }
}
