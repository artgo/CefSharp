// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

#define STRICT_TYPED_ITEMIDS
#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers


#ifndef _WIN32_WINNT_WIN8
#define _WIN32_WINNT_WIN8                   0x0602
#endif

// Windows Header Files:
#include <windows.h>
#include <commctrl.h>
#include <shlobj.h>
#include <shellapi.h>
#include <Dwmapi.h>
#include <crtdbg.h>


