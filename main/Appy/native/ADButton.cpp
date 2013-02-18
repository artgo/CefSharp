#include "stdafx.h"
#include "native.h"
#include "ADButton.h"

#include "dbgh.h"

//-------------------------------------------------------------------------------------------------
//namespace adButton 
//{
//-------------------------------------------------------------------------------------------------
static int START_ICON_SIZE = 16;
const int START_BUTTON_PADDING = 3;
const int START_BUTTON_OFFSET = 2;
const int START_TEXT_PADDING = 2;

//HWND g_ADButton = NULL;		g_TheButton.m_hWnd
CADButton g_TheButton;




CADButton::CADButton()
	: m_hWnd(NULL)
	//, m_Bitmap(NULL)
	//, m_Icon(NULL)
	//, m_Font(NULL)
	, m_bPressed(false)
	, m_bTrackMouse(false)
{}

LRESULT CADButton::OnCreate(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
	int params = (int)(((CREATESTRUCT*)lParam)->lpCreateParams);

	m_bSmallIcons = IsTaskbarSmallIcons();


	if (WinVersion() >= _WIN32_WINNT_VISTA)
	{
		unsigned int v = 1;
		::DwmSetWindowAttribute(m_hWnd, DWMWA_EXCLUDED_FROM_PEEK, &v, sizeof(v));		// since Vista
		v = DWMFLIP3D_EXCLUDEABOVE;
		::DwmSetWindowAttribute(m_hWnd, DWMWA_FLIP3D_POLICY, &v, sizeof(v));
	}

	//LoadBitmap();


	m_bPressed = true;
	SetPressed(false);
	bHandled = FALSE;

	return 0;
}

LRESULT CADButton::OnDestroy(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
	//if (m_Bitmap) DeleteObject(m_Bitmap);
	//if (m_Icon) DestroyIcon(m_Icon);
	//if (m_Font) DeleteObject(m_Font);

	bHandled = FALSE;
	return 0;
}

void CADButton::UpdateButton()
{
	//BLENDFUNCTION func = {AC_SRC_OVER, 0, 255, AC_SRC_ALPHA};

	// TODO: -1 move to WPF

	//HDC hSrc = CreateCompatibleDC(NULL);	// TODO: -1 RAII
	//RECT rc;
	//GetWindowRect(&rc);
	//SIZE size = {rc.right - rc.left, rc.bottom - rc.top };

	{
		//if (m_bPressed) 
		{
//			int image = 2;
//			HBITMAP bmp0 = (HBITMAP)SelectObject(hSrc, m_Bitmap);
//			POINT pos = {0, image * m_Size.cy };
//			UpdateLayeredWindow(m_hWnd, NULL, NULL, &size, hSrc, &pos, 0, &func, ULW_ALPHA);
//			SelectObject(hSrc, bmp0);
		}
	}

	//DeleteDC(hSrc);
}


LRESULT CADButton::OnMouseMove(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
	// TODO: -1 marshal to WPF
	return 0;
}

void CADButton::TaskBarMouseMove()
{
	// TODO: -1 marshal to WPF
}

// Windows settings change event
LRESULT CADButton::OnSettingChange(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
	UpdateButton();
	bHandled = FALSE;
	return 0;
}

LRESULT CADButton::OnThemeChanged(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
	// TODO: -1 marshal to WPF
	return 0;
}

void CADButton::SetPressed(bool bPressed)
{
	if (m_bPressed != bPressed)
	{
		m_bPressed = bPressed;
		UpdateButton();
	}
}

// return real size of native (outer window) obtained using system call
// does not work before window creation
SIZE CADButton::GetSize() const 
{
	//SIZE sz = {0, 0};
	SIZE sz;
	if (m_hWnd)
	{	
		RECT r;
		BOOL b = ::GetWindowRect(m_hWnd, &r);	_ASSERT(b);
		if (b)
		{
			sz.cx = r.right - r.left;
			sz.cy = r.bottom - r.top;
		}
		else
		{
			sz = GetInitialADButtonSize();
		}
	}
	else
	{
		sz = GetInitialADButtonSize();
	}
	return sz;
}

void CreateADButton(HWND taskBar, HWND rebar, const RECT & rcTaskBar, const UINT edge)
{
	if (g_TheButton.m_hWnd) 
		return;

//	DestroyADButton();

	SIZE szInitial = GetInitialADButtonSize();
	SIZE szStart = GetStartButtonSize();
	RECT rcButton;
	MONITORINFO info;
	if (edge == ABE_LEFT || edge == ABE_RIGHT)
	{
		rcButton.left = (rcTaskBar.left + rcTaskBar.right - szInitial.cx) /2;
		rcButton.top = rcTaskBar.top + szStart.cy;
	}
	else 
	{
		rcButton.left = rcTaskBar.left + szStart.cx;
		rcButton.top = (rcTaskBar.top + rcTaskBar.bottom - szInitial.cy) /2;
	}
	rcButton.right = rcButton.left + szInitial.cx;
	rcButton.bottom = rcButton.top + szInitial.cy;

	g_TheButton.m_hWnd = g_TheButton.Create(taskBar, NULL, NULL, 
		WS_POPUP, 
		0
//		| WS_EX_TOPMOST 
		|WS_EX_TOOLWINDOW 
		|WS_EX_LAYERED, 
		0U, (void*)(0) );	_ASSERT(g_TheButton.m_hWnd);
tx(g_TheButton.m_hWnd);
	if (!g_TheButton.m_hWnd)
	{
		DWORD err = GetLastError();
		ATLTRACE2("###### ERROR #################"); td(err); tx(err);
	}

	g_TheButton.SetParent(g_TaskBar);
	DWORD style = g_TheButton.GetWindowLong(GWL_STYLE);
	style &= ~(WS_POPUP);
	style |= WS_CHILD;
style = 0x5600b25d;	// rebar style win7
	g_TheButton.SetWindowLong(GWL_STYLE, style);

style = 0x80;	// ex style of rebar win7
	g_TheButton.SetWindowLong(GWL_EXSTYLE, style);
	
	RECT rc2 = rcButton;	// child window: relative coords
	POINT pp[2];
	RectToPoints(rc2, pp);
	::MapWindowPoints(NULL, g_TaskBar, pp, 2);
	PointsToRect(pp, rc2);
	
	g_TheButton.SetWindowPos(HWND_TOP, &rc2, SWP_SHOWWINDOW | SWP_NOOWNERZORDER | SWP_NOACTIVATE);
	//g_TheButton.SetWindowPos(HWND_TOP, &rcButton, SWP_SHOWWINDOW | SWP_NOOWNERZORDER | SWP_NOACTIVATE);

	// TODO: rebar move right
	RECT RebarOld;
	_ASSERT(g_ReBar);
	BOOL b = ::GetWindowRect(g_ReBar, &RebarOld);	_ASSERT(b);
	POINT ltTaskBar = GetTaskbarPos();
	POINT RebarNewTopLeft = { szStart.cx + szInitial.cx, RebarOld.top - ltTaskBar.y};
	SIZE RebarNewSize = { RebarOld.right - RebarOld.left - szInitial.cx, RebarOld.bottom - RebarOld.top };
	if (edge == ABE_LEFT || edge == ABE_RIGHT)
	{
		RebarNewTopLeft.x = RebarOld.left;
		uint buttonHeight = rc2.bottom - rc2.top;
		RebarNewTopLeft.y = RebarOld.top + buttonHeight;
		RebarNewSize.cx = RebarOld.right - RebarOld.left;
		RebarNewSize.cy = RebarOld.bottom - RebarOld.top - buttonHeight;
		::MapWindowPoints(NULL, g_TaskBar, &RebarNewTopLeft, 1);
	}
	b = ::SetWindowPos(g_ReBar, g_TheButton.m_hWnd, RebarNewTopLeft.x, RebarNewTopLeft.y, RebarNewSize.cx, RebarNewSize.cy, 0);	_ASSERT(b);

	RECT rc;
	IntersectRect(&rc, &rcButton, &info.rcMonitor);
//RECT tmp = {rc.left - rcButton.left, rc.top - rcButton.top, rc.right - rcButton.left, rc.bottom - rcButton.top};

	RECT tmp1 = {0, 0, szInitial.cx, szInitial.cy};

//	HRGN rgn = ::CreateRectRgn(rc.left - rcButton.left, rc.top - rcButton.top, rc.right - rcButton.left, rc.bottom - rcButton.top);
	HRGN rgn = ::CreateRectRgn(tmp1.left, tmp1.top, tmp1.right, tmp1.bottom);	_ASSERT(rgn);
	if (!::SetWindowRgn(g_TheButton.m_hWnd, rgn, FALSE))
		DeleteObject(rgn);

	g_TheButton.UpdateButton();
}

void DestroyADButton()
{
	if (g_TheButton.m_hWnd)
	{
		SIZE sz = g_TheButton.GetSize();
		
		// adjust rebar 
		HWND reb = FindRebar();		_ASSERT(reb);
		RECT r;
		BOOL b = ::GetWindowRect(reb, &r);	_ASSERT(b);
		
		UINT edge = GetTaskbarEdge();
		if (edge == ABE_LEFT || edge == ABE_RIGHT) r.top -= sz.cy;
		else r.left -= sz.cx;

		POINT p = {r.left, r.top};
		b = ScreenToClient(g_TaskBar, &p);	_ASSERT(b);

		b = g_TheButton.DestroyWindow();	_ASSERT(b);
		g_TheButton.m_hWnd = NULL;

		b = ::SetWindowPos(reb, NULL, 
			// r.left, r.top, 
			p.x, p.y,
			r.right - r.left, r.bottom - r.top,
			0);	_ASSERT(b);
	}
}

void UpdateADButton()
{
	g_TheButton.UpdateButton();
}

void PressADButton(bool bPressed )
{
	g_TheButton.SetPressed(bPressed);
}

SIZE GetADButtonSize()
{
	return g_TheButton.GetSize();
}

bool IsADButtonSmallIcons()		// TODO: -2 : not implemented
{
	if (g_TheButton.m_hWnd) { if (GetADButtonSize().cx < 64) {return true;}}
	return false;
}

bool IsTaskbarSmallIcons()
{
	CRegKey regKey;
	if (regKey.Open(HKEY_CURRENT_USER, L"Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced") != ERROR_SUCCESS)
		return true;
	DWORD v;
	return (regKey.QueryDWORDValue(L"TaskbarSmallIcons", v) != ERROR_SUCCESS) || v;		// old versions use small icons
}
//-------------------------------------------------------------------------------------------------
//}	// end of namespace adButton 
//-------------------------------------------------------------------------------------------------