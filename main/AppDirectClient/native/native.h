#pragma once

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

struct APPDIRECT_IPC_MESSAGES {
  UINT ExitMessage;
  UINT UpdateMessage;
  HWND AppDirectHwnd;
};

typedef struct APPDIRECT_IPC_MESSAGES APPDIRECT_IPC_MESSAGES;

// return HWND of taskbar window for the given process, NULL on error
//NATIVE_API HWND FindTaskBar(DWORD process);
HWND FindTaskBar(DWORD process);

// WH_GETMESSAGE hook for the explorer's GUI thread. The start menu exe uses this hook to inject code into the explorer process
NATIVE_API LRESULT CALLBACK SetupHooks(int code, WPARAM wParam, LPARAM lParam);
NATIVE_API LRESULT CALLBACK SetupHooks2(int code, WPARAM wParam, LPARAM lParam);		// just clone for debug&bugfix

extern NATIVE_API const wchar_t * gc_TheDllName;
extern HINSTANCE g_Instance;	// in dllmain.cpp

NATIVE_API WORD WinVersion();

extern "C" NATIVE_API DWORD GetRebarThread();
extern "C" NATIVE_API HWND FindTaskBar();
extern "C" NATIVE_API HWND FindRebar();
extern "C" NATIVE_API UINT GetExitMsg();
extern "C" NATIVE_API UINT GetUpdatePositionMsg();

// functions bellow performs system call each time
// it is intended, force system calls to get actual information
HWND GetProgmanHwnd();
DWORD GetExplorerProcess();
DWORD GetTaskbarThread();
DWORD ThreadFromWnd(HWND wnd);
SIZE GetTaskbarSize();
SIZE SizeOfRect(const RECT& r);
HWND FindRebar(HWND taskbar);

extern "C" NATIVE_API void InjectExplrorerExe();
extern "C" NATIVE_API void DetachHooks();
