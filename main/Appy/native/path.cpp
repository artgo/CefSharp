#include "stdafx.h"
#include "path.h"


CString GetExePath()
{
	CString p;
	wchar_t * path = p.GetBuffer(_MAX_PATH);
	::GetModuleFileName(NULL, path, _MAX_PATH);
	*(::PathFindFileName(path)) = L'\0';
	p.ReleaseBuffer();
	return p;
}

// set current directory to this module's path
void ChangeDirToHere()
{
	::SetCurrentDirectory(GetExePath());
}
