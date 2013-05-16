#include "stdafx.h"
#include "native.h"

// The following messages are registered with ::RegisterWindowsMessage
// because they are used across application
#define APPDIRECT_MESSAGE_NAME_UPDATE L"AppDirectButtonPositionUpdateMessage"
#define APPDIRECT_MESSAGE_NAME_NATIVE_TERMINATE L"AppDirectNativeTerminateMessage"
UINT WM_APPDIRECT_UPDATE = 0;
UINT WM_APPDIRECT_NATIVE_TERMINATE = 0;

#define WM_APPDIRECT_SETUP_SUBCLASS WM_USER + 1
#define WM_APPDIRECT_TEARDOWN_SUBCLASS WM_USER + 2


BOOL g_bIsLoaded = FALSE;
HMODULE g_hModule = NULL;
RECT g_RectButtons;

BOOL DoSetupSubclass();
BOOL DoTearDownSubclass(BOOL bAsync = FALSE);
HWND FindTaskBar();
HWND FindReBar(HWND hwndTaskBar);
BOOL IsVertical();

void SetModuleHandle(HMODULE hModule)
{
	g_hModule = hModule;
}

HWND FindTaskBar()
{
	return ::FindWindow(L"Shell_TrayWnd", NULL);
}

HWND FindReBar(HWND hwndTaskBar)
{
	return ::FindWindowEx(hwndTaskBar, NULL, REBARCLASSNAME, NULL);
}

BOOL IsVertical()
{
	HWND taskBar = FindTaskBar();
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
		DoTearDownSubclass(TRUE);
		return lResult;
	} else {
		if (uMsg == WM_WINDOWPOSCHANGING)
		{
			WINDOWPOS* p = (WINDOWPOS*)lParam;

			// Prevent Rebar from restoring its position if the AppDirect button is properly positioned
			if (g_RectButtons.left <= (p->x + p->cx) && g_RectButtons.right  >= p->x &&
				g_RectButtons.top  <= (p->y + p->cy) && g_RectButtons.bottom >= p->y) {
				if (IsVertical()) {
					const int deltaY = g_RectButtons.bottom - p->y;
					if (deltaY > 0) {
						p->y += deltaY;
						p->cy -= deltaY;
					}
				} else {
					const int deltaX = g_RectButtons.right - p->x;
					if (deltaX > 0) {
						p->x += deltaX;
						p->cx -= deltaX;
					}
				}
			}

			// Notify the application that the button should be repositioned
			::PostMessage(hwndAdButton, WM_APPDIRECT_UPDATE, FALSE, NULL);

			return 0;	// this message is processed
		} else if (uMsg == WM_APPDIRECT_UPDATE) {
			g_RectButtons.left = HIWORD(wParam);
			g_RectButtons.top = LOWORD(wParam);
			g_RectButtons.right = HIWORD(lParam);
			g_RectButtons.bottom = LOWORD(lParam);

			return 0;	// this message is processed
		}
	}

	return DefSubclassProc(hWnd, uMsg, wParam, lParam);
}

LRESULT CALLBACK SubclassTaskbarProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)
{
	const HWND hwndAdButton = ((HWND)dwRefData);

	if (!::IsWindow(hwndAdButton)) {
		LRESULT lResult = DefSubclassProc(hWnd, uMsg, wParam, lParam);
		DoTearDownSubclass(TRUE);
		return lResult;
	} else if (uMsg == WM_APPDIRECT_NATIVE_TERMINATE) {
		return DoTearDownSubclass(TRUE);
	} else {
		switch (uMsg) {
		case WM_WINDOWPOSCHANGED: 
			{
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
				break;
			}
		case WM_MOVE:
			// Notify the application that the button should be repositioned
			::SendMessage(hwndAdButton, WM_APPDIRECT_UPDATE, TRUE, NULL);
			break;
		default:
			break;
		}
	}
	return DefSubclassProc(hWnd, uMsg, wParam, lParam);
}

/**************/

BOOL DoSetupSubclass(HWND hwndAdButton) 
{
	// Register update message
	WM_APPDIRECT_UPDATE = ::RegisterWindowMessage(APPDIRECT_MESSAGE_NAME_UPDATE);
	WM_APPDIRECT_NATIVE_TERMINATE = ::RegisterWindowMessage(APPDIRECT_MESSAGE_NAME_NATIVE_TERMINATE);

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

BOOL DoTearDownSubclass(BOOL bAsync)
{
	if (!g_bIsLoaded) {
		return TRUE;
	}

	// Remove subclasses
	HWND hwndTaskBar = FindTaskBar(); _ASSERT(hwndTaskBar);
	BOOL bTaskBarHook = ::RemoveWindowSubclass(hwndTaskBar, SubclassTaskbarProc, 0); _ASSERT(bTaskBarHook);

	HWND hwndReBar = FindReBar(hwndTaskBar); _ASSERT(hwndReBar);	
	BOOL bReBarHook = ::RemoveWindowSubclass(hwndReBar, SubclassRebarProc, 0); _ASSERT(bReBarHook);

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