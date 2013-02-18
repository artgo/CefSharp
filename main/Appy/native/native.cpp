#include "stdafx.h"
#include "native.h"
#include "ADButton.h"
#include "consts.h"

#include "dbgh.h"	// TODO: -2 remove after release



// TODO: -1
//// This is an example of an exported variable
//NATIVE_API int nnative=0;
//
//// This is an example of an exported function.
//NATIVE_API int fnnative()
//{
//	return 42;
//}
//
//// This is the constructor of a class that has been exported.
//// see native.h for the class definition
//Cnative::Cnative()
//{
//	return;
//}


//-------------------------------------------------------------------------------------------------
//namespace adButton 
//{
//-------------------------------------------------------------------------------------------------
static const int BuffSize = 256;

bool g_bInitDone = false;
HWND g_TaskBar = NULL;
HWND g_ReBar = NULL;		// of the taskbar
SIZE g_RebarOffset;
HHOOK g_ProgHook = NULL;
HMODULE g_hDll = NULL;

static void InitTheDll();

// TODO: -1: explode to 
// Returns Windows version as usually defined in VC headers: targetver.h
static WORD g_version = 0;

WORD WinVersion()
{	
	if (!g_version)
	{
		DWORD v = ::GetVersion();
		g_version = MAKEWORD(HIBYTE(v), LOBYTE(v) );
	}
	return g_version;
}

//static BOOL CALLBACK TooltipEnumFunc(HWND hwnd, LPARAM lParam)
//{
//	CStringW s;
//	wchar_t * name = s.GetBuffer(BuffSize);
//	GetClassName(hwnd, name, BuffSize);
//	s.ReleaseBuffer();
//	if (s.CompareNoCase(TOOLTIPS_CLASS) != 0 ) return TRUE;
//	TOOLINFO info = { sizeof(info), 0, g_TaskBar, (UINT_PTR)g_WinStartButton };
//	if ( ::SendMessage(hwnd, TTM_GETTOOLINFO, 0, (LPARAM)&info) )
//	{
//		g_Tooltip = hwnd;
//		return FALSE;
//	}
//	return TRUE;
//}




static HWND tmpTaskbar = NULL;

// look for top-level window with class "Shell_TrayWnd" and process ID=lParam
static BOOL CALLBACK TaskBarEnumFunc(HWND hwnd, LPARAM lParam)
{
	DWORD process;
	::GetWindowThreadProcessId(hwnd, &process);
	if (process != lParam) return TRUE;

	CStringW s;
	::GetClassName(hwnd, s.GetBuffer(BuffSize), BuffSize);
	s.ReleaseBuffer();
	if (s.CompareNoCase(L"Shell_TrayWnd") != 0)	return TRUE;
	
	tmpTaskbar = hwnd;
	return FALSE;
}

NATIVE_API HWND FindTaskBar()
{
	return FindTaskBar(GetExplorerProcess());
}

// Find the taskbar window for the given process
// Perform system call every time
HWND FindTaskBar(DWORD process)
{
	tmpTaskbar = NULL;
	::EnumWindows(TaskBarEnumFunc, process);	// changing tmpTaskbar
	_ASSERT(tmpTaskbar);
	return tmpTaskbar;
}


static HWND tmpStartButton = NULL;
static BOOL CALLBACK StartButtonEnumFunc(HWND hwnd, LPARAM lParam)
{
	CStringW s;
	int len = ::GetClassName(hwnd, s.GetBuffer(BuffSize), BuffSize);
	s.ReleaseBuffer();
	if (s.CompareNoCase(L"button") != 0) return TRUE;
	tmpStartButton = hwnd;
	return FALSE;
}

HWND FindStartButton()
{
	DWORD pidExplorer = GetTaskbarThread();
	tmpStartButton = NULL;
	::EnumThreadWindows(pidExplorer, StartButtonEnumFunc, NULL);	// find Start button
	_ASSERT(WinVersion() >= _WIN32_WINNT_WIN8 || tmpStartButton);
	return tmpStartButton;
}

SIZE GetStartButtonSize()
{
	HWND sb = FindStartButton();
	RECT r;
	BOOL b = ::GetWindowRect(sb, &r);		_ASSERT(WinVersion() >= _WIN32_WINNT_WIN8 || b);
	SIZE sz = {0,0};
	if (b) sz = SizeOfRect(r);
	return sz;
}

SIZE SizeOfRect(const RECT& r)
{
	SIZE sz = { r.right - r.left, r.bottom - r.top};
	return sz;
}

SIZE GetInitialADButtonSize()
{
	// TODO: -2 save/load
	// TODO: -2 accomodate to the Taskbar actual size
	SIZE tb = GetTaskbarSize();

	UINT edge = GetTaskbarEdge(g_TaskBar, NULL, NULL, NULL);
	SIZE s2;
	if (WinVersion() < _WIN32_WINNT_WIN8)
	{
		SIZE sb =  GetStartButtonSize();
		s2 = sb;
		
		if (edge == ABE_LEFT || edge == ABE_RIGHT)
		{	// vertical taskbar
			s2.cy =	40;

			// TODO: -2 implement code block bellow if we need some locig for small icons
			//if (IsTaskbarSmallIcons())
			//{
			//	s2.cy = 
			//}
		}
		else
		{
			if (s2.cy > tb.cy)	// limit height: horizontal taskbar with small icons has Start button window out of the screen
				s2.cy = tb.cy;		
		}
	}
	else	// win8, 9, ...
	{
		s2.cx = gc_DefaultButtonWidth;
		s2.cy = tb.cy;
		// TODO: -0 vertical taskbar
	}
	return s2;
}



// TODO: -1
// WH_GETMESSAGE hook for the Progman window
NATIVE_API LRESULT CALLBACK HookProgMan(int code, WPARAM wParam, LPARAM lParam )
{
	if (code == HC_ACTION)
	{
		MSG *msg = (MSG*)lParam;
		if (msg->message == WM_SYSCOMMAND && (msg->wParam & 0xFFF0) == SC_TASKLIST)
		{
			// Win button pressed
			if (msg->lParam=='CLSM')
			{
				// TODO: -3
			}
			else
			{
				// FindTaskBar();
				// TODO: -1 open main menu here
			}
		}
	}
	return CallNextHookEx(NULL, code, wParam, lParam);
}

// true if the mouse pointer is on the taskbar portion of the button
bool PointAroundADButton()
{
	if (!g_TaskBar) return false;
	CPoint pt(GetMessagePos());
	RECT rc;
	GetWindowRect(g_TaskBar, &rc);
	if (!PtInRect(&rc, pt))
		return false;

	UINT uEdge = GetTaskbarEdge(g_TaskBar, NULL, NULL, NULL);

	// Win7 Start button
	if (uEdge == ABE_LEFT || uEdge == ABE_RIGHT)
	{
		pt.x = (rc.left + rc.right) /2;		// vertical taskbar, set X
		if (pt.y > (rc.top + rc.bottom) /2)
			return false;
	}
	else
	{
		pt.y = (rc.top + rc.bottom) /2;		// vertical taskbar, set Y
	}
	ScreenToClient(g_TaskBar, &pt);
	HWND child = ChildWindowFromPointEx(g_TaskBar, pt, CWP_SKIPINVISIBLE | CWP_SKIPTRANSPARENT);
	if (child != NULL && child != g_TaskBar)
	{
		// ignore the click if it is on a child window (like the rebar or the tray area)
		return false;
	}

		//// take into account the button itself
		//RECT rc2;
		//GetWindowRect(g_TheButton, &rc2);
		//if (uEdge == ABE_LEFT || uEdge == ABE_RIGHT)
		//{
		//	rc2.left = rc.left;
		//	rc2.right = rc.right;
		//}
		//else
		//{
		//	rc2.top = rc.top;
		//	rc2.bottom = rc.bottom;
		//}
		//return PtInRect(&rc2, pt) != 0;

	return true;
}

static LRESULT CALLBACK SubclassTaskBarProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)
{
	if (uMsg == WM_MOUSEACTIVATE && HIWORD(lParam)==WM_MBUTTONDOWN)
	{
		// TODO: -2 menu
		// TODO: -1 marshal to WPF
		return MA_ACTIVATEANDEAT; 
	}

	if (!g_TaskBar) return DefSubclassProc(hWnd, uMsg, wParam, lParam);

	if ((uMsg == WM_NCMOUSEMOVE || uMsg == WM_MOUSEMOVE) && PointAroundADButton() )
		g_TheButton.TaskBarMouseMove();

	if (uMsg == WM_WINDOWPOSCHANGED)
	{
		if (IsADButtonSmallIcons() != IsTaskbarSmallIcons() )
			RecreateADButton();

		WINDOWPOS * pPos = (WINDOWPOS*)lParam;
		RECT rcTask;
		::GetWindowRect(hWnd, &rcTask);
		MONITORINFO info;
		UINT uEdge = GetTaskbarEdge(hWnd, &info, NULL, NULL);
		DWORD flags = SWP_NOACTIVATE | SWP_NOOWNERZORDER | SWP_SHOWWINDOW | SWP_NOSIZE;
		APPBARDATA appbar = { sizeof(appbar) };
			
		if (SHAppBarMessage(ABM_GETSTATE, &appbar) & ABS_AUTOHIDE)
		{
			bool bHide=false;
			if (uEdge == ABE_LEFT)
				bHide = (rcTask.right < info.rcMonitor.left + 5);
			else if (uEdge == ABE_RIGHT)
				bHide = (rcTask.left > info.rcMonitor.right - 5);
			else if (uEdge == ABE_TOP)
				bHide = (rcTask.bottom < info.rcMonitor.top + 5);
			else
				bHide = (rcTask.top > info.rcMonitor.bottom - 5);
			if (bHide)
				flags = (flags & ~SWP_SHOWWINDOW) | SWP_HIDEWINDOW;
		}
		if (uEdge == ABE_TOP || uEdge == ABE_BOTTOM)
		{
			if (rcTask.left < info.rcMonitor.left) rcTask.left = info.rcMonitor.left;
			if (rcTask.right > info.rcMonitor.right) rcTask.right = info.rcMonitor.right;
		}
		else
		{
			if (rcTask.top < info.rcMonitor.top) rcTask.top = info.rcMonitor.top;
		}
		if (!IsADButtonSmallIcons())
		{
			if (uEdge == ABE_TOP) OffsetRect(&rcTask, 0, -1);
			else 
			if (uEdge == ABE_BOTTOM) OffsetRect(&rcTask, 0, 1);
		}
		HWND zPos = (pPos->flags & SWP_NOZORDER) ? HWND_TOPMOST : pPos->hwndInsertAfter;
		if (zPos == HWND_TOP 
//			&& !(::GetWindowLong(g_WinStartButton, GWL_EXSTYLE) & WS_EX_TOPMOST)
			)
			zPos = HWND_TOPMOST;

		SIZE size = GetADButtonSize();
		int x, y;
		if (uEdge == ABE_LEFT || uEdge == ABE_RIGHT)
		{
			x=(rcTask.left+rcTask.right-size.cx)/2;
			y=rcTask.top;
		}
		else if (GetWindowLong(g_ReBar, GWL_EXSTYLE) & WS_EX_LAYOUTRTL)
		{
			x = rcTask.right - size.cx;
			y = (rcTask.top + rcTask.bottom - size.cy) /2;
		}
		else
		{
			x = rcTask.left;
			y = (rcTask.top + rcTask.bottom - size.cy) /2;
		}
		RECT rcButton = {x, y, x + size.cx, y + size.cy};
		RECT rc;
		IntersectRect(&rc, &rcButton, &info.rcMonitor);
		HRGN rgn = ::CreateRectRgn(rc.left - x, rc.top - y, rc.right - x, rc.bottom - y);
		if (!::SetWindowRgn(g_TheButton, rgn, FALSE))
			::DeleteObject(rgn);
		g_TheButton.SetWindowPos(zPos, x, y, 0, 0, flags);
	}
	if (uMsg == WM_THEMECHANGED)
	{
		RecreateADButton();
	}
	if (uMsg == WM_PAINT 
	//	&& g_WinStartButton
		)
	{
		//PAINTSTRUCT ps;
		//HDC hdc = BeginPaint(hWnd, &ps);

		//EndPaint(hWnd, &ps);
		//return 0;
	}
	if (uMsg == WM_DESTROY)
	{
		DestroyADButton();
	}
	if (uMsg == WM_TIMER && wParam == 'CLSM')
	{
	}
	return DefSubclassProc(hWnd, uMsg, wParam, lParam);
}

static bool bExiting = false;
static LRESULT CALLBACK SubclassRebarProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)
{
	if (uMsg == WM_WINDOWPOSCHANGING)
	{
		if (!bExiting)
		{
			// prevent rebar from restoring position and hovering our button: return
			WINDOWPOS * p = (WINDOWPOS*)lParam;
			p->flags |= SWP_NOMOVE;
			p->flags |= SWP_NOSIZE;
		}
		return 0;	// this message is processed
	}
	else { if (uMsg == dwRefData)		// WM_adButtonsExit
	{
		if (g_bInitDone)
		{
			bExiting = true;
			g_bInitDone = false;

			BOOL b = ::RemoveWindowSubclass(FindRebar(), SubclassRebarProc, 0);	_ASSERT(b);

// TODO: -1 unload not from itself
//			b = ::FreeLibrary(g_hDll);	_ASSERT(b);
			// g_hDll = NULL;
			
			// force repaint rebar by itself
			b = ::ShowWindow(FindRebar(), SW_RESTORE);
		}
		bExiting = false;
		return 0;	// this message is processed
	}}
	return DefSubclassProc(hWnd, uMsg, wParam, lParam);
}

HWND GetProgmanHwnd()
{
	HWND progWin = ::FindWindowEx(NULL, NULL, L"Progman", NULL);	_ASSERT(progWin);
	return progWin;
}

DWORD GetExplorerProcess()
{
	DWORD ExplorerExeProcess;
	DWORD ProgmanThread = ::GetWindowThreadProcessId(GetProgmanHwnd(), &ExplorerExeProcess);
	return ExplorerExeProcess;
}

DWORD GetRebarThread()
{
	return ThreadFromWnd(FindRebar());
}

DWORD GetTaskbarThread()
{
	return ThreadFromWnd(FindTaskBar());
}

DWORD ThreadFromWnd(HWND wnd)
{
	DWORD ExplProcess;
	DWORD thr = GetWindowThreadProcessId(wnd, &ExplProcess);	_ASSERT(thr);
	return thr;
}

HWND FindRebar()
{
	return FindRebar(FindTaskBar());
}

HWND FindRebar(HWND ParentTaskbar)
{
	HWND h = ::FindWindowEx(ParentTaskbar, NULL, REBARCLASSNAME, NULL);	_ASSERT(h);	
	return 	h;
}

SIZE GetTaskbarSize()
{
	MONITORINFO mi;
	RECT r;
	GetTaskbarEdge(FindTaskBar(), &mi, NULL, &r);
	return SizeOfRect(r);
}

// return global coords of top left conner of taskbar
POINT GetTaskbarPos()
{
	HWND h = FindTaskBar();
	MONITORINFO Info;
	RECT r;
	GetTaskbarEdge(h, &Info, NULL, &r);
	POINT LeftTop = {r.left, r.top};
	return LeftTop;
}

// rect into two points
void RectToPoints(const RECT& r, POINT * pp)
{
	_ASSERT(pp);
	pp[0].x = r.left; pp[0].y = r.top;
	pp[1].x = r.right; pp[1].y = r.bottom;
}

// two points into rect
void PointsToRect(const POINT * pp, RECT& r)
{
	_ASSERT(pp);
	r.left = pp[0].x; r.top = pp[0].y;
	r.right = pp[1].x;  r.bottom = pp[1].y;
}

// return global coords of left top conner of our window = placeholder of buttons
POINT GetADButtonWndPos()
{
	POINT p = GetTaskbarPos();
	SIZE s = GetStartButtonSize();
	p.x += s.cx;	// TODO: -1 defferent position of taskbar
	return p;
}

// return one of 4 possible edge
UINT GetTaskbarEdge(HWND taskBar, MONITORINFO * pInfo, HMONITOR * pMonitor, RECT * TaskbarRect)
{
	if (!::IsWindow(taskBar)) return 0xFFFFFFFF;	// TODO: -1 signature of the function and returning of error

	APPBARDATA appbar = {sizeof(appbar), taskBar};
	SHAppBarMessage(ABM_GETTASKBARPOS, &appbar);
	if (TaskbarRect) 
		{ *TaskbarRect = appbar.rc; }
	if (pInfo)
	{
		pInfo->cbSize = sizeof(MONITORINFO);
		HMONITOR monitor = ::MonitorFromRect(&appbar.rc, MONITOR_DEFAULTTONEAREST);
		BOOL b = ::GetMonitorInfo(monitor, pInfo); _ASSERT(b);
		if (pMonitor) 
			{ *pMonitor = monitor; }
	}
	return appbar.uEdge;
}

UINT GetTaskbarEdge()
{
	_ASSERT(g_TaskBar);
	MONITORINFO info;
	return GetTaskbarEdge(g_TaskBar, &info, NULL, NULL);
}


UINT GetExitMsg()
{
	return ::RegisterWindowMessage(L"adButtonsExit");
}



// TODO: -1 merge into actual SetupHooks()
NATIVE_API LRESULT CALLBACK SetupHooks2(int code, WPARAM wParam, LPARAM lParam)
{
	if (code == HC_ACTION && !g_bInitDone)
	{
		g_bInitDone = true;
		g_hDll = ::LoadLibrary(gc_TheDllName); _ASSERT(g_hDll);		// prevent dll from unloading
		BOOL b = ::SetWindowSubclass(FindRebar(), SubclassRebarProc, 0, GetExitMsg());	_ASSERT(b);
	}
	return ::CallNextHookEx(NULL, code, wParam, lParam);
}

HHOOK ExplorerHook = NULL;
void InjectExplrorerExe()
{
	// install hooks in the explorer process: 
	// we are hooking another process and must use DLL http://msdn.microsoft.com/en-us/library/windows/desktop/ms644990(v=vs.85).aspx

	HMODULE hHookModule = ::GetModuleHandle(gc_TheDllName);		_ASSERT(hHookModule);	// reference was not counted: do not release	
	ExplorerHook = ::SetWindowsHookEx(WH_GETMESSAGE, SetupHooks2, hHookModule, GetRebarThread());	_ASSERT(ExplorerHook);
	if (!ExplorerHook)
	{
		int err = GetLastError();
		at("Hook FAILS! "); tx(err);
	}
	::PostMessage(FindRebar(), WM_NULL, 0, 0); // make sure there is one message in the queue
}

void RecreateADButton()
{
	DestroyADButton();

	RECT TBRect;
	::GetWindowRect(g_TaskBar, &TBRect);
	RECT TBRect2 = TBRect;
	MONITORINFO info;
	UINT edge = GetTaskbarEdge(g_TaskBar, &info, NULL, NULL);
	if (edge == ABE_TOP || edge == ABE_BOTTOM)
	{
		if (TBRect.left < info.rcMonitor.left) TBRect.left = info.rcMonitor.left;
		if (TBRect.right > info.rcMonitor.right) TBRect.right = info.rcMonitor.right;
	}
	else
	{
		if (TBRect.top < info.rcMonitor.top) TBRect.top = info.rcMonitor.top;
	}


//_ASSERT(g_ReBar);
//BOOL b = ::SetWindowSubclass(g_ReBar, SubclassRebarProc, 'CLSH', 0);	_ASSERT(b);
	

	CreateADButton(FindTaskBar(), g_ReBar, TBRect2, edge);	// CreateADButton(g_TaskBar, g_ReBar, TBRect2, edge);
	InjectExplrorerExe();

//	g_RebarOffset = GetADButtonSize();	// future offcet

//	::PostMessage(g_TaskBar, WM_SIZE, SIZE_RESTORED, MAKELONG(TBRect.right - TBRect.left, TBRect.bottom - TBRect.top));
}

void DetachHooks()
{
	if (ExplorerHook)
	{
		BOOL b = ::UnhookWindowsHookEx(ExplorerHook);	_ASSERT(b);
		ExplorerHook = NULL;
		HWND reb = FindRebar();	_ASSERT(reb);
		//b = ::SendMessage(reb, GetExitMsg(), 0, 0);	_ASSERT(b);
		b = ::PostMessage(reb, GetExitMsg(), 0, 0);	_ASSERT(b);
	}
}

void ReDestroyADButton()
{
	DetachHooks();
	DestroyADButton();
}

//static LRESULT CALLBACK HookAppManager(int code, WPARAM wParam, LPARAM lParam)
//{
//	if (code == HC_ACTION)
//	{
//		//UINT uEdge = GetTaskbarEdge(g_TaskBar, NULL, NULL, NULL);
//		//if (uEdge == ABE_BOTTOM)
//		//{
//		//	// check if the mouse is over the taskbar
//		//	RECT rc;
//		//	GetWindowRect(it->second.taskBar,&rc);
//		//	CPoint pt(GetMessagePos());
//		//	if (PtInRect(&rc,pt))
//		//	{
//		//		if (msg->message==WM_LBUTTONDOWN)
//		//		{
//		//			// forward the mouse click to the taskbar
//		//			PostMessage(it->second.taskBar,WM_NCLBUTTONDOWN,MK_LBUTTON,MAKELONG(pt.x,pt.y));
//		//			msg->message=WM_NULL;
//		//		}
//		//		wchar_t className[256]={0};
//		//		GetClassName(msg->hwnd,className,_countof(className));
//		//		if (wcscmp(className,L"ImmersiveSwitchList")==0)
//		//		{
//		//			// suppress the opening of the ImmersiveSwitchList
//		//			msg->message=WM_NULL;
//		//			ShowWindow(msg->hwnd,SW_HIDE); // hide the popup
//		//			g_SwitchList=msg->hwnd;
//		//		}
//		//	}
//		//}
//	}
//	return CallNextHookEx(NULL,code,wParam,lParam);
//}
static void InitTheDll()
{
	if (g_bInitDone) return;

	srand(GetTickCount());
	
{ ATLTRACE2("@@@@@@"); DWORD pr = ::GetCurrentProcessId(); td(pr); tx(pr); tx(g_TaskBar); }

	HWND ProgWin = ::FindWindowEx(NULL, NULL, L"Progman", NULL);
	DWORD ExplorerProcess;
	DWORD ProgmanThread = ::GetWindowThreadProcessId(ProgWin, &ExplorerProcess);
	g_TaskBar = FindTaskBar(ExplorerProcess); _ASSERT(g_TaskBar);
	DWORD ExplorerProcess2;
	DWORD TaskbarThread = ::GetWindowThreadProcessId(g_TaskBar, &ExplorerProcess2);

	g_ProgHook = ::SetWindowsHookEx(WH_GETMESSAGE, HookProgMan, NULL, TaskbarThread);
_ASSERT(g_ProgHook);
//	g_StartHook = ::SetWindowsHookEx(WH_GETMESSAGE, HookStartButton, NULL, GetCurrentThreadId() );

	HWND hwnd = ::FindWindow(START_HOOK_WND_CLASS, START_HOOK_WND_NAME);
tx(hwnd);
	::LoadLibrary(gc_TheDllName);						// keep the DLL from unloading
	if (hwnd) ::PostMessage(hwnd, WM_CLEAR, 0, 0);		// tell the exe to unhook this hook

	g_ReBar = ::FindWindowEx(g_TaskBar, NULL, REBARCLASSNAME, NULL);
tx(g_ReBar);
	if (g_ReBar)
	{
// TODO: -1 test
//			::SendMessage(g_TaskBar, WM_SETTINGCHANGE, 0, 0);
		::SetWindowSubclass(g_ReBar, SubclassRebarProc, 'CLSH', 0);
	}
	::SetWindowSubclass(g_TaskBar, SubclassTaskBarProc, 'CLSH', 0);
	RecreateADButton();


	//if (GetWinVersion() >= _WIN32_WINNT_WIN8)
	//{
	//	g_AppManager = ::FindWindow(L"ApplicationManager_DesktopShellWindow", NULL);
	//	if (g_AppManager)
	//	{
	//		DWORD thread = ::GetWindowThreadProcessId(g_AppManager, NULL);
	//		g_AppManagerHook = SetWindowsHookEx(WH_GETMESSAGE, HookAppManager, g_Instance, thread);
	//		g_pAppVisibility.CoCreateInstance(CLSID_MetroMode);
	//		if (g_pAppVisibility)
	//		{
	//			CMonitorModeEvents *monitor = new CMonitorModeEvents();
	//			g_pAppVisibility->Advise(monitor, &g_AppVisibilityMonitorCookie);
	//			monitor->Release();
	//		}
	//	}
	//}

	//::EnumWindows(HookAllTaskbarsEnum, 0);
	//g_NewTaskbarHook = ::SetWindowsHookEx(WH_CBT, HookNewTaskbar, g_Instance, GetCurrentThreadId());

	g_bInitDone = true;
}

// WH_GETMESSAGE hook for the explorer's GUI thread. The start menu exe uses this hook to inject code into the explorer process
NATIVE_API LRESULT CALLBACK SetupHooks(int code, WPARAM wParam, LPARAM lParam)
{
	if (g_bInitDone) return 0;
::MessageBox(NULL, L"SetupHooks", L"SetupHooks", MB_OK);
ATLTRACE2("SetupHooks(): "); tx(g_TaskBar);
	if (code == HC_ACTION 
		//&& !g_TaskBar
		)
		InitTheDll();
	return 0;	//  ::CallNextHookEx(NULL, code, wParam, lParam);
}



//SIZE DimensionsFromRect(const RECT & r)
//{
//	return SIZE() { r.rihgt - r.left, r.bottom - r.top};
//}
//
//static BOOL CALLBACK TaskBarEnumFunc(HWND hwnd, LPARAM lParam)
//{
//	
//	if (s.CompareNoCase(L"Shell_TrayWnd") != 0)
//		return TRUE;
//	tmpTaskbar = hwnd;
//	return FALSE;
//}
//
//// Find the taskbar window for the given process
//HWND FindTaskBar(DWORD process)
//{
////	g_WinStartButton = NULL;
//
//// TODO: -1 rewrite to system call every time
////	tmpTaskbar = NULL;
//	if (!tmpTaskbar)
//	{
//		::EnumWindows(TaskBarEnumFunc, process);	// changing globals
//		if (tmpTaskbar)
//		{
//HWND GetStartButton()
//{
//	
//}
//
//SIZE GetStartButtonSize()
//{
//	RECT r;
//	BOOL b = ::GetWindowRect(GetStartButton(), &r);	_ASSERT(b);
//	return DimensionsFromRect(r);
//}
//-------------------------------------------------------------------------------------------------
//}	// end of namespace adButton 
//-------------------------------------------------------------------------------------------------