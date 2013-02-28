#include "stdafx.h"
#include "native.h"
#include "consts.h"
#include <mutex>
#include <string>
#include <sstream>

static const int BuffSize = 256;

bool g_bInitDone = false;
HWND g_ReBar = NULL;		// of the taskbar
SIZE g_RebarOffset;
HHOOK g_ProgHook = NULL;
HMODULE g_hDll = NULL;

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

static bool IsVertical()
{
	HWND taskBar = FindTaskBar();
	APPBARDATA appbar = {sizeof(appbar), taskBar};
    SHAppBarMessage(ABM_GETTASKBARPOS, &appbar);
	return (appbar.uEdge == ABE_LEFT) || (appbar.uEdge == ABE_RIGHT);
}

static volatile bool bExiting = false;
static std::mutex _mutex;
static RECT buttonsRect;
static LRESULT CALLBACK SubclassRebarProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)
{
	if ((uMsg == WM_WINDOWPOSCHANGING) && !bExiting)
	{
		// prevent rebar from restoring position and hovering our button: return
		WINDOWPOS* p = (WINDOWPOS*)lParam;

		std::unique_lock<std::mutex> lock(_mutex);

		//std::wstringstream sstm;
		//sstm << L"l: " << buttonsRect.left;
		//sstm << L"\nt: " << buttonsRect.top;
		//sstm << L"\nr: " << buttonsRect.right;
		//sstm << L"\nb: " << buttonsRect.bottom;

		//sstm << L"\np\nl: " << p->x;
		//sstm << L"\nt: " << p->y;
		//sstm << L"\nr: " << p->x + p->cx;
		//sstm << L"\nb: " << p->y + p->cy;

		//::MessageBox(NULL, sstm.str().c_str() , L"Changing", MB_OK);

		if (buttonsRect.left < (p->x + p->cx) && buttonsRect.right  > p->x &&
			buttonsRect.top  < (p->y + p->cy) && buttonsRect.bottom > p->y)
		{
			if (IsVertical())
			{
				const int deltaY = buttonsRect.bottom - p->y;
				//std::wstringstream sstm2;
				//sstm2 << L"y:" << deltaY;
				//::MessageBox(NULL, sstm2.str().c_str(), L"Check2", MB_OK);
				if (deltaY > 0)
				{
					p->y += deltaY;
					p->cy -= deltaY;
				}
			}
			else
			{
				const int deltaX = buttonsRect.right - p->x;
				//std::wstringstream sstm2;
				//sstm2 << L"x: " << deltaX;
				//::MessageBox(NULL, sstm2.str().c_str(), L"Check3", MB_OK);
				if (deltaX > 0)
				{
					p->x += deltaX;
					p->cx -= deltaX;
				}
			}
		}

		return 0;	// this message is processed
	}
	else 
	{
		APPDIRECT_IPC_MESSAGES* messages = ((APPDIRECT_IPC_MESSAGES*)dwRefData);
		if (uMsg == messages->ExitMessage)		// WM_adButtonsExit
		{
			if (g_bInitDone)
			{
				bExiting = true;
				g_bInitDone = false;
				HWND rebarHwnd = FindRebar();

				BOOL b = ::RemoveWindowSubclass(rebarHwnd, SubclassRebarProc, 0); _ASSERT(b);

				// TODO: -1 unload not from itself
				//	b = ::FreeLibrary(g_hDll);	_ASSERT(b);
				// g_hDll = NULL;

				// force repaint rebar by itself
				b = ::ShowWindow(rebarHwnd, SW_RESTORE);

				delete messages;
			}
			bExiting = false;
			return 0;	// this message is processed
		} 
		else if (uMsg == messages->UpdateMessage) 
		{
			std::unique_lock<std::mutex> lock(_mutex);

			const unsigned int p1 = (unsigned int)wParam;
			const unsigned int p2 = (unsigned int)lParam;
			buttonsRect.left = p1 >> 16;
			buttonsRect.top = p1 & 0xFFFF;
			buttonsRect.right = p2 >> 16;
			buttonsRect.bottom = p2 & 0xFFFF;
		}
	}

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

UINT GetExitMsg()
{
	return ::RegisterWindowMessage(L"AppDirectButtonsExit");
}

UINT GetUpdatePositionMsg()
{
	return ::RegisterWindowMessage(L"AppDirectButtonPositionUpdate");
}

NATIVE_API LRESULT CALLBACK SetupHooks2(int code, WPARAM wParam, LPARAM lParam)
{
	if (code == HC_ACTION && !g_bInitDone)
	{
		g_bInitDone = true;
		APPDIRECT_IPC_MESSAGES* messages = new APPDIRECT_IPC_MESSAGES();
		messages->ExitMessage = GetExitMsg();
		messages->UpdateMessage = GetUpdatePositionMsg();
		g_hDll = ::LoadLibrary(gc_TheDllName); _ASSERT(g_hDll);		// prevent dll from unloading
		BOOL b = ::SetWindowSubclass(FindRebar(), SubclassRebarProc, 0, (DWORD_PTR)messages);	_ASSERT(b);
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
		b = ::PostMessage(reb, GetExitMsg(), 0, 0);	_ASSERT(b);
	}
}