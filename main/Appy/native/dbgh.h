#pragma once

#define tt(format, v) { DWORD hw = (DWORD)(v); ATLTRACE2("Line:%d: %s = " #format "\n", __LINE__, #v, hw);}
#define tx(v) tt(0x%X, v)
#define td(v) tt(%d, v)

#define at(a) ATLTRACE2(a)

class hh
{
public:
	operator HWND () 
	{
		ATLTRACE2("Accessed: %X", _h);
		return _h;
	}
	HWND& operator=(HWND & v)
	{
		ATLTRACE2("set new HWND: %X", v);
		if (_h == v) 
			ATLTRACE2("THE SAME!");
		_h = v;
		return _h;
	}
	hh& operator=(hh & v)
	{
		ATLTRACE2("set new HH: %X", v._h);
		if (_h == v._h) 
			ATLTRACE2("THE SAME!");
		_h = v._h;
		return *this;
	}
private:
	HWND _h;
};

