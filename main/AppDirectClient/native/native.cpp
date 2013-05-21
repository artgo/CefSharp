#include "stdafx.h"
#include "native.h"

// The following messages are registered with ::RegisterWindowsMessage
// because they are used across application
// NB! MAKE SURE THAT THESE VALUES STAY IN SYNC WITH THE APPLICATION AND THE INSTALLER!!!
#define APPDIRECT_MESSAGE_NAME_NATIVE_TERMINATE L"AppDirectNativeTerminateMessage"
#define APPDIRECT_MESSAGE_NAME_NATIVE_UPDATE_OFFSET L"AppDirectNativeUpdateOffsetMessage"
#define APPDIRECT_MESSAGE_NAME_MANAGED_REBAR_UPDATED L"AppDirectManagedReBarUpdatedMessage"
#define APPDIRECT_MESSAGE_NAME_MANAGED_TASKBAR_UPDATED L"AppDirectManagedTaskBarUpdatedMessage"
UINT WM_APPDIRECT_NATIVE_UPDATE_OFFSET = 0;
UINT WM_APPDIRECT_NATIVE_TERMINATE = 0;
UINT WM_APPDIRECT_MANAGED_REBAR_UPDATED = 0;
UINT WM_APPDIRECT_MANAGED_TASKBAR_UPDATED = 0;

#define WM_APPDIRECT_SETUP_SUBCLASS WM_USER + 1
#define WM_APPDIRECT_TEARDOWN_SUBCLASS WM_USER + 2
#define WM_APPDIRECT_IS_SUBCLASSED WM_USER + 3


BOOL g_bIsLoaded = FALSE;
HMODULE g_hModule = NULL;
int g_ReBarOffset = 0;

HWND FindTaskBar();
HWND FindReBar(HWND hwndTaskBar);
BOOL IsVertical(HWND hwndTaskBar = NULL);
BOOL DoSetupSubclass();
BOOL DoTearDownSubclass(BOOL bAsync = FALSE, BOOL bResetReBar = FALSE);
BOOL ResetReBar(HWND hwndTaskBar, HWND hwndReBar, int offset);

HWND FindTaskBar()
{
	return ::FindWindow(L"Shell_TrayWnd", NULL);
}

HWND FindReBar(HWND hwndTaskBar)
{
	return ::FindWindowEx(hwndTaskBar, NULL, REBARCLASSNAME, NULL);
}

BOOL IsVertical(HWND taskBar)
{
	if (taskBar == NULL) {
		taskBar = FindTaskBar();
	}

	APPBARDATA appbar = {sizeof(appbar), taskBar};
    SHAppBarMessage(ABM_GETTASKBARPOS, &appbar);
	return (appbar.uEdge == ABE_LEFT) || (appbar.uEdge == ABE_RIGHT);
}

LRESULT CALLBACK SubclassRebarProc(const HWND hWnd, const UINT uMsg, const WPARAM wParam, const LPARAM lParam, 
										  const UINT_PTR uIdSubclass, const DWORD_PTR dwRefData)
{
	HWND hwndAdButton = (HWND)dwRefData;

	if (!::IsWindow(hwndAdButton)) {
		LRESULT lResult = DefSubclassProc(hWnd, uMsg, wParam, lParam);
		DoTearDownSubclass(TRUE, TRUE);
		return lResult;
	} else if (uMsg == WM_APPDIRECT_NATIVE_TERMINATE) {
		return DoTearDownSubclass(TRUE);
	} else if (uMsg == WM_APPDIRECT_NATIVE_UPDATE_OFFSET) {
		g_ReBarOffset = (int)(wParam);
		return 0;
	} else if (uMsg == WM_WINDOWPOSCHANGING) {
		WINDOWPOS* p = (WINDOWPOS*)lParam;

		LPARAM lParamUpdate = MAKELPARAM(p->y, p->x);
		LPARAM wParamUpdate = MAKEWPARAM(p->y + p->cy, p->x + p->cx);

		if (g_ReBarOffset != 0) {
			if (IsVertical()) {
				p->y += g_ReBarOffset;
				p->cy -= g_ReBarOffset;
			} else {
				p->x += g_ReBarOffset;
				p->cx -= g_ReBarOffset;
			}
		}

		// Notify the application that the button might need to be repositioned
		::SendMessage(hwndAdButton, WM_APPDIRECT_MANAGED_REBAR_UPDATED, wParamUpdate, lParamUpdate);

		return 0;	// this message is processed
	}

	return DefSubclassProc(hWnd, uMsg, wParam, lParam);
}

LRESULT CALLBACK SubclassTaskbarProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)
{
	const HWND hwndAdButton = ((HWND)dwRefData);	

	if (!::IsWindow(hwndAdButton)) {
		LRESULT lResult = DefSubclassProc(hWnd, uMsg, wParam, lParam);
		DoTearDownSubclass(TRUE, TRUE);
		return lResult;
	} else if (uMsg == WM_MOVE) {
		::SendMessage(hwndAdButton, WM_APPDIRECT_MANAGED_TASKBAR_UPDATED, NULL, NULL);
	} else if (uMsg == WM_WINDOWPOSCHANGED) {
		// place on top of task bar
		WINDOWPOS * p = (WINDOWPOS*)lParam;
		BOOL b = ::SetWindowPos(
				hwndAdButton, 
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
	return DefSubclassProc(hWnd, uMsg, wParam, lParam);
}

/**************/

BOOL DoSetupSubclass(HWND hwndAdButton) 
{
	// Register update message
	WM_APPDIRECT_NATIVE_UPDATE_OFFSET = ::RegisterWindowMessage(APPDIRECT_MESSAGE_NAME_NATIVE_UPDATE_OFFSET);
	WM_APPDIRECT_NATIVE_TERMINATE = ::RegisterWindowMessage(APPDIRECT_MESSAGE_NAME_NATIVE_TERMINATE);
	WM_APPDIRECT_MANAGED_REBAR_UPDATED = ::RegisterWindowMessage(APPDIRECT_MESSAGE_NAME_MANAGED_REBAR_UPDATED);
	WM_APPDIRECT_MANAGED_TASKBAR_UPDATED = ::RegisterWindowMessage(APPDIRECT_MESSAGE_NAME_MANAGED_TASKBAR_UPDATED);

	if (g_bIsLoaded) {
		return TRUE;
	}

	// Get current module name
	TCHAR libName[MAX_PATH]; 
	::GetModuleFileName(g_hModule, libName, MAX_PATH);

	// Increment reference counter on the current module
	// Which will prevent it from unloading when the hook is removed
	HMODULE hModule = ::LoadLibrary(libName); _ASSERT(hModule == g_hModule);
	if (hModule) {
		// Subclass relevant windows
		HWND taskBar = ::FindWindow(L"Shell_TrayWnd", NULL); _ASSERT(taskBar);
		BOOL bTaskBarHook = ::SetWindowSubclass(taskBar, SubclassTaskbarProc, 0, (DWORD_PTR)hwndAdButton); _ASSERT(bTaskBarHook);

		HWND reBar = ::FindWindowEx(taskBar, NULL, REBARCLASSNAME, NULL); _ASSERT(reBar);	
		BOOL bReBarHook = ::SetWindowSubclass(reBar, SubclassRebarProc, 0, (DWORD_PTR)hwndAdButton); _ASSERT(bReBarHook);

		g_bIsLoaded = TRUE;
		return TRUE;
	}

	return FALSE;
}

BOOL DoTearDownSubclass(BOOL bAsync, BOOL bResetReBar)
{
	if (!g_bIsLoaded) {
		return TRUE;
	}

	// Remove subclasses
	HWND hwndTaskBar = FindTaskBar(); _ASSERT(hwndTaskBar);
	BOOL bTaskBarHook = ::RemoveWindowSubclass(hwndTaskBar, SubclassTaskbarProc, 0); _ASSERT(bTaskBarHook);

	HWND hwndReBar = FindReBar(hwndTaskBar); _ASSERT(hwndReBar);	
	BOOL bReBarHook = ::RemoveWindowSubclass(hwndReBar, SubclassRebarProc, 0); _ASSERT(bReBarHook);

	if (bResetReBar && g_ReBarOffset != 0) {
		RECT rect;
		::GetWindowRect(hwndReBar, &rect);
		POINT tl;
		tl.x = rect.left;
		tl.y = rect.top;
		if (::ScreenToClient(hwndTaskBar, &tl)) {
			if (::IsVertical(hwndTaskBar)) {
				::MoveWindow(hwndReBar, tl.x, tl.y - g_ReBarOffset, rect.right - rect.left, rect.bottom - rect.top + g_ReBarOffset, TRUE);
			} else {
				::MoveWindow(hwndReBar, tl.x - g_ReBarOffset, tl.y, rect.right - rect.left + g_ReBarOffset, rect.bottom - rect.top, TRUE);
			}
		}
	}

	// Release the module
	if (bAsync) {
		// Asynchronously
		::CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)::FreeLibrary, g_hModule, 0, NULL);
		return TRUE;
	} else {
		// Synchronously
		if (::FreeLibrary(g_hModule)) {
			g_bIsLoaded = FALSE;
			return TRUE;
		}
	}

	return FALSE;
}

LRESULT CALLBACK HookProc(int code, WPARAM wParam, LPARAM lParam)
{
	if (code == HC_ACTION) {
		PCWPSTRUCT pCW = (PCWPSTRUCT)lParam;
		
		HHOOK hHook = (HHOOK)pCW->wParam;
		HWND hwndAdButton = (HWND)pCW->lParam;

		switch (pCW->message) {
		case WM_APPDIRECT_SETUP_SUBCLASS:
			::UnhookWindowsHookEx(hHook);
			return DoSetupSubclass(hwndAdButton);
		case WM_APPDIRECT_TEARDOWN_SUBCLASS: 
			::UnhookWindowsHookEx(hHook);
			return DoTearDownSubclass();
		case WM_APPDIRECT_IS_SUBCLASSED:
			return g_bIsLoaded;
		default:
			break;
		}
	}

	return ::CallNextHookEx(NULL, code, wParam, lParam);
}


BOOL SendMessageWithHook(UINT message, HWND hwndArg) 
{
	HWND shellTrayHwnd = ::FindWindow(L"Shell_TrayWnd", NULL);
	if (shellTrayHwnd == NULL) {
		return FALSE;
	}

	DWORD threadId = ::GetWindowThreadProcessId(shellTrayHwnd, NULL);
	HHOOK hHook = ::SetWindowsHookEx(WH_CALLWNDPROC, HookProc, g_hModule, threadId);

	if (!hHook) {
		return FALSE;
	}

	return (::SendMessage(shellTrayHwnd, message, (WPARAM)hHook, (LPARAM)hwndArg) != FALSE);
}

BOOL SetupSubclass(HWND hwndAdButton)
{
	return SendMessageWithHook(WM_APPDIRECT_SETUP_SUBCLASS, hwndAdButton);
}

BOOL TearDownSubclass()
{
	return SendMessageWithHook(WM_APPDIRECT_TEARDOWN_SUBCLASS, NULL);
}

BOOL IsSubclassed()
{
	return SendMessageWithHook(WM_APPDIRECT_IS_SUBCLASSED, NULL);
}