#pragma once
#include "consts.h"
//-------------------------------------------------------------------------------------------------
//namespace adButton 
//{
//-------------------------------------------------------------------------------------------------
class CADButton: public CWindowImpl<CADButton>	// implementation of the button: outer win32 window
{
public:
	DECLARE_WND_CLASS_EX(APP_BASENAME L".CADButton", CS_DBLCLKS, COLOR_MENU)

	// message handlers
	BEGIN_MSG_MAP( CADButton )
		MESSAGE_HANDLER( WM_CREATE, OnCreate )
		MESSAGE_HANDLER( WM_DESTROY, OnDestroy )
		MESSAGE_HANDLER( WM_CLOSE, OnClose )
		MESSAGE_HANDLER( WM_MOUSEACTIVATE, OnMouseActivate )
		MESSAGE_HANDLER( WM_MOUSEMOVE, OnMouseMove )
		MESSAGE_HANDLER( WM_ERASEBKGND, OnEraseBkgnd )
		MESSAGE_HANDLER( WM_SETTINGCHANGE, OnSettingChange )
	END_MSG_MAP()
public:
	CADButton();

	void SetPressed(bool bPressed);
	void UpdateButton();
	void TaskBarMouseMove();

	SIZE GetSize() const;
	bool GetSmallIcons() const { return m_bSmallIcons; }

	HWND m_hWnd;

protected:
	LRESULT OnCreate(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled);
	LRESULT OnDestroy(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled);
	LRESULT OnClose(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled) { return 0; }
	LRESULT OnEraseBkgnd(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled) { return 1; }

	// TODO: -1 test & think out
	LRESULT OnMouseActivate(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled) { return MA_NOACTIVATE; }

	LRESULT OnMouseMove(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled);
	LRESULT OnSettingChange(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled);
	LRESULT OnThemeChanged(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled);

private:

// TODO: -1 RAII for all the resources
	SIZE m_Size;
// 	HBITMAP m_Bitmap;
//	unsigned int *m_Bits, *m_BlendBits;
//	HICON m_Icon;
//	HFONT m_Font;
	bool m_bPressed;
	bool m_bTrackMouse;
	bool m_bSmallIcons;
	
//	void LoadBitmap();
};

//extern NATIVE_API CADButton g_TheButton;		// the button	g_TheButton.m_nWnd its HWND

//-------------------------------------------------------------------------------------------------
//}	// end of namespace adButton 
//-------------------------------------------------------------------------------------------------