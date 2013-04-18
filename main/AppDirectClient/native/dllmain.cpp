// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"
#include "native.h"

// http://msdn.microsoft.com/en-us/library/windows/desktop/bb773175(v=vs.85).aspx
#pragma comment(linker, \
	"\"/manifestdependency:type='Win32' "\
	"name='Microsoft.Windows.Common-Controls' "\
	"version='6.0.0.0' "\
	"processorArchitecture='*' "\
	"publicKeyToken='6595b64144ccf1df' "\
	"language='*'\"")



//-------------------------------------------------------------------------------------------------
//namespace adButton 
//{
//-------------------------------------------------------------------------------------------------
HINSTANCE g_Instance;

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	//switch (ul_reason_for_call)
	//{
	//case DLL_PROCESS_ATTACH:
	//case DLL_THREAD_ATTACH:
	//case DLL_THREAD_DETACH:
	//case DLL_PROCESS_DETACH:
	//	break;
	//}
	if (ul_reason_for_call == DLL_PROCESS_ATTACH)
	{
		g_Instance = hModule;
	}

	return TRUE;
}

//-------------------------------------------------------------------------------------------------
//}	// end of namespace adButton 
//-------------------------------------------------------------------------------------------------