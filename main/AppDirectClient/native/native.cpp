#include "stdafx.h"
#include "native.h"
#include "consts.h"

static const int BuffSize = 256;

bool g_bInitDone = false;
HWND g_ReBar = NULL;		// of the taskbar
SIZE g_RebarOffset;
HHOOK g_ProgHook = NULL;
HMODULE g_hDll = NULL;

NATIVE_API HWND FindTaskBar()
{
	return ::FindWindow(L"Shell_TrayWnd", NULL);
}

static bool IsVertical()
{
	HWND taskBar = FindTaskBar();
	APPBARDATA appbar = {sizeof(appbar), taskBar};
    SHAppBarMessage(ABM_GETTASKBARPOS, &appbar);
	return (appbar.uEdge == ABE_LEFT) || (appbar.uEdge == ABE_RIGHT);
}

UINT GetExitMsg()
{
	return ::RegisterWindowMessage(L"AppDirectButtonsExit");
}

UINT GetUpdatePositionMsg()
{
	return ::RegisterWindowMessage(L"AppDirectButtonPositionUpdate");
}

HWND GetAppDirectHwnd()
{
	static const wchar_t * TheWndCaption = L"AppDirectTaskbarButtonsWindow";
	HWND appDirectHwnd = NULL;
	appDirectHwnd = ::FindWindowEx(NULL, NULL, NULL, TheWndCaption);	
	_ASSERT(appDirectHwnd);
	return appDirectHwnd;
}

static LRESULT CALLBACK SubclassTaskbarProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData);
static volatile bool bExiting = false;
static RECT buttonsRect;
static LRESULT CALLBACK SubclassRebarProc(const HWND hWnd, const UINT uMsg, const WPARAM wParam, const LPARAM lParam, 
										  const UINT_PTR uIdSubclass, const DWORD_PTR dwRefData)
{
	const APPDIRECT_IPC_MESSAGES* messages = ((APPDIRECT_IPC_MESSAGES*)dwRefData);

	if ((uMsg == WM_WINDOWPOSCHANGING) && !bExiting)
	{
		// prevent rebar from restoring position and hovering our button: return
		WINDOWPOS* p = (WINDOWPOS*)lParam;

		if (buttonsRect.left < (p->x + p->cx) && buttonsRect.right  > p->x &&
			buttonsRect.top  < (p->y + p->cy) && buttonsRect.bottom > p->y)
		{
			if (IsVertical())
			{
				const int deltaY = buttonsRect.bottom - p->y;
				if (deltaY > 0)
				{
					p->y += deltaY;
					p->cy -= deltaY;
				}
			}
			else
			{
				const int deltaX = buttonsRect.right - p->x;
				if (deltaX > 0)
				{
					p->x += deltaX;
					p->cx -= deltaX;
				}
			}
		}

		// And send them to the main application message queue
		::PostMessage(messages->AppDirectHwnd, messages->UpdateMessage, FALSE, NULL);

		return 0;	// this message is processed
	}
	else 
	{
		if (uMsg == messages->ExitMessage)
		{
			if (g_bInitDone)
			{
				bExiting = true;
				g_bInitDone = false;
				HWND rebarHwnd = FindRebar();	_ASSERT(rebarHwnd);
				BOOL b = ::RemoveWindowSubclass(rebarHwnd, SubclassRebarProc, 0); _ASSERT(b);

				HWND taskbar = FindTaskBar();	_ASSERT(taskbar);
				b = ::RemoveWindowSubclass(taskbar, SubclassTaskbarProc, 0); _ASSERT(b);

				// force repaint rebar by itself
				b = ::ShowWindow(rebarHwnd, SW_RESTORE);
				
				HWND theButton = GetAppDirectHwnd();
				_ASSERT(g_hDll);
				b = ::PostMessage(theButton, messages->ExitMessage, 0, (LPARAM)g_hDll);	_ASSERT(b);

				delete messages;

				g_hDll = NULL;
			}

			return 0;	// this message is processed
		}
		else if ((uMsg == messages->UpdateMessage) && !bExiting) 
		{
			buttonsRect.left = HIWORD(wParam);
			buttonsRect.top = LOWORD(wParam);
			buttonsRect.right = HIWORD(lParam);
			buttonsRect.bottom = LOWORD(lParam);

			return 0;	// this message is processed
		}
	}

	return DefSubclassProc(hWnd, uMsg, wParam, lParam);
}

static LRESULT CALLBACK SubclassTaskbarProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)
{
	if (bExiting) 
		return DefSubclassProc(hWnd, uMsg, wParam, lParam);

	const APPDIRECT_IPC_MESSAGES* messages = ((APPDIRECT_IPC_MESSAGES*)dwRefData);

	switch (uMsg)
	{
		// Handle on top / topmost Z-Order
		// place the button on top of Taskbar but not hover the
		case WM_WINDOWPOSCHANGED:
		{
			// place on top of task bar
			if (messages->AppDirectHwnd)
			{
				WINDOWPOS * p = (WINDOWPOS*)lParam;
				BOOL b = ::SetWindowPos(
						messages->AppDirectHwnd, 
						p->hwndInsertAfter,
						0, 0, 0, 0, 0
						| SWP_NOACTIVATE
						| SWP_NOMOVE
						| SWP_NOSIZE
						| SWP_ASYNCWINDOWPOS
						| SWP_NOOWNERZORDER
					);
				_ASSERT(b);
			}
			break;
		}
		case WM_MOVE:
			if (messages->AppDirectHwnd) {
				int xPos = (int)(short) LOWORD(lParam);
				int yPos = (int)(short) HIWORD(lParam);
				::SendMessage(messages->AppDirectHwnd, messages->UpdateMessage, TRUE, NULL);
			}
			break;
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

NATIVE_API LRESULT CALLBACK SetupHooks2(int code, WPARAM wParam, LPARAM lParam)
{
	if (code == HC_ACTION && !g_bInitDone)
	{
		g_bInitDone = true;
		APPDIRECT_IPC_MESSAGES* messages = new APPDIRECT_IPC_MESSAGES();
		messages->ExitMessage = GetExitMsg();
		messages->UpdateMessage = GetUpdatePositionMsg();
		messages->AppDirectHwnd = GetAppDirectHwnd();
		g_hDll = ::LoadLibrary(gc_TheDllName); _ASSERT(g_hDll);		// prevent dll from unloading
		BOOL b = ::SetWindowSubclass(FindRebar(), SubclassRebarProc, 0, (DWORD_PTR)messages);	_ASSERT(b);
		b = ::SetWindowSubclass(FindTaskBar(), SubclassTaskbarProc, 0, (DWORD_PTR)messages);	_ASSERT(b);
	}

	return ::CallNextHookEx(NULL, code, wParam, lParam);
}

volatile HHOOK ExplorerHook = NULL;
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
		BOOL b = ::UnhookWindowsHookEx(ExplorerHook); _ASSERT(b);
		ExplorerHook = NULL;
		HWND reb = FindRebar();	_ASSERT(reb);
		::PostMessage(reb, GetExitMsg(), 0, 0);
	}
}