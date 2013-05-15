#include "stdafx.h"
#include "native.h"

#define WM_APPDIRECT_SETUP_SUBCLASS WM_USER + 1
#define WM_APPDIRECT_TEARDOWN_SUBCLASS WM_USER + 2

// The following message is registered with ::RegisterWindowsMessage
// because it's used across application
UINT WM_APPDIRECT_UPDATE = 0;


BOOL g_bIsLoaded = FALSE;
HMODULE g_hModule = NULL;
RECT g_RectButtons;

void setModuleHandle(HMODULE hModule)
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

static bool IsVertical()
{
	HWND taskBar = FindTaskBar();
	APPBARDATA appbar = {sizeof(appbar), taskBar};
    SHAppBarMessage(ABM_GETTASKBARPOS, &appbar);
	return (appbar.uEdge == ABE_LEFT) || (appbar.uEdge == ABE_RIGHT);
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

static LRESULT CALLBACK SubclassRebarProc(const HWND hWnd, const UINT uMsg, const WPARAM wParam, const LPARAM lParam, 
										  const UINT_PTR uIdSubclass, const DWORD_PTR dwRefData)
{
	HWND hwndAdButton = (HWND)dwRefData;

	if (::IsWindow(hwndAdButton)) {
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

static LRESULT CALLBACK SubclassTaskbarProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)
{
	const HWND hwndAdButton = ((HWND)dwRefData);

	if (::IsWindow(hwndAdButton)) {
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

LRESULT CALLBACK HookProc(int code, WPARAM wParam, LPARAM lParam)
{
	if (code == HC_ACTION) {
		PCWPSTRUCT pCW = (PCWPSTRUCT)lParam;
		
		HHOOK hHook = (HHOOK)pCW->wParam;
		HWND adButtonHwnd = (HWND)pCW->lParam;

		switch (pCW->message) {
		case WM_APPDIRECT_SETUP_SUBCLASS:
			{
				::UnhookWindowsHookEx(hHook);

				// Register update message
				WM_APPDIRECT_UPDATE = GetUpdatePositionMsg();

				if (!g_bIsLoaded) {
					// Get current module name
					TCHAR libName[MAX_PATH]; 
					::GetModuleFileName(g_hModule, libName, MAX_PATH);

					// Increment reference counter on the current module
					// Which will prevent it from unloading when the hook is removed
					if (::LoadLibrary(libName)) {
						// Subclass relevant windows
						HWND taskBar = ::FindWindow(L"Shell_TrayWnd", NULL); _ASSERT(taskBar);
						BOOL bTaskBarHook = ::SetWindowSubclass(taskBar, SubclassTaskbarProc, 0, (DWORD_PTR)adButtonHwnd); _ASSERT(bTaskBarHook);

						HWND reBar = ::FindWindowEx(taskBar, NULL, REBARCLASSNAME, NULL); _ASSERT(reBar);	
						BOOL bReBarHook = ::SetWindowSubclass(reBar, SubclassRebarProc, 0, (DWORD_PTR)adButtonHwnd); _ASSERT(bReBarHook);

						g_bIsLoaded = TRUE;
						return TRUE;
					}
				}
				break;
			}
		case WM_APPDIRECT_TEARDOWN_SUBCLASS: 
			{
				::UnhookWindowsHookEx(hHook);

				if (g_bIsLoaded) {
					// Remove subclasses
					HWND hwndTaskBar = FindTaskBar(); _ASSERT(hwndTaskBar);
					BOOL bTaskBarHook = ::RemoveWindowSubclass(hwndTaskBar, SubclassTaskbarProc, 0); _ASSERT(bTaskBarHook);

					HWND hwndReBar = FindReBar(hwndTaskBar); _ASSERT(hwndReBar);	
					BOOL bReBarHook = ::RemoveWindowSubclass(hwndReBar, SubclassRebarProc, 0); _ASSERT(bReBarHook);

					// Release the module
					if (::FreeLibrary(g_hModule)) {
						g_bIsLoaded = FALSE;
						return TRUE;
					}
				}
				break;
			}
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
	HHOOK hHook = SetWindowsHookEx(WH_CALLWNDPROC, HookProc, g_hModule, threadId);

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