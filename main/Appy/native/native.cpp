#include "stdafx.h"
#include "native.h"
#include "consts.h"

static const int BuffSize = 256;

bool g_bInitDone = false;
HWND _TaskBar = NULL;
HWND g_ReBar = NULL;		// of the taskbar
SIZE g_RebarOffset;
HHOOK g_ProgHook = NULL;
HMODULE g_hDll = NULL;

// TODO: -1: explode to 
// Returns Windows version as usually defined in VC headers: targetver.h
static WORD g_version = 0;

HWND GetTaskbar()
{
	if (!_TaskBar) _TaskBar = FindTaskBar();
	_ASSERT(_TaskBar);
	return _TaskBar;
}

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

	wchar_t s[BuffSize];
	::GetClassName(hwnd, s, BuffSize);

	if (_wcsnicmp(s, L"Shell_TrayWnd", BuffSize) != 0)	return TRUE;
	
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
	wchar_t s[BuffSize];
	int len = ::GetClassName(hwnd, s, BuffSize);
	if (_wcsnicmp(s, L"Button", BuffSize) != 0) return TRUE;
	tmpStartButton = hwnd;
	return FALSE;
}

HWND FindStartButton()
{
	DWORD pidExplorer = GetTaskbarThread();
	tmpStartButton = NULL;
	if (WinVersion() < _WIN32_WINNT_VISTA)
	{
		tmpStartButton = ::FindWindowEx(FindTaskBar(), NULL, L"Button", NULL);	_ASSERT(tmpStartButton);
	}
	else
	{
		::EnumThreadWindows(pidExplorer, StartButtonEnumFunc, NULL);	// find Start button
	}
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

	UINT edge = GetTaskbarEdge(GetTaskbar(), NULL, NULL, NULL);
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
	MONITORINFO info;
	return GetTaskbarEdge(GetTaskbar(), &info, NULL, NULL);
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
		//at("Hook FAILS! "); tx(err);
	}
	::PostMessage(FindRebar(), WM_NULL, 0, 0); // make sure there is one message in the queue
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

//SIZE GetStartButtonSize()
//{
//	RECT r;
//	BOOL b = ::GetWindowRect(GetStartButton(), &r);	_ASSERT(b);
//	return DimensionsFromRect(r);
//}
