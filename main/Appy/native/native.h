// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the NATIVE_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// NATIVE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef NATIVE_EXPORTS
#define NATIVE_API __declspec(dllexport)
#else
#define NATIVE_API __declspec(dllimport)
#endif


#include "consts.h"

//// This class is exported from the native.dll
//class NATIVE_API Cnative {
//public:
//	Cnative();
//	// TODO: add your methods here.
//};
//
//extern NATIVE_API int nnative;
//
//NATIVE_API int fnnative();


//-------------------------------------------------------------------------------------------------
//namespace adButton 
//{
//-------------------------------------------------------------------------------------------------
// return HWND of taskbar window for the given process, NULL on error
//NATIVE_API HWND FindTaskBar(DWORD process);
HWND FindTaskBar(DWORD process);

// WH_GETMESSAGE hook for the explorer's GUI thread. The start menu exe uses this hook to inject code into the explorer process
NATIVE_API LRESULT CALLBACK SetupHooks(int code, WPARAM wParam, LPARAM lParam);
NATIVE_API LRESULT CALLBACK SetupHooks2(int code, WPARAM wParam, LPARAM lParam);		// just clone for debug&bugfix

extern NATIVE_API const wchar_t * gc_TheDllName;
//extern NATIVE_API HWND g_ReBar;		// of the taskbar

//extern NATIVE_API CADButton g_TheButton;		// the button	g_TheButton.m_nWnd its HWND


//-------------------------------------------------------------------------------------------------
//	ADButton.cpp:
void CreateADButton(HWND taskBar, HWND rebar, const RECT &rcTask, const UINT edge);
//NATIVE_API void DestroyADButton();
void UpdateADButton();
void PressADButton(bool bPressed);
NATIVE_API SIZE GetADButtonSize();

bool IsADButtonSmallIcons();
//-------------------------------------------------------------------------------------------------





extern HINSTANCE g_Instance;	// in dllmain.cpp

UINT GetTaskbarEdge(HWND taskBar, MONITORINFO *pInfo, HMONITOR *pMonitor, RECT * TaskbarRect);

NATIVE_API WORD WinVersion();

NATIVE_API void RecreateADButton();	// TODO: -0 hide
NATIVE_API void DestroyADButton();
NATIVE_API void ReDestroyADButton();
extern "C" NATIVE_API DWORD GetRebarThread();
extern "C" NATIVE_API HWND FindTaskBar();
extern "C" NATIVE_API HWND FindRebar();

bool PointAroundADButton();


// functions bellow performs system call each time
// it is intended, force system calls to get actual information
HWND GetProgmanHwnd();
DWORD GetExplorerProcess();
DWORD GetTaskbarThread();
//DWORD GetRebarThread();
DWORD ThreadFromWnd(HWND wnd);
//HWND FindTaskBar();
SIZE GetTaskbarSize();

//HWND FindRebar();
HWND FindRebar(HWND taskbar);

NATIVE_API HWND FindStartButton();

extern "C" NATIVE_API SIZE GetInitialADButtonSize();
extern "C" NATIVE_API SIZE GetStartButtonSize();
extern "C" NATIVE_API POINT GetTaskbarPos();
extern "C" NATIVE_API void InjectExplrorerExe();
extern "C" NATIVE_API void DetachHooks();
extern "C" NATIVE_API UINT GetTaskbarEdge();


SIZE SizeOfRect(const RECT& r);


void RectToPoints(const RECT& r, POINT * pp);
void PointsToRect(const POINT * pp, RECT& r);
//-------------------------------------------------------------------------------------------------
//}	// end of namespace adButton 

//-------------------------------------------------------------------------------------------------