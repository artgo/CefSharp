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

extern HMODULE g_hModule;

extern "C" NATIVE_API BOOL SetupSubclass(HWND adButtonHwnd);
extern "C" NATIVE_API BOOL TearDownSubclass();
extern "C" NATIVE_API BOOL IsSubclassed();

