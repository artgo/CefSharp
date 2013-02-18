﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    public struct COLORREF
    {
        public uint _ColorDWORD;

        //Those goofs at MS store a COLORREF as XXBBGGRR and a Color as AARRGGBB
        //I'm sure that there's a good reason for it but it sure is a bummer
        //Lets do some bit shifting
        public COLORREF(Color color)
        {
            _ColorDWORD = ((uint)color.R) +
                          (uint)(color.G << 8) +
                          (uint)(color.B << 16);
        }

        public Color GetColor()
        {
            return Color.FromArgb((int)(0x000000FFU | _ColorDWORD),
                                  (int)((0x0000FF00 | _ColorDWORD) >> 2),
                                  (int)((0x00FF0000 | _ColorDWORD) >> 4));
        }

        public void SetColor(Color color)
        {
            _ColorDWORD = ((uint)color.R) +
                          (uint)(color.G << 8) +
                          (uint)(color.B << 16);
        }

        public override string ToString()
        {
            return _ColorDWORD.ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct COLORSCHEME
    {
        public uint dwSize;
        public COLORREF clrBtnHighlight;
        public COLORREF clrBtnShadow;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INITCOMMONCONTROLSEX
    {
        public uint dwSize; // size of this structure
        public uint dwICC; // flags indicating which classes to be initialized
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public MARGINS(int Left, int Right, int Top, int Bottom)
        {
            cxLeftWidth = Left;
            cxRightWidth = Right;
            cyTopHeight = Top;
            cyBottomHeight = Bottom;
        }

        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NMHDR
    {
        public IntPtr hwndFrom;
        public uint idFrom;
        public int code;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NMMOUSE
    {
        public NMHDR hdr;
        public IntPtr dwItemSpec;
        public IntPtr dwItemData;
        public POINT pt;
        public IntPtr dwHitInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NMREBARCHILDSIZE
    {
        public NMHDR hdr;
        public uint uBand;
        public uint wID;
        public RECT rcChild;
        public RECT rcBand;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct REBARBANDINFO
    {
        public void init()
        {
            cbSize = (uint)Marshal.SizeOf(typeof(REBARBANDINFO));
            fMask = 0U;
            fStyle = 0U;
            clrFore = new COLORREF(System.Drawing.SystemColors.ControlText);
            clrBack = new COLORREF(System.Drawing.SystemColors.Control);
            lpText = "";
            cch = 0U;
            iImage = 0;
            hwndChild = IntPtr.Zero;
            cxMinChild = 0U;
            cyMinChild = 0U;
            cx = 0U;
            hbmBack = IntPtr.Zero;
            wID = 0U;
            cyChild = 0U; //Initial Child Height
            cyMaxChild = 0U;
            cyIntegral = 0U;
            cxIdeal = 0U;
            lParam = IntPtr.Zero;
            cxHeader = 0U;
        }

        public uint cbSize;
        public uint fMask;
        public uint fStyle;
        public COLORREF clrFore;
        public COLORREF clrBack;
        public string lpText;
        public uint cch; //Size of text buffer
        public int iImage;
        public IntPtr hwndChild;
        public uint cxMinChild;
        public uint cyMinChild;
        public uint cx;
        public IntPtr hbmBack;
        public uint wID;
        public uint cyChild;
        public uint cyMaxChild;
        public uint cyIntegral;
        public uint cxIdeal;
        public IntPtr lParam;
        public uint cxHeader;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct REBARINFO
    {
        public uint cbSize;
        public uint fMask;
        public IntPtr himl;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TRACKMOUSEEVENT
    {
        public int cbSize;
        public uint dwFlags;
        public IntPtr hwndTrack;
        public int dwHoverTime;
    }

    public enum WindowsMessages : int
    {
        WM_NULL = 0x0000,
        WM_CREATE = 0x0001,
        WM_DESTROY = 0x0002,
        WM_MOVE = 0x0003,
        WM_SIZE = 0x0005,
        WM_ACTIVATE = 0x0006,
        WM_SETFOCUS = 0x0007,
        WM_KILLFOCUS = 0x0008,
        WM_ENABLE = 0x000A,
        WM_SETREDRAW = 0x000B,
        WM_SETTEXT = 0x000C,
        WM_GETTEXT = 0x000D,
        WM_GETTEXTLENGTH = 0x000E,
        WM_PAINT = 0x000F,
        WM_CLOSE = 0x0010,
        WM_QUERYENDSESSION = 0x0011,
        WM_QUERYOPEN = 0x0013,
        WM_ENDSESSION = 0x0016,
        WM_QUIT = 0x0012,
        WM_ERASEBKGND = 0x0014,
        WM_SYSCOLORCHANGE = 0x0015,
        WM_SHOWWINDOW = 0x0018,
        WM_WININICHANGE = 0x001A,
        WM_SETTINGCHANGE = 0x001A,
        WM_DEVMODECHANGE = 0x001B,
        WM_ACTIVATEAPP = 0x001C,
        WM_FONTCHANGE = 0x001D,
        WM_TIMECHANGE = 0x001E,
        WM_CANCELMODE = 0x001F,
        WM_SETCURSOR = 0x0020,
        WM_MOUSEACTIVATE = 0x0021,
        WM_CHILDACTIVATE = 0x0022,
        WM_QUEUESYNC = 0x0023,
        WM_GETMINMAXINFO = 0x0024,
        WM_PAINTICON = 0x0026,
        WM_ICONERASEBKGND = 0x0027,
        WM_NEXTDLGCTL = 0x0028,
        WM_SPOOLERSTATUS = 0x002A,
        WM_DRAWITEM = 0x002B,
        WM_MEASUREITEM = 0x002C,
        WM_DELETEITEM = 0x002D,
        WM_VKEYTOITEM = 0x002E,
        WM_CHARTOITEM = 0x002F,
        WM_SETFONT = 0x0030,
        WM_GETFONT = 0x0031,
        WM_SETHOTKEY = 0x0032,
        WM_GETHOTKEY = 0x0033,
        WM_QUERYDRAGICON = 0x0037,
        WM_COMPAREITEM = 0x0039,
        WM_GETOBJECT = 0x003D,
        WM_COMPACTING = 0x0041,
        WM_COMMNOTIFY = 0x0044,
        WM_WINDOWPOSCHANGING = 0x0046,
        WM_WINDOWPOSCHANGED = 0x0047,
        WM_POWER = 0x0048,
        WM_COPYDATA = 0x004A,
        WM_CANCELJOURNAL = 0x004B,
        WM_NOTIFY = 0x004E,
        WM_INPUTLANGCHANGEREQUEST = 0x0050,
        WM_INPUTLANGCHANGE = 0x0051,
        WM_TCARD = 0x0052,
        WM_HELP = 0x0053,
        WM_USERCHANGED = 0x0054,
        WM_NOTIFYFORMAT = 0x0055,
        WM_CONTEXTMENU = 0x007B,
        WM_STYLECHANGING = 0x007C,
        WM_STYLECHANGED = 0x007D,
        WM_DISPLAYCHANGE = 0x007E,
        WM_GETICON = 0x007F,
        WM_SETICON = 0x0080,
        WM_NCCREATE = 0x0081,
        WM_NCDESTROY = 0x0082,
        WM_NCCALCSIZE = 0x0083,
        WM_NCHITTEST = 0x0084,
        WM_NCPAINT = 0x0085,
        WM_NCACTIVATE = 0x0086,
        WM_GETDLGCODE = 0x0087,
        WM_SYNCPAINT = 0x0088,
        WM_NCMOUSEMOVE = 0x00A0,
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCLBUTTONUP = 0x00A2,
        WM_NCLBUTTONDBLCLK = 0x00A3,
        WM_NCRBUTTONDOWN = 0x00A4,
        WM_NCRBUTTONUP = 0x00A5,
        WM_NCRBUTTONDBLCLK = 0x00A6,
        WM_NCMBUTTONDOWN = 0x00A7,
        WM_NCMBUTTONUP = 0x00A8,
        WM_NCMBUTTONDBLCLK = 0x00A9,
        WM_NCXBUTTONDOWN = 0x00AB,
        WM_NCXBUTTONUP = 0x00AC,
        WM_NCXBUTTONDBLCLK = 0x00AD,
        WM_INPUT = 0x00FF,
        WM_KEYFIRST = 0x0100,
        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x0101,
        WM_CHAR = 0x0102,
        WM_DEADCHAR = 0x0103,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105,
        WM_SYSCHAR = 0x0106,
        WM_SYSDEADCHAR = 0x0107,
        WM_UNICHAR = 0x0109,
        WM_KEYLAST_NT501 = 0x0109,
        UNICODE_NOCHAR = 0xFFFF,
        WM_KEYLAST_PRE501 = 0x0108,
        WM_IME_STARTCOMPOSITION = 0x010D,
        WM_IME_ENDCOMPOSITION = 0x010E,
        WM_IME_COMPOSITION = 0x010F,
        WM_IME_KEYLAST = 0x010F,
        WM_INITDIALOG = 0x0110,
        WM_COMMAND = 0x0111,
        WM_SYSCOMMAND = 0x0112,
        WM_TIMER = 0x0113,
        WM_HSCROLL = 0x0114,
        WM_VSCROLL = 0x0115,
        WM_INITMENU = 0x0116,
        WM_INITMENUPOPUP = 0x0117,
        WM_MENUSELECT = 0x011F,
        WM_MENUCHAR = 0x0120,
        WM_ENTERIDLE = 0x0121,
        WM_MENURBUTTONUP = 0x0122,
        WM_MENUDRAG = 0x0123,
        WM_MENUGETOBJECT = 0x0124,
        WM_UNINITMENUPOPUP = 0x0125,
        WM_MENUCOMMAND = 0x0126,
        WM_CHANGEUISTATE = 0x0127,
        WM_UPDATEUISTATE = 0x0128,
        WM_QUERYUISTATE = 0x0129,
        WM_CTLCOLORMSGBOX = 0x0132,
        WM_CTLCOLOREDIT = 0x0133,
        WM_CTLCOLORLISTBOX = 0x0134,
        WM_CTLCOLORBTN = 0x0135,
        WM_CTLCOLORDLG = 0x0136,
        WM_CTLCOLORSCROLLBAR = 0x0137,
        WM_CTLCOLORSTATIC = 0x0138,
        WM_MOUSEFIRST = 0x0200,
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,
        WM_MOUSEWHEEL = 0x020A,
        WM_XBUTTONDOWN = 0x020B,
        WM_XBUTTONUP = 0x020C,
        WM_XBUTTONDBLCLK = 0x020D,
        WM_MOUSELAST_5 = 0x020D,
        WM_MOUSELAST_4 = 0x020A,
        WM_MOUSELAST_PRE_4 = 0x0209,
        WM_PARENTNOTIFY = 0x0210,
        WM_ENTERMENULOOP = 0x0211,
        WM_EXITMENULOOP = 0x0212,
        WM_NEXTMENU = 0x0213,
        WM_SIZING = 0x0214,
        WM_CAPTURECHANGED = 0x0215,
        WM_MOVING = 0x0216,
        WM_POWERBROADCAST = 0x0218,
        WM_DEVICECHANGE = 0x0219,
        WM_MDICREATE = 0x0220,
        WM_MDIDESTROY = 0x0221,
        WM_MDIACTIVATE = 0x0222,
        WM_MDIRESTORE = 0x0223,
        WM_MDINEXT = 0x0224,
        WM_MDIMAXIMIZE = 0x0225,
        WM_MDITILE = 0x0226,
        WM_MDICASCADE = 0x0227,
        WM_MDIICONARRANGE = 0x0228,
        WM_MDIGETACTIVE = 0x0229,
        WM_MDISETMENU = 0x0230,
        WM_ENTERSIZEMOVE = 0x0231,
        WM_EXITSIZEMOVE = 0x0232,
        WM_DROPFILES = 0x0233,
        WM_MDIREFRESHMENU = 0x0234,
        WM_IME_SETCONTEXT = 0x0281,
        WM_IME_NOTIFY = 0x0282,
        WM_IME_CONTROL = 0x0283,
        WM_IME_COMPOSITIONFULL = 0x0284,
        WM_IME_SELECT = 0x0285,
        WM_IME_CHAR = 0x0286,
        WM_IME_REQUEST = 0x0288,
        WM_IME_KEYDOWN = 0x0290,
        WM_IME_KEYUP = 0x0291,
        WM_MOUSEHOVER = 0x02A1,
        WM_MOUSELEAVE = 0x02A3,
        WM_NCMOUSEHOVER = 0x02A0,
        WM_NCMOUSELEAVE = 0x02A2,
        WM_WTSSESSION_CHANGE = 0x02B1,
        WM_TABLET_FIRST = 0x02c0,
        WM_TABLET_LAST = 0x02df,
        WM_CUT = 0x0300,
        WM_COPY = 0x0301,
        WM_PASTE = 0x0302,
        WM_CLEAR = 0x0303,
        WM_UNDO = 0x0304,
        WM_RENDERFORMAT = 0x0305,
        WM_RENDERALLFORMATS = 0x0306,
        WM_DESTROYCLIPBOARD = 0x0307,
        WM_DRAWCLIPBOARD = 0x0308,
        WM_PAINTCLIPBOARD = 0x0309,
        WM_VSCROLLCLIPBOARD = 0x030A,
        WM_SIZECLIPBOARD = 0x030B,
        WM_ASKCBFORMATNAME = 0x030C,
        WM_CHANGECBCHAIN = 0x030D,
        WM_HSCROLLCLIPBOARD = 0x030E,
        WM_QUERYNEWPALETTE = 0x030F,
        WM_PALETTEISCHANGING = 0x0310,
        WM_PALETTECHANGED = 0x0311,
        WM_HOTKEY = 0x0312,
        WM_PRINT = 0x0317,
        WM_PRINTCLIENT = 0x0318,
        WM_APPCOMMAND = 0x0319,
        WM_THEMECHANGED = 0x031A,
        WM_HANDHELDFIRST = 0x0358,
        WM_HANDHELDLAST = 0x035F,
        WM_AFXFIRST = 0x0360,
        WM_AFXLAST = 0x037F,
        WM_PENWINFIRST = 0x0380,
        WM_PENWINLAST = 0x038F,
        WM_APP = 0x8000,
        WM_USER = 0x0400,
        EM_GETSEL = 0x00B0,
        EM_SETSEL = 0x00B1,
        EM_GETRECT = 0x00B2,
        EM_SETRECT = 0x00B3,
        EM_SETRECTNP = 0x00B4,
        EM_SCROLL = 0x00B5,
        EM_LINESCROLL = 0x00B6,
        EM_SCROLLCARET = 0x00B7,
        EM_GETMODIFY = 0x00B8,
        EM_SETMODIFY = 0x00B9,
        EM_GETLINECOUNT = 0x00BA,
        EM_LINEINDEX = 0x00BB,
        EM_SETHANDLE = 0x00BC,
        EM_GETHANDLE = 0x00BD,
        EM_GETTHUMB = 0x00BE,
        EM_LINELENGTH = 0x00C1,
        EM_REPLACESEL = 0x00C2,
        EM_GETLINE = 0x00C4,
        EM_LIMITTEXT = 0x00C5,
        EM_CANUNDO = 0x00C6,
        EM_UNDO = 0x00C7,
        EM_FMTLINES = 0x00C8,
        EM_LINEFROMCHAR = 0x00C9,
        EM_SETTABSTOPS = 0x00CB,
        EM_SETPASSWORDCHAR = 0x00CC,
        EM_EMPTYUNDOBUFFER = 0x00CD,
        EM_GETFIRSTVISIBLELINE = 0x00CE,
        EM_SETREADONLY = 0x00CF,
        EM_SETWORDBREAKPROC = 0x00D0,
        EM_GETWORDBREAKPROC = 0x00D1,
        EM_GETPASSWORDCHAR = 0x00D2,
        EM_SETMARGINS = 0x00D3,
        EM_GETMARGINS = 0x00D4,
        EM_SETLIMITTEXT = EM_LIMITTEXT,
        EM_GETLIMITTEXT = 0x00D5,
        EM_POSFROMCHAR = 0x00D6,
        EM_CHARFROMPOS = 0x00D7,
        EM_SETIMESTATUS = 0x00D8,
        EM_GETIMESTATUS = 0x00D9,
        BM_GETCHECK = 0x00F0,
        BM_SETCHECK = 0x00F1,
        BM_GETSTATE = 0x00F2,
        BM_SETSTATE = 0x00F3,
        BM_SETSTYLE = 0x00F4,
        BM_CLICK = 0x00F5,
        BM_GETIMAGE = 0x00F6,
        BM_SETIMAGE = 0x00F7,
        STM_SETICON = 0x0170,
        STM_GETICON = 0x0171,
        STM_SETIMAGE = 0x0172,
        STM_GETIMAGE = 0x0173,
        STM_MSGMAX = 0x0174,
        DM_GETDEFID = (WM_USER + 0),
        DM_SETDEFID = (WM_USER + 1),
        DM_REPOSITION = (WM_USER + 2),
        LB_ADDSTRING = 0x0180,
        LB_INSERTSTRING = 0x0181,
        LB_DELETESTRING = 0x0182,
        LB_SELITEMRANGEEX = 0x0183,
        LB_RESETCONTENT = 0x0184,
        LB_SETSEL = 0x0185,
        LB_SETCURSEL = 0x0186,
        LB_GETSEL = 0x0187,
        LB_GETCURSEL = 0x0188,
        LB_GETTEXT = 0x0189,
        LB_GETTEXTLEN = 0x018A,
        LB_GETCOUNT = 0x018B,
        LB_SELECTSTRING = 0x018C,
        LB_DIR = 0x018D,
        LB_GETTOPINDEX = 0x018E,
        LB_FINDSTRING = 0x018F,
        LB_GETSELCOUNT = 0x0190,
        LB_GETSELITEMS = 0x0191,
        LB_SETTABSTOPS = 0x0192,
        LB_GETHORIZONTALEXTENT = 0x0193,
        LB_SETHORIZONTALEXTENT = 0x0194,
        LB_SETCOLUMNWIDTH = 0x0195,
        LB_ADDFILE = 0x0196,
        LB_SETTOPINDEX = 0x0197,
        LB_GETITEMRECT = 0x0198,
        LB_GETITEMDATA = 0x0199,
        LB_SETITEMDATA = 0x019A,
        LB_SELITEMRANGE = 0x019B,
        LB_SETANCHORINDEX = 0x019C,
        LB_GETANCHORINDEX = 0x019D,
        LB_SETCARETINDEX = 0x019E,
        LB_GETCARETINDEX = 0x019F,
        LB_SETITEMHEIGHT = 0x01A0,
        LB_GETITEMHEIGHT = 0x01A1,
        LB_FINDSTRINGEXACT = 0x01A2,
        LB_SETLOCALE = 0x01A5,
        LB_GETLOCALE = 0x01A6,
        LB_SETCOUNT = 0x01A7,
        LB_INITSTORAGE = 0x01A8,
        LB_ITEMFROMPOINT = 0x01A9,
        LB_MULTIPLEADDSTRING = 0x01B1,
        LB_GETLISTBOXINFO = 0x01B2,
        LB_MSGMAX_501 = 0x01B3,
        LB_MSGMAX_WCE4 = 0x01B1,
        LB_MSGMAX_4 = 0x01B0,
        LB_MSGMAX_PRE4 = 0x01A8,
        CB_GETEDITSEL = 0x0140,
        CB_LIMITTEXT = 0x0141,
        CB_SETEDITSEL = 0x0142,
        CB_ADDSTRING = 0x0143,
        CB_DELETESTRING = 0x0144,
        CB_DIR = 0x0145,
        CB_GETCOUNT = 0x0146,
        CB_GETCURSEL = 0x0147,
        CB_GETLBTEXT = 0x0148,
        CB_GETLBTEXTLEN = 0x0149,
        CB_INSERTSTRING = 0x014A,
        CB_RESETCONTENT = 0x014B,
        CB_FINDSTRING = 0x014C,
        CB_SELECTSTRING = 0x014D,
        CB_SETCURSEL = 0x014E,
        CB_SHOWDROPDOWN = 0x014F,
        CB_GETITEMDATA = 0x0150,
        CB_SETITEMDATA = 0x0151,
        CB_GETDROPPEDCONTROLRECT = 0x0152,
        CB_SETITEMHEIGHT = 0x0153,
        CB_GETITEMHEIGHT = 0x0154,
        CB_SETEXTENDEDUI = 0x0155,
        CB_GETEXTENDEDUI = 0x0156,
        CB_GETDROPPEDSTATE = 0x0157,
        CB_FINDSTRINGEXACT = 0x0158,
        CB_SETLOCALE = 0x0159,
        CB_GETLOCALE = 0x015A,
        CB_GETTOPINDEX = 0x015B,
        CB_SETTOPINDEX = 0x015C,
        CB_GETHORIZONTALEXTENT = 0x015d,
        CB_SETHORIZONTALEXTENT = 0x015e,
        CB_GETDROPPEDWIDTH = 0x015f,
        CB_SETDROPPEDWIDTH = 0x0160,
        CB_INITSTORAGE = 0x0161,
        CB_MULTIPLEADDSTRING = 0x0163,
        CB_GETCOMBOBOXINFO = 0x0164,
        CB_MSGMAX_501 = 0x0165,
        CB_MSGMAX_WCE400 = 0x0163,
        CB_MSGMAX_400 = 0x0162,
        CB_MSGMAX_PRE400 = 0x015B,
        SBM_SETPOS = 0x00E0,
        SBM_GETPOS = 0x00E1,
        SBM_SETRANGE = 0x00E2,
        SBM_SETRANGEREDRAW = 0x00E6,
        SBM_GETRANGE = 0x00E3,
        SBM_ENABLE_ARROWS = 0x00E4,
        SBM_SETSCROLLINFO = 0x00E9,
        SBM_GETSCROLLINFO = 0x00EA,
        SBM_GETSCROLLBARINFO = 0x00EB,
        LVM_FIRST = 0x1000, // ListView messages
        TV_FIRST = 0x1100, // TreeView messages
        HDM_FIRST = 0x1200, // Header messages
        TCM_FIRST = 0x1300, // Tab control messages
        PGM_FIRST = 0x1400, // Pager control messages
        ECM_FIRST = 0x1500, // Edit control messages
        BCM_FIRST = 0x1600, // Button control messages
        CBM_FIRST = 0x1700, // Combobox control messages
        CCM_FIRST = 0x2000, // Common control shared messages
        CCM_LAST = (CCM_FIRST + 0x200),
        CCM_SETBKCOLOR = (CCM_FIRST + 1),
        CCM_SETCOLORSCHEME = (CCM_FIRST + 2),
        CCM_GETCOLORSCHEME = (CCM_FIRST + 3),
        CCM_GETDROPTARGET = (CCM_FIRST + 4),
        CCM_SETUNICODEFORMAT = (CCM_FIRST + 5),
        CCM_GETUNICODEFORMAT = (CCM_FIRST + 6),
        CCM_SETVERSION = (CCM_FIRST + 0x7),
        CCM_GETVERSION = (CCM_FIRST + 0x8),
        CCM_SETNOTIFYWINDOW = (CCM_FIRST + 0x9),
        CCM_SETWINDOWTHEME = (CCM_FIRST + 0xb),
        CCM_DPISCALE = (CCM_FIRST + 0xc),
        HDM_GETITEMCOUNT = (HDM_FIRST + 0),
        HDM_INSERTITEMA = (HDM_FIRST + 1),
        HDM_INSERTITEMW = (HDM_FIRST + 10),
        HDM_DELETEITEM = (HDM_FIRST + 2),
        HDM_GETITEMA = (HDM_FIRST + 3),
        HDM_GETITEMW = (HDM_FIRST + 11),
        HDM_SETITEMA = (HDM_FIRST + 4),
        HDM_SETITEMW = (HDM_FIRST + 12),
        HDM_LAYOUT = (HDM_FIRST + 5),
        HDM_HITTEST = (HDM_FIRST + 6),
        HDM_GETITEMRECT = (HDM_FIRST + 7),
        HDM_SETIMAGELIST = (HDM_FIRST + 8),
        HDM_GETIMAGELIST = (HDM_FIRST + 9),
        HDM_ORDERTOINDEX = (HDM_FIRST + 15),
        HDM_CREATEDRAGIMAGE = (HDM_FIRST + 16),
        HDM_GETORDERARRAY = (HDM_FIRST + 17),
        HDM_SETORDERARRAY = (HDM_FIRST + 18),
        HDM_SETHOTDIVIDER = (HDM_FIRST + 19),
        HDM_SETBITMAPMARGIN = (HDM_FIRST + 20),
        HDM_GETBITMAPMARGIN = (HDM_FIRST + 21),
        HDM_SETUNICODEFORMAT = CCM_SETUNICODEFORMAT,
        HDM_GETUNICODEFORMAT = CCM_GETUNICODEFORMAT,
        HDM_SETFILTERCHANGETIMEOUT = (HDM_FIRST + 22),
        HDM_EDITFILTER = (HDM_FIRST + 23),
        HDM_CLEARFILTER = (HDM_FIRST + 24),
        TB_ENABLEBUTTON = (WM_USER + 1),
        TB_CHECKBUTTON = (WM_USER + 2),
        TB_PRESSBUTTON = (WM_USER + 3),
        TB_HIDEBUTTON = (WM_USER + 4),
        TB_INDETERMINATE = (WM_USER + 5),
        TB_MARKBUTTON = (WM_USER + 6),
        TB_ISBUTTONENABLED = (WM_USER + 9),
        TB_ISBUTTONCHECKED = (WM_USER + 10),
        TB_ISBUTTONPRESSED = (WM_USER + 11),
        TB_ISBUTTONHIDDEN = (WM_USER + 12),
        TB_ISBUTTONINDETERMINATE = (WM_USER + 13),
        TB_ISBUTTONHIGHLIGHTED = (WM_USER + 14),
        TB_SETSTATE = (WM_USER + 17),
        TB_GETSTATE = (WM_USER + 18),
        TB_ADDBITMAP = (WM_USER + 19),
        TB_ADDBUTTONSA = (WM_USER + 20),
        TB_INSERTBUTTONA = (WM_USER + 21),
        TB_ADDBUTTONS = (WM_USER + 20),
        TB_INSERTBUTTON = (WM_USER + 21),
        TB_DELETEBUTTON = (WM_USER + 22),
        TB_GETBUTTON = (WM_USER + 23),
        TB_BUTTONCOUNT = (WM_USER + 24),
        TB_COMMANDTOINDEX = (WM_USER + 25),
        TB_SAVERESTOREA = (WM_USER + 26),
        TB_SAVERESTOREW = (WM_USER + 76),
        TB_CUSTOMIZE = (WM_USER + 27),
        TB_ADDSTRINGA = (WM_USER + 28),
        TB_ADDSTRINGW = (WM_USER + 77),
        TB_GETITEMRECT = (WM_USER + 29),
        TB_BUTTONSTRUCTSIZE = (WM_USER + 30),
        TB_SETBUTTONSIZE = (WM_USER + 31),
        TB_SETBITMAPSIZE = (WM_USER + 32),
        TB_AUTOSIZE = (WM_USER + 33),
        TB_GETTOOLTIPS = (WM_USER + 35),
        TB_SETTOOLTIPS = (WM_USER + 36),
        TB_SETPARENT = (WM_USER + 37),
        TB_SETROWS = (WM_USER + 39),
        TB_GETROWS = (WM_USER + 40),
        TB_SETCMDID = (WM_USER + 42),
        TB_CHANGEBITMAP = (WM_USER + 43),
        TB_GETBITMAP = (WM_USER + 44),
        TB_GETBUTTONTEXTA = (WM_USER + 45),
        TB_GETBUTTONTEXTW = (WM_USER + 75),
        TB_REPLACEBITMAP = (WM_USER + 46),
        TB_SETINDENT = (WM_USER + 47),
        TB_SETIMAGELIST = (WM_USER + 48),
        TB_GETIMAGELIST = (WM_USER + 49),
        TB_LOADIMAGES = (WM_USER + 50),
        TB_GETRECT = (WM_USER + 51),
        TB_SETHOTIMAGELIST = (WM_USER + 52),
        TB_GETHOTIMAGELIST = (WM_USER + 53),
        TB_SETDISABLEDIMAGELIST = (WM_USER + 54),
        TB_GETDISABLEDIMAGELIST = (WM_USER + 55),
        TB_SETSTYLE = (WM_USER + 56),
        TB_GETSTYLE = (WM_USER + 57),
        TB_GETBUTTONSIZE = (WM_USER + 58),
        TB_SETBUTTONWIDTH = (WM_USER + 59),
        TB_SETMAXTEXTROWS = (WM_USER + 60),
        TB_GETTEXTROWS = (WM_USER + 61),
        TB_GETOBJECT = (WM_USER + 62),
        TB_GETHOTITEM = (WM_USER + 71),
        TB_SETHOTITEM = (WM_USER + 72),
        TB_SETANCHORHIGHLIGHT = (WM_USER + 73),
        TB_GETANCHORHIGHLIGHT = (WM_USER + 74),
        TB_MAPACCELERATORA = (WM_USER + 78),
        TB_GETINSERTMARK = (WM_USER + 79),
        TB_SETINSERTMARK = (WM_USER + 80),
        TB_INSERTMARKHITTEST = (WM_USER + 81),
        TB_MOVEBUTTON = (WM_USER + 82),
        TB_GETMAXSIZE = (WM_USER + 83),
        TB_SETEXTENDEDSTYLE = (WM_USER + 84),
        TB_GETEXTENDEDSTYLE = (WM_USER + 85),
        TB_GETPADDING = (WM_USER + 86),
        TB_SETPADDING = (WM_USER + 87),
        TB_SETINSERTMARKCOLOR = (WM_USER + 88),
        TB_GETINSERTMARKCOLOR = (WM_USER + 89),
        TB_SETCOLORSCHEME = CCM_SETCOLORSCHEME,
        TB_GETCOLORSCHEME = CCM_GETCOLORSCHEME,
        TB_SETUNICODEFORMAT = CCM_SETUNICODEFORMAT,
        TB_GETUNICODEFORMAT = CCM_GETUNICODEFORMAT,
        TB_MAPACCELERATORW = (WM_USER + 90),
        TB_GETBITMAPFLAGS = (WM_USER + 41),
        TB_GETBUTTONINFOW = (WM_USER + 63),
        TB_SETBUTTONINFOW = (WM_USER + 64),
        TB_GETBUTTONINFOA = (WM_USER + 65),
        TB_SETBUTTONINFOA = (WM_USER + 66),
        TB_INSERTBUTTONW = (WM_USER + 67),
        TB_ADDBUTTONSW = (WM_USER + 68),
        TB_HITTEST = (WM_USER + 69),
        TB_SETDRAWTEXTFLAGS = (WM_USER + 70),
        TB_GETSTRINGW = (WM_USER + 91),
        TB_GETSTRINGA = (WM_USER + 92),
        TB_GETMETRICS = (WM_USER + 101),
        TB_SETMETRICS = (WM_USER + 102),
        TB_SETWINDOWTHEME = CCM_SETWINDOWTHEME,
        RB_INSERTBANDA = (WM_USER + 1),
        RB_DELETEBAND = (WM_USER + 2),
        RB_GETBARINFO = (WM_USER + 3),
        RB_SETBARINFO = (WM_USER + 4),
        RB_GETBANDINFO = (WM_USER + 5),
        RB_SETBANDINFOA = (WM_USER + 6),
        RB_SETPARENT = (WM_USER + 7),
        RB_HITTEST = (WM_USER + 8),
        RB_GETRECT = (WM_USER + 9),
        RB_INSERTBANDW = (WM_USER + 10),
        RB_SETBANDINFOW = (WM_USER + 11),
        RB_GETBANDCOUNT = (WM_USER + 12),
        RB_GETROWCOUNT = (WM_USER + 13),
        RB_GETROWHEIGHT = (WM_USER + 14),
        RB_IDTOINDEX = (WM_USER + 16),
        RB_GETTOOLTIPS = (WM_USER + 17),
        RB_SETTOOLTIPS = (WM_USER + 18),
        RB_SETBKCOLOR = (WM_USER + 19),
        RB_GETBKCOLOR = (WM_USER + 20),
        RB_SETTEXTCOLOR = (WM_USER + 21),
        RB_GETTEXTCOLOR = (WM_USER + 22),
        RB_SIZETORECT = (WM_USER + 23),
        RB_SETCOLORSCHEME = CCM_SETCOLORSCHEME,
        RB_GETCOLORSCHEME = CCM_GETCOLORSCHEME,
        RB_BEGINDRAG = (WM_USER + 24),
        RB_ENDDRAG = (WM_USER + 25),
        RB_DRAGMOVE = (WM_USER + 26),
        RB_GETBARHEIGHT = (WM_USER + 27),
        RB_GETBANDINFOW = (WM_USER + 28),
        RB_GETBANDINFOA = (WM_USER + 29),
        RB_MINIMIZEBAND = (WM_USER + 30),
        RB_MAXIMIZEBAND = (WM_USER + 31),
        RB_GETDROPTARGET = (CCM_GETDROPTARGET),
        RB_GETBANDBORDERS = (WM_USER + 34),
        RB_SHOWBAND = (WM_USER + 35),
        RB_SETPALETTE = (WM_USER + 37),
        RB_GETPALETTE = (WM_USER + 38),
        RB_MOVEBAND = (WM_USER + 39),
        RB_SETUNICODEFORMAT = CCM_SETUNICODEFORMAT,
        RB_GETUNICODEFORMAT = CCM_GETUNICODEFORMAT,
        RB_GETBANDMARGINS = (WM_USER + 40),
        RB_SETWINDOWTHEME = CCM_SETWINDOWTHEME,
        RB_PUSHCHEVRON = (WM_USER + 43),
        TTM_ACTIVATE = (WM_USER + 1),
        TTM_SETDELAYTIME = (WM_USER + 3),
        TTM_ADDTOOLA = (WM_USER + 4),
        TTM_ADDTOOLW = (WM_USER + 50),
        TTM_DELTOOLA = (WM_USER + 5),
        TTM_DELTOOLW = (WM_USER + 51),
        TTM_NEWTOOLRECTA = (WM_USER + 6),
        TTM_NEWTOOLRECTW = (WM_USER + 52),
        TTM_RELAYEVENT = (WM_USER + 7),
        TTM_GETTOOLINFOA = (WM_USER + 8),
        TTM_GETTOOLINFOW = (WM_USER + 53),
        TTM_SETTOOLINFOA = (WM_USER + 9),
        TTM_SETTOOLINFOW = (WM_USER + 54),
        TTM_HITTESTA = (WM_USER + 10),
        TTM_HITTESTW = (WM_USER + 55),
        TTM_GETTEXTA = (WM_USER + 11),
        TTM_GETTEXTW = (WM_USER + 56),
        TTM_UPDATETIPTEXTA = (WM_USER + 12),
        TTM_UPDATETIPTEXTW = (WM_USER + 57),
        TTM_GETTOOLCOUNT = (WM_USER + 13),
        TTM_ENUMTOOLSA = (WM_USER + 14),
        TTM_ENUMTOOLSW = (WM_USER + 58),
        TTM_GETCURRENTTOOLA = (WM_USER + 15),
        TTM_GETCURRENTTOOLW = (WM_USER + 59),
        TTM_WINDOWFROMPOINT = (WM_USER + 16),
        TTM_TRACKACTIVATE = (WM_USER + 17),
        TTM_TRACKPOSITION = (WM_USER + 18),
        TTM_SETTIPBKCOLOR = (WM_USER + 19),
        TTM_SETTIPTEXTCOLOR = (WM_USER + 20),
        TTM_GETDELAYTIME = (WM_USER + 21),
        TTM_GETTIPBKCOLOR = (WM_USER + 22),
        TTM_GETTIPTEXTCOLOR = (WM_USER + 23),
        TTM_SETMAXTIPWIDTH = (WM_USER + 24),
        TTM_GETMAXTIPWIDTH = (WM_USER + 25),
        TTM_SETMARGIN = (WM_USER + 26),
        TTM_GETMARGIN = (WM_USER + 27),
        TTM_POP = (WM_USER + 28),
        TTM_UPDATE = (WM_USER + 29),
        TTM_GETBUBBLESIZE = (WM_USER + 30),
        TTM_ADJUSTRECT = (WM_USER + 31),
        TTM_SETTITLEA = (WM_USER + 32),
        TTM_SETTITLEW = (WM_USER + 33),
        TTM_POPUP = (WM_USER + 34),
        TTM_GETTITLE = (WM_USER + 35),
        TTM_SETWINDOWTHEME = CCM_SETWINDOWTHEME,
        SB_SETTEXTA = (WM_USER + 1),
        SB_SETTEXTW = (WM_USER + 11),
        SB_GETTEXTA = (WM_USER + 2),
        SB_GETTEXTW = (WM_USER + 13),
        SB_GETTEXTLENGTHA = (WM_USER + 3),
        SB_GETTEXTLENGTHW = (WM_USER + 12),
        SB_SETPARTS = (WM_USER + 4),
        SB_GETPARTS = (WM_USER + 6),
        SB_GETBORDERS = (WM_USER + 7),
        SB_SETMINHEIGHT = (WM_USER + 8),
        SB_SIMPLE = (WM_USER + 9),
        SB_GETRECT = (WM_USER + 10),
        SB_ISSIMPLE = (WM_USER + 14),
        SB_SETICON = (WM_USER + 15),
        SB_SETTIPTEXTA = (WM_USER + 16),
        SB_SETTIPTEXTW = (WM_USER + 17),
        SB_GETTIPTEXTA = (WM_USER + 18),
        SB_GETTIPTEXTW = (WM_USER + 19),
        SB_GETICON = (WM_USER + 20),
        SB_SETUNICODEFORMAT = CCM_SETUNICODEFORMAT,
        SB_GETUNICODEFORMAT = CCM_GETUNICODEFORMAT,
        SB_SETBKCOLOR = CCM_SETBKCOLOR,
        SB_SIMPLEID = 0x00ff,
        TBM_GETPOS = (WM_USER),
        TBM_GETRANGEMIN = (WM_USER + 1),
        TBM_GETRANGEMAX = (WM_USER + 2),
        TBM_GETTIC = (WM_USER + 3),
        TBM_SETTIC = (WM_USER + 4),
        TBM_SETPOS = (WM_USER + 5),
        TBM_SETRANGE = (WM_USER + 6),
        TBM_SETRANGEMIN = (WM_USER + 7),
        TBM_SETRANGEMAX = (WM_USER + 8),
        TBM_CLEARTICS = (WM_USER + 9),
        TBM_SETSEL = (WM_USER + 10),
        TBM_SETSELSTART = (WM_USER + 11),
        TBM_SETSELEND = (WM_USER + 12),
        TBM_GETPTICS = (WM_USER + 14),
        TBM_GETTICPOS = (WM_USER + 15),
        TBM_GETNUMTICS = (WM_USER + 16),
        TBM_GETSELSTART = (WM_USER + 17),
        TBM_GETSELEND = (WM_USER + 18),
        TBM_CLEARSEL = (WM_USER + 19),
        TBM_SETTICFREQ = (WM_USER + 20),
        TBM_SETPAGESIZE = (WM_USER + 21),
        TBM_GETPAGESIZE = (WM_USER + 22),
        TBM_SETLINESIZE = (WM_USER + 23),
        TBM_GETLINESIZE = (WM_USER + 24),
        TBM_GETTHUMBRECT = (WM_USER + 25),
        TBM_GETCHANNELRECT = (WM_USER + 26),
        TBM_SETTHUMBLENGTH = (WM_USER + 27),
        TBM_GETTHUMBLENGTH = (WM_USER + 28),
        TBM_SETTOOLTIPS = (WM_USER + 29),
        TBM_GETTOOLTIPS = (WM_USER + 30),
        TBM_SETTIPSIDE = (WM_USER + 31),
        TBM_SETBUDDY = (WM_USER + 32),
        TBM_GETBUDDY = (WM_USER + 33),
        TBM_SETUNICODEFORMAT = CCM_SETUNICODEFORMAT,
        TBM_GETUNICODEFORMAT = CCM_GETUNICODEFORMAT,
        DL_BEGINDRAG = (WM_USER + 133),
        DL_DRAGGING = (WM_USER + 134),
        DL_DROPPED = (WM_USER + 135),
        DL_CANCELDRAG = (WM_USER + 136),
        UDM_SETRANGE = (WM_USER + 101),
        UDM_GETRANGE = (WM_USER + 102),
        UDM_SETPOS = (WM_USER + 103),
        UDM_GETPOS = (WM_USER + 104),
        UDM_SETBUDDY = (WM_USER + 105),
        UDM_GETBUDDY = (WM_USER + 106),
        UDM_SETACCEL = (WM_USER + 107),
        UDM_GETACCEL = (WM_USER + 108),
        UDM_SETBASE = (WM_USER + 109),
        UDM_GETBASE = (WM_USER + 110),
        UDM_SETRANGE32 = (WM_USER + 111),
        UDM_GETRANGE32 = (WM_USER + 112),
        UDM_SETUNICODEFORMAT = CCM_SETUNICODEFORMAT,
        UDM_GETUNICODEFORMAT = CCM_GETUNICODEFORMAT,
        UDM_SETPOS32 = (WM_USER + 113),
        UDM_GETPOS32 = (WM_USER + 114),
        PBM_SETRANGE = (WM_USER + 1),
        PBM_SETPOS = (WM_USER + 2),
        PBM_DELTAPOS = (WM_USER + 3),
        PBM_SETSTEP = (WM_USER + 4),
        PBM_STEPIT = (WM_USER + 5),
        PBM_SETRANGE32 = (WM_USER + 6),
        PBM_GETRANGE = (WM_USER + 7),
        PBM_GETPOS = (WM_USER + 8),
        PBM_SETBARCOLOR = (WM_USER + 9),
        PBM_SETBKCOLOR = CCM_SETBKCOLOR,
        HKM_SETHOTKEY = (WM_USER + 1),
        HKM_GETHOTKEY = (WM_USER + 2),
        HKM_SETRULES = (WM_USER + 3),
        LVM_SETUNICODEFORMAT = CCM_SETUNICODEFORMAT,
        LVM_GETUNICODEFORMAT = CCM_GETUNICODEFORMAT,
        LVM_GETBKCOLOR = (LVM_FIRST + 0),
        LVM_SETBKCOLOR = (LVM_FIRST + 1),
        LVM_GETIMAGELIST = (LVM_FIRST + 2),
        LVM_SETIMAGELIST = (LVM_FIRST + 3),
        LVM_GETITEMCOUNT = (LVM_FIRST + 4),
        LVM_GETITEMA = (LVM_FIRST + 5),
        LVM_GETITEMW = (LVM_FIRST + 75),
        LVM_SETITEMA = (LVM_FIRST + 6),
        LVM_SETITEMW = (LVM_FIRST + 76),
        LVM_INSERTITEMA = (LVM_FIRST + 7),
        LVM_INSERTITEMW = (LVM_FIRST + 77),
        LVM_DELETEITEM = (LVM_FIRST + 8),
        LVM_DELETEALLITEMS = (LVM_FIRST + 9),
        LVM_GETCALLBACKMASK = (LVM_FIRST + 10),
        LVM_SETCALLBACKMASK = (LVM_FIRST + 11),
        LVM_FINDITEMA = (LVM_FIRST + 13),
        LVM_FINDITEMW = (LVM_FIRST + 83),
        LVM_GETITEMRECT = (LVM_FIRST + 14),
        LVM_SETITEMPOSITION = (LVM_FIRST + 15),
        LVM_GETITEMPOSITION = (LVM_FIRST + 16),
        LVM_GETSTRINGWIDTHA = (LVM_FIRST + 17),
        LVM_GETSTRINGWIDTHW = (LVM_FIRST + 87),
        LVM_HITTEST = (LVM_FIRST + 18),
        LVM_ENSUREVISIBLE = (LVM_FIRST + 19),
        LVM_SCROLL = (LVM_FIRST + 20),
        LVM_REDRAWITEMS = (LVM_FIRST + 21),
        LVM_ARRANGE = (LVM_FIRST + 22),
        LVM_EDITLABELA = (LVM_FIRST + 23),
        LVM_EDITLABELW = (LVM_FIRST + 118),
        LVM_GETEDITCONTROL = (LVM_FIRST + 24),
        LVM_GETCOLUMNA = (LVM_FIRST + 25),
        LVM_GETCOLUMNW = (LVM_FIRST + 95),
        LVM_SETCOLUMNA = (LVM_FIRST + 26),
        LVM_SETCOLUMNW = (LVM_FIRST + 96),
        LVM_INSERTCOLUMNA = (LVM_FIRST + 27),
        LVM_INSERTCOLUMNW = (LVM_FIRST + 97),
        LVM_DELETECOLUMN = (LVM_FIRST + 28),
        LVM_GETCOLUMNWIDTH = (LVM_FIRST + 29),
        LVM_SETCOLUMNWIDTH = (LVM_FIRST + 30),
        LVM_CREATEDRAGIMAGE = (LVM_FIRST + 33),
        LVM_GETVIEWRECT = (LVM_FIRST + 34),
        LVM_GETTEXTCOLOR = (LVM_FIRST + 35),
        LVM_SETTEXTCOLOR = (LVM_FIRST + 36),
        LVM_GETTEXTBKCOLOR = (LVM_FIRST + 37),
        LVM_SETTEXTBKCOLOR = (LVM_FIRST + 38),
        LVM_GETTOPINDEX = (LVM_FIRST + 39),
        LVM_GETCOUNTPERPAGE = (LVM_FIRST + 40),
        LVM_GETORIGIN = (LVM_FIRST + 41),
        LVM_UPDATE = (LVM_FIRST + 42),
        LVM_SETITEMSTATE = (LVM_FIRST + 43),
        LVM_GETITEMSTATE = (LVM_FIRST + 44),
        LVM_GETITEMTEXTA = (LVM_FIRST + 45),
        LVM_GETITEMTEXTW = (LVM_FIRST + 115),
        LVM_SETITEMTEXTA = (LVM_FIRST + 46),
        LVM_SETITEMTEXTW = (LVM_FIRST + 116),
        LVM_SETITEMCOUNT = (LVM_FIRST + 47),
        LVM_SORTITEMS = (LVM_FIRST + 48),
        LVM_SETITEMPOSITION32 = (LVM_FIRST + 49),
        LVM_GETSELECTEDCOUNT = (LVM_FIRST + 50),
        LVM_GETITEMSPACING = (LVM_FIRST + 51),
        LVM_GETISEARCHSTRINGA = (LVM_FIRST + 52),
        LVM_GETISEARCHSTRINGW = (LVM_FIRST + 117),
        LVM_SETICONSPACING = (LVM_FIRST + 53),
        LVM_SETEXTENDEDLISTVIEWSTYLE = (LVM_FIRST + 54),
        LVM_GETEXTENDEDLISTVIEWSTYLE = (LVM_FIRST + 55),
        LVM_GETSUBITEMRECT = (LVM_FIRST + 56),
        LVM_SUBITEMHITTEST = (LVM_FIRST + 57),
        LVM_SETCOLUMNORDERARRAY = (LVM_FIRST + 58),
        LVM_GETCOLUMNORDERARRAY = (LVM_FIRST + 59),
        LVM_SETHOTITEM = (LVM_FIRST + 60),
        LVM_GETHOTITEM = (LVM_FIRST + 61),
        LVM_SETHOTCURSOR = (LVM_FIRST + 62),
        LVM_GETHOTCURSOR = (LVM_FIRST + 63),
        LVM_APPROXIMATEVIEWRECT = (LVM_FIRST + 64),
        LVM_SETWORKAREAS = (LVM_FIRST + 65),
        LVM_GETWORKAREAS = (LVM_FIRST + 70),
        LVM_GETNUMBEROFWORKAREAS = (LVM_FIRST + 73),
        LVM_GETSELECTIONMARK = (LVM_FIRST + 66),
        LVM_SETSELECTIONMARK = (LVM_FIRST + 67),
        LVM_SETHOVERTIME = (LVM_FIRST + 71),
        LVM_GETHOVERTIME = (LVM_FIRST + 72),
        LVM_SETTOOLTIPS = (LVM_FIRST + 74),
        LVM_GETTOOLTIPS = (LVM_FIRST + 78),
        LVM_SORTITEMSEX = (LVM_FIRST + 81),
        LVM_SETBKIMAGEA = (LVM_FIRST + 68),
        LVM_SETBKIMAGEW = (LVM_FIRST + 138),
        LVM_GETBKIMAGEA = (LVM_FIRST + 69),
        LVM_GETBKIMAGEW = (LVM_FIRST + 139),
        LVM_SETSELECTEDCOLUMN = (LVM_FIRST + 140),
        LVM_SETTILEWIDTH = (LVM_FIRST + 141),
        LVM_SETVIEW = (LVM_FIRST + 142),
        LVM_GETVIEW = (LVM_FIRST + 143),
        LVM_INSERTGROUP = (LVM_FIRST + 145),
        LVM_SETGROUPINFO = (LVM_FIRST + 147),
        LVM_GETGROUPINFO = (LVM_FIRST + 149),
        LVM_REMOVEGROUP = (LVM_FIRST + 150),
        LVM_MOVEGROUP = (LVM_FIRST + 151),
        LVM_MOVEITEMTOGROUP = (LVM_FIRST + 154),
        LVM_SETGROUPMETRICS = (LVM_FIRST + 155),
        LVM_GETGROUPMETRICS = (LVM_FIRST + 156),
        LVM_ENABLEGROUPVIEW = (LVM_FIRST + 157),
        LVM_SORTGROUPS = (LVM_FIRST + 158),
        LVM_INSERTGROUPSORTED = (LVM_FIRST + 159),
        LVM_REMOVEALLGROUPS = (LVM_FIRST + 160),
        LVM_HASGROUP = (LVM_FIRST + 161),
        LVM_SETTILEVIEWINFO = (LVM_FIRST + 162),
        LVM_GETTILEVIEWINFO = (LVM_FIRST + 163),
        LVM_SETTILEINFO = (LVM_FIRST + 164),
        LVM_GETTILEINFO = (LVM_FIRST + 165),
        LVM_SETINSERTMARK = (LVM_FIRST + 166),
        LVM_GETINSERTMARK = (LVM_FIRST + 167),
        LVM_INSERTMARKHITTEST = (LVM_FIRST + 168),
        LVM_GETINSERTMARKRECT = (LVM_FIRST + 169),
        LVM_SETINSERTMARKCOLOR = (LVM_FIRST + 170),
        LVM_GETINSERTMARKCOLOR = (LVM_FIRST + 171),
        LVM_SETINFOTIP = (LVM_FIRST + 173),
        LVM_GETSELECTEDCOLUMN = (LVM_FIRST + 174),
        LVM_ISGROUPVIEWENABLED = (LVM_FIRST + 175),
        LVM_GETOUTLINECOLOR = (LVM_FIRST + 176),
        LVM_SETOUTLINECOLOR = (LVM_FIRST + 177),
        LVM_CANCELEDITLABEL = (LVM_FIRST + 179),
        LVM_MAPINDEXTOID = (LVM_FIRST + 180),
        LVM_MAPIDTOINDEX = (LVM_FIRST + 181),
        TVM_INSERTITEMA = (TV_FIRST + 0),
        TVM_INSERTITEMW = (TV_FIRST + 50),
        TVM_DELETEITEM = (TV_FIRST + 1),
        TVM_EXPAND = (TV_FIRST + 2),
        TVM_GETITEMRECT = (TV_FIRST + 4),
        TVM_GETCOUNT = (TV_FIRST + 5),
        TVM_GETINDENT = (TV_FIRST + 6),
        TVM_SETINDENT = (TV_FIRST + 7),
        TVM_GETIMAGELIST = (TV_FIRST + 8),
        TVM_SETIMAGELIST = (TV_FIRST + 9),
        TVM_GETNEXTITEM = (TV_FIRST + 10),
        TVM_SELECTITEM = (TV_FIRST + 11),
        TVM_GETITEMA = (TV_FIRST + 12),
        TVM_GETITEMW = (TV_FIRST + 62),
        TVM_SETITEMA = (TV_FIRST + 13),
        TVM_SETITEMW = (TV_FIRST + 63),
        TVM_EDITLABELA = (TV_FIRST + 14),
        TVM_EDITLABELW = (TV_FIRST + 65),
        TVM_GETEDITCONTROL = (TV_FIRST + 15),
        TVM_GETVISIBLECOUNT = (TV_FIRST + 16),
        TVM_HITTEST = (TV_FIRST + 17),
        TVM_CREATEDRAGIMAGE = (TV_FIRST + 18),
        TVM_SORTCHILDREN = (TV_FIRST + 19),
        TVM_ENSUREVISIBLE = (TV_FIRST + 20),
        TVM_SORTCHILDRENCB = (TV_FIRST + 21),
        TVM_ENDEDITLABELNOW = (TV_FIRST + 22),
        TVM_GETISEARCHSTRINGA = (TV_FIRST + 23),
        TVM_GETISEARCHSTRINGW = (TV_FIRST + 64),
        TVM_SETTOOLTIPS = (TV_FIRST + 24),
        TVM_GETTOOLTIPS = (TV_FIRST + 25),
        TVM_SETINSERTMARK = (TV_FIRST + 26),
        TVM_SETUNICODEFORMAT = CCM_SETUNICODEFORMAT,
        TVM_GETUNICODEFORMAT = CCM_GETUNICODEFORMAT,
        TVM_SETITEMHEIGHT = (TV_FIRST + 27),
        TVM_GETITEMHEIGHT = (TV_FIRST + 28),
        TVM_SETBKCOLOR = (TV_FIRST + 29),
        TVM_SETTEXTCOLOR = (TV_FIRST + 30),
        TVM_GETBKCOLOR = (TV_FIRST + 31),
        TVM_GETTEXTCOLOR = (TV_FIRST + 32),
        TVM_SETSCROLLTIME = (TV_FIRST + 33),
        TVM_GETSCROLLTIME = (TV_FIRST + 34),
        TVM_SETINSERTMARKCOLOR = (TV_FIRST + 37),
        TVM_GETINSERTMARKCOLOR = (TV_FIRST + 38),
        TVM_GETITEMSTATE = (TV_FIRST + 39),
        TVM_SETLINECOLOR = (TV_FIRST + 40),
        TVM_GETLINECOLOR = (TV_FIRST + 41),
        TVM_MAPACCIDTOHTREEITEM = (TV_FIRST + 42),
        TVM_MAPHTREEITEMTOACCID = (TV_FIRST + 43),
        CBEM_INSERTITEMA = (WM_USER + 1),
        CBEM_SETIMAGELIST = (WM_USER + 2),
        CBEM_GETIMAGELIST = (WM_USER + 3),
        CBEM_GETITEMA = (WM_USER + 4),
        CBEM_SETITEMA = (WM_USER + 5),
        CBEM_DELETEITEM = CB_DELETESTRING,
        CBEM_GETCOMBOCONTROL = (WM_USER + 6),
        CBEM_GETEDITCONTROL = (WM_USER + 7),
        CBEM_SETEXTENDEDSTYLE = (WM_USER + 14),
        CBEM_GETEXTENDEDSTYLE = (WM_USER + 9),
        CBEM_SETUNICODEFORMAT = CCM_SETUNICODEFORMAT,
        CBEM_GETUNICODEFORMAT = CCM_GETUNICODEFORMAT,
        CBEM_SETEXSTYLE = (WM_USER + 8),
        CBEM_GETEXSTYLE = (WM_USER + 9),
        CBEM_HASEDITCHANGED = (WM_USER + 10),
        CBEM_INSERTITEMW = (WM_USER + 11),
        CBEM_SETITEMW = (WM_USER + 12),
        CBEM_GETITEMW = (WM_USER + 13),
        TCM_GETIMAGELIST = (TCM_FIRST + 2),
        TCM_SETIMAGELIST = (TCM_FIRST + 3),
        TCM_GETITEMCOUNT = (TCM_FIRST + 4),
        TCM_GETITEMA = (TCM_FIRST + 5),
        TCM_GETITEMW = (TCM_FIRST + 60),
        TCM_SETITEMA = (TCM_FIRST + 6),
        TCM_SETITEMW = (TCM_FIRST + 61),
        TCM_INSERTITEMA = (TCM_FIRST + 7),
        TCM_INSERTITEMW = (TCM_FIRST + 62),
        TCM_DELETEITEM = (TCM_FIRST + 8),
        TCM_DELETEALLITEMS = (TCM_FIRST + 9),
        TCM_GETITEMRECT = (TCM_FIRST + 10),
        TCM_GETCURSEL = (TCM_FIRST + 11),
        TCM_SETCURSEL = (TCM_FIRST + 12),
        TCM_HITTEST = (TCM_FIRST + 13),
        TCM_SETITEMEXTRA = (TCM_FIRST + 14),
        TCM_ADJUSTRECT = (TCM_FIRST + 40),
        TCM_SETITEMSIZE = (TCM_FIRST + 41),
        TCM_REMOVEIMAGE = (TCM_FIRST + 42),
        TCM_SETPADDING = (TCM_FIRST + 43),
        TCM_GETROWCOUNT = (TCM_FIRST + 44),
        TCM_GETTOOLTIPS = (TCM_FIRST + 45),
        TCM_SETTOOLTIPS = (TCM_FIRST + 46),
        TCM_GETCURFOCUS = (TCM_FIRST + 47),
        TCM_SETCURFOCUS = (TCM_FIRST + 48),
        TCM_SETMINTABWIDTH = (TCM_FIRST + 49),
        TCM_DESELECTALL = (TCM_FIRST + 50),
        TCM_HIGHLIGHTITEM = (TCM_FIRST + 51),
        TCM_SETEXTENDEDSTYLE = (TCM_FIRST + 52),
        TCM_GETEXTENDEDSTYLE = (TCM_FIRST + 53),
        TCM_SETUNICODEFORMAT = CCM_SETUNICODEFORMAT,
        TCM_GETUNICODEFORMAT = CCM_GETUNICODEFORMAT,
        ACM_OPENA = (WM_USER + 100),
        ACM_OPENW = (WM_USER + 103),
        ACM_PLAY = (WM_USER + 101),
        ACM_STOP = (WM_USER + 102),
        MCM_FIRST = 0x1000,
        MCM_GETCURSEL = (MCM_FIRST + 1),
        MCM_SETCURSEL = (MCM_FIRST + 2),
        MCM_GETMAXSELCOUNT = (MCM_FIRST + 3),
        MCM_SETMAXSELCOUNT = (MCM_FIRST + 4),
        MCM_GETSELRANGE = (MCM_FIRST + 5),
        MCM_SETSELRANGE = (MCM_FIRST + 6),
        MCM_GETMONTHRANGE = (MCM_FIRST + 7),
        MCM_SETDAYSTATE = (MCM_FIRST + 8),
        MCM_GETMINREQRECT = (MCM_FIRST + 9),
        MCM_SETCOLOR = (MCM_FIRST + 10),
        MCM_GETCOLOR = (MCM_FIRST + 11),
        MCM_SETTODAY = (MCM_FIRST + 12),
        MCM_GETTODAY = (MCM_FIRST + 13),
        MCM_HITTEST = (MCM_FIRST + 14),
        MCM_SETFIRSTDAYOFWEEK = (MCM_FIRST + 15),
        MCM_GETFIRSTDAYOFWEEK = (MCM_FIRST + 16),
        MCM_GETRANGE = (MCM_FIRST + 17),
        MCM_SETRANGE = (MCM_FIRST + 18),
        MCM_GETMONTHDELTA = (MCM_FIRST + 19),
        MCM_SETMONTHDELTA = (MCM_FIRST + 20),
        MCM_GETMAXTODAYWIDTH = (MCM_FIRST + 21),
        MCM_SETUNICODEFORMAT = CCM_SETUNICODEFORMAT,
        MCM_GETUNICODEFORMAT = CCM_GETUNICODEFORMAT,
        DTM_FIRST = 0x1000,
        DTM_GETSYSTEMTIME = (DTM_FIRST + 1),
        DTM_SETSYSTEMTIME = (DTM_FIRST + 2),
        DTM_GETRANGE = (DTM_FIRST + 3),
        DTM_SETRANGE = (DTM_FIRST + 4),
        DTM_SETFORMATA = (DTM_FIRST + 5),
        DTM_SETFORMATW = (DTM_FIRST + 50),
        DTM_SETMCCOLOR = (DTM_FIRST + 6),
        DTM_GETMCCOLOR = (DTM_FIRST + 7),
        DTM_GETMONTHCAL = (DTM_FIRST + 8),
        DTM_SETMCFONT = (DTM_FIRST + 9),
        DTM_GETMCFONT = (DTM_FIRST + 10),
        PGM_SETCHILD = (PGM_FIRST + 1),
        PGM_RECALCSIZE = (PGM_FIRST + 2),
        PGM_FORWARDMOUSE = (PGM_FIRST + 3),
        PGM_SETBKCOLOR = (PGM_FIRST + 4),
        PGM_GETBKCOLOR = (PGM_FIRST + 5),
        PGM_SETBORDER = (PGM_FIRST + 6),
        PGM_GETBORDER = (PGM_FIRST + 7),
        PGM_SETPOS = (PGM_FIRST + 8),
        PGM_GETPOS = (PGM_FIRST + 9),
        PGM_SETBUTTONSIZE = (PGM_FIRST + 10),
        PGM_GETBUTTONSIZE = (PGM_FIRST + 11),
        PGM_GETBUTTONSTATE = (PGM_FIRST + 12),
        PGM_GETDROPTARGET = CCM_GETDROPTARGET,
        BCM_GETIDEALSIZE = (BCM_FIRST + 0x0001),
        BCM_SETIMAGELIST = (BCM_FIRST + 0x0002),
        BCM_GETIMAGELIST = (BCM_FIRST + 0x0003),
        BCM_SETTEXTMARGIN = (BCM_FIRST + 0x0004),
        BCM_GETTEXTMARGIN = (BCM_FIRST + 0x0005),
        EM_SETCUEBANNER = (ECM_FIRST + 1),
        EM_GETCUEBANNER = (ECM_FIRST + 2),
        EM_SHOWBALLOONTIP = (ECM_FIRST + 3),
        EM_HIDEBALLOONTIP = (ECM_FIRST + 4),
        CB_SETMINVISIBLE = (CBM_FIRST + 1),
        CB_GETMINVISIBLE = (CBM_FIRST + 2),
        LM_HITTEST = (WM_USER + 0x300),
        LM_GETIDEALHEIGHT = (WM_USER + 0x301),
        LM_SETITEM = (WM_USER + 0x302),
        LM_GETITEM = (WM_USER + 0x303)
    }

    /// <summary>
    /// Windows Messages
    /// Defined in winuser.h from Windows SDK v6.1
    /// Documentation pulled from MSDN.
    /// </summary>
    public enum WM : uint
    {
        /// <summary>
        /// The WM_NULL message performs no operation. An application sends the WM_NULL message if it wants to post a message that the recipient window will ignore.
        /// </summary>
        NULL = 0x0000,

        /// <summary>
        /// The WM_CREATE message is sent when an application requests that a window be created by calling the CreateWindowEx or CreateWindow function. (The message is sent before the function returns.) The window procedure of the new window receives this message after the window is created, but before the window becomes visible.
        /// </summary>
        CREATE = 0x0001,

        /// <summary>
        /// The WM_DESTROY message is sent when a window is being destroyed. It is sent to the window procedure of the window being destroyed after the window is removed from the screen. 
        /// This message is sent first to the window being destroyed and then to the child windows (if any) as they are destroyed. During the processing of the message, it can be assumed that all child windows still exist.
        /// /// </summary>
        DESTROY = 0x0002,

        /// <summary>
        /// The WM_MOVE message is sent after a window has been moved. 
        /// </summary>
        MOVE = 0x0003,

        /// <summary>
        /// The WM_SIZE message is sent to a window after its size has changed.
        /// </summary>
        SIZE = 0x0005,

        /// <summary>
        /// The WM_ACTIVATE message is sent to both the window being activated and the window being deactivated. If the windows use the same input queue, the message is sent synchronously, first to the window procedure of the top-level window being deactivated, then to the window procedure of the top-level window being activated. If the windows use different input queues, the message is sent asynchronously, so the window is activated immediately. 
        /// </summary>
        ACTIVATE = 0x0006,

        /// <summary>
        /// The WM_SETFOCUS message is sent to a window after it has gained the keyboard focus. 
        /// </summary>
        SETFOCUS = 0x0007,

        /// <summary>
        /// The WM_KILLFOCUS message is sent to a window immediately before it loses the keyboard focus. 
        /// </summary>
        KILLFOCUS = 0x0008,

        /// <summary>
        /// The WM_ENABLE message is sent when an application changes the enabled state of a window. It is sent to the window whose enabled state is changing. This message is sent before the EnableWindow function returns, but after the enabled state (WS_DISABLED style bit) of the window has changed. 
        /// </summary>
        ENABLE = 0x000A,

        /// <summary>
        /// An application sends the WM_SETREDRAW message to a window to allow changes in that window to be redrawn or to prevent changes in that window from being redrawn. 
        /// </summary>
        SETREDRAW = 0x000B,

        /// <summary>
        /// An application sends a WM_SETTEXT message to set the text of a window. 
        /// </summary>
        SETTEXT = 0x000C,

        /// <summary>
        /// An application sends a WM_GETTEXT message to copy the text that corresponds to a window into a buffer provided by the caller. 
        /// </summary>
        GETTEXT = 0x000D,

        /// <summary>
        /// An application sends a WM_GETTEXTLENGTH message to determine the length, in characters, of the text associated with a window. 
        /// </summary>
        GETTEXTLENGTH = 0x000E,

        /// <summary>
        /// The WM_PAINT message is sent when the system or another application makes a request to paint a portion of an application's window. The message is sent when the UpdateWindow or RedrawWindow function is called, or by the DispatchMessage function when the application obtains a WM_PAINT message by using the GetMessage or PeekMessage function. 
        /// </summary>
        PAINT = 0x000F,

        /// <summary>
        /// The WM_CLOSE message is sent as a signal that a window or an application should terminate.
        /// </summary>
        CLOSE = 0x0010,

        /// <summary>
        /// The WM_QUERYENDSESSION message is sent when the user chooses to end the session or when an application calls one of the system shutdown functions. If any application returns zero, the session is not ended. The system stops sending WM_QUERYENDSESSION messages as soon as one application returns zero.
        /// After processing this message, the system sends the WM_ENDSESSION message with the wParam parameter set to the results of the WM_QUERYENDSESSION message.
        /// </summary>
        QUERYENDSESSION = 0x0011,

        /// <summary>
        /// The WM_QUERYOPEN message is sent to an icon when the user requests that the window be restored to its previous size and position.
        /// </summary>
        QUERYOPEN = 0x0013,

        /// <summary>
        /// The WM_ENDSESSION message is sent to an application after the system processes the results of the WM_QUERYENDSESSION message. The WM_ENDSESSION message informs the application whether the session is ending.
        /// </summary>
        ENDSESSION = 0x0016,

        /// <summary>
        /// The WM_QUIT message indicates a request to terminate an application and is generated when the application calls the PostQuitMessage function. It causes the GetMessage function to return zero.
        /// </summary>
        QUIT = 0x0012,

        /// <summary>
        /// The WM_ERASEBKGND message is sent when the window background must be erased (for example, when a window is resized). The message is sent to prepare an invalidated portion of a window for painting. 
        /// </summary>
        ERASEBKGND = 0x0014,

        /// <summary>
        /// This message is sent to all top-level windows when a change is made to a system color setting. 
        /// </summary>
        SYSCOLORCHANGE = 0x0015,

        /// <summary>
        /// The WM_SHOWWINDOW message is sent to a window when the window is about to be hidden or shown.
        /// </summary>
        SHOWWINDOW = 0x0018,

        /// <summary>
        /// An application sends the WM_WININICHANGE message to all top-level windows after making a change to the WIN.INI file. The SystemParametersInfo function sends this message after an application uses the function to change a setting in WIN.INI.
        /// Note  The WM_WININICHANGE message is provided only for compatibility with earlier versions of the system. Applications should use the WM_SETTINGCHANGE message.
        /// </summary>
        WININICHANGE = 0x001A,

        /// <summary>
        /// An application sends the WM_WININICHANGE message to all top-level windows after making a change to the WIN.INI file. The SystemParametersInfo function sends this message after an application uses the function to change a setting in WIN.INI.
        /// Note  The WM_WININICHANGE message is provided only for compatibility with earlier versions of the system. Applications should use the WM_SETTINGCHANGE message.
        /// </summary>
        SETTINGCHANGE = WININICHANGE,

        /// <summary>
        /// The WM_DEVMODECHANGE message is sent to all top-level windows whenever the user changes device-mode settings. 
        /// </summary>
        DEVMODECHANGE = 0x001B,

        /// <summary>
        /// The WM_ACTIVATEAPP message is sent when a window belonging to a different application than the active window is about to be activated. The message is sent to the application whose window is being activated and to the application whose window is being deactivated.
        /// </summary>
        ACTIVATEAPP = 0x001C,

        /// <summary>
        /// An application sends the WM_FONTCHANGE message to all top-level windows in the system after changing the pool of font resources. 
        /// </summary>
        FONTCHANGE = 0x001D,

        /// <summary>
        /// A message that is sent whenever there is a change in the system time.
        /// </summary>
        TIMECHANGE = 0x001E,

        /// <summary>
        /// The WM_CANCELMODE message is sent to cancel certain modes, such as mouse capture. For example, the system sends this message to the active window when a dialog box or message box is displayed. Certain functions also send this message explicitly to the specified window regardless of whether it is the active window. For example, the EnableWindow function sends this message when disabling the specified window.
        /// </summary>
        CANCELMODE = 0x001F,

        /// <summary>
        /// The WM_SETCURSOR message is sent to a window if the mouse causes the cursor to move within a window and mouse input is not captured. 
        /// </summary>
        SETCURSOR = 0x0020,

        /// <summary>
        /// The WM_MOUSEACTIVATE message is sent when the cursor is in an inactive window and the user presses a mouse button. The parent window receives this message only if the child window passes it to the DefWindowProc function.
        /// </summary>
        MOUSEACTIVATE = 0x0021,

        /// <summary>
        /// The WM_CHILDACTIVATE message is sent to a child window when the user clicks the window's title bar or when the window is activated, moved, or sized.
        /// </summary>
        CHILDACTIVATE = 0x0022,

        /// <summary>
        /// The WM_QUEUESYNC message is sent by a computer-based training (CBT) application to separate user-input messages from other messages sent through the WH_JOURNALPLAYBACK Hook procedure. 
        /// </summary>
        QUEUESYNC = 0x0023,

        /// <summary>
        /// The WM_GETMINMAXINFO message is sent to a window when the size or position of the window is about to change. An application can use this message to override the window's default maximized size and position, or its default minimum or maximum tracking size. 
        /// </summary>
        GETMINMAXINFO = 0x0024,

        /// <summary>
        /// Windows NT 3.51 and earlier: The WM_PAINTICON message is sent to a minimized window when the icon is to be painted. This message is not sent by newer versions of Microsoft Windows, except in unusual circumstances explained in the Remarks.
        /// </summary>
        PAINTICON = 0x0026,

        /// <summary>
        /// Windows NT 3.51 and earlier: The WM_ICONERASEBKGND message is sent to a minimized window when the background of the icon must be filled before painting the icon. A window receives this message only if a class icon is defined for the window; otherwise, WM_ERASEBKGND is sent. This message is not sent by newer versions of Windows.
        /// </summary>
        ICONERASEBKGND = 0x0027,

        /// <summary>
        /// The WM_NEXTDLGCTL message is sent to a dialog box procedure to set the keyboard focus to a different control in the dialog box. 
        /// </summary>
        NEXTDLGCTL = 0x0028,

        /// <summary>
        /// The WM_SPOOLERSTATUS message is sent from Print Manager whenever a job is added to or removed from the Print Manager queue. 
        /// </summary>
        SPOOLERSTATUS = 0x002A,

        /// <summary>
        /// The WM_DRAWITEM message is sent to the parent window of an owner-drawn button, combo box, list box, or menu when a visual aspect of the button, combo box, list box, or menu has changed.
        /// </summary>
        DRAWITEM = 0x002B,

        /// <summary>
        /// The WM_MEASUREITEM message is sent to the owner window of a combo box, list box, list view control, or menu item when the control or menu is created.
        /// </summary>
        MEASUREITEM = 0x002C,

        /// <summary>
        /// Sent to the owner of a list box or combo box when the list box or combo box is destroyed or when items are removed by the LB_DELETESTRING, LB_RESETCONTENT, CB_DELETESTRING, or CB_RESETCONTENT message. The system sends a WM_DELETEITEM message for each deleted item. The system sends the WM_DELETEITEM message for any deleted list box or combo box item with nonzero item data.
        /// </summary>
        DELETEITEM = 0x002D,

        /// <summary>
        /// Sent by a list box with the LBS_WANTKEYBOARDINPUT style to its owner in response to a WM_KEYDOWN message. 
        /// </summary>
        VKEYTOITEM = 0x002E,

        /// <summary>
        /// Sent by a list box with the LBS_WANTKEYBOARDINPUT style to its owner in response to a WM_CHAR message. 
        /// </summary>
        CHARTOITEM = 0x002F,

        /// <summary>
        /// An application sends a WM_SETFONT message to specify the font that a control is to use when drawing text. 
        /// </summary>
        SETFONT = 0x0030,

        /// <summary>
        /// An application sends a WM_GETFONT message to a control to retrieve the font with which the control is currently drawing its text. 
        /// </summary>
        GETFONT = 0x0031,

        /// <summary>
        /// An application sends a WM_SETHOTKEY message to a window to associate a hot key with the window. When the user presses the hot key, the system activates the window. 
        /// </summary>
        SETHOTKEY = 0x0032,

        /// <summary>
        /// An application sends a WM_GETHOTKEY message to determine the hot key associated with a window. 
        /// </summary>
        GETHOTKEY = 0x0033,

        /// <summary>
        /// The WM_QUERYDRAGICON message is sent to a minimized (iconic) window. The window is about to be dragged by the user but does not have an icon defined for its class. An application can return a handle to an icon or cursor. The system displays this cursor or icon while the user drags the icon.
        /// </summary>
        QUERYDRAGICON = 0x0037,

        /// <summary>
        /// The system sends the WM_COMPAREITEM message to determine the relative position of a new item in the sorted list of an owner-drawn combo box or list box. Whenever the application adds a new item, the system sends this message to the owner of a combo box or list box created with the CBS_SORT or LBS_SORT style. 
        /// </summary>
        COMPAREITEM = 0x0039,

        /// <summary>
        /// Active Accessibility sends the WM_GETOBJECT message to obtain information about an accessible object contained in a server application. 
        /// Applications never send this message directly. It is sent only by Active Accessibility in response to calls to AccessibleObjectFromPoint, AccessibleObjectFromEvent, or AccessibleObjectFromWindow. However, server applications handle this message. 
        /// </summary>
        GETOBJECT = 0x003D,

        /// <summary>
        /// The WM_COMPACTING message is sent to all top-level windows when the system detects more than 12.5 percent of system time over a 30- to 60-second interval is being spent compacting memory. This indicates that system memory is low.
        /// </summary>
        COMPACTING = 0x0041,

        /// <summary>
        /// WM_COMMNOTIFY is Obsolete for Win32-Based Applications
        /// </summary>
        [Obsolete]
        COMMNOTIFY = 0x0044,

        /// <summary>
        /// The WM_WINDOWPOSCHANGING message is sent to a window whose size, position, or place in the Z order is about to change as a result of a call to the SetWindowPos function or another window-management function.
        /// </summary>
        WINDOWPOSCHANGING = 0x0046,

        /// <summary>
        /// The WM_WINDOWPOSCHANGED message is sent to a window whose size, position, or place in the Z order has changed as a result of a call to the SetWindowPos function or another window-management function.
        /// </summary>
        WINDOWPOSCHANGED = 0x0047,

        /// <summary>
        /// Notifies applications that the system, typically a battery-powered personal computer, is about to enter a suspended mode.
        /// Use: POWERBROADCAST
        /// </summary>
        [Obsolete]
        POWER = 0x0048,

        /// <summary>
        /// An application sends the WM_COPYDATA message to pass data to another application. 
        /// </summary>
        COPYDATA = 0x004A,

        /// <summary>
        /// The WM_CANCELJOURNAL message is posted to an application when a user cancels the application's journaling activities. The message is posted with a NULL window handle. 
        /// </summary>
        CANCELJOURNAL = 0x004B,

        /// <summary>
        /// Sent by a common control to its parent window when an event has occurred or the control requires some information. 
        /// </summary>
        NOTIFY = 0x004E,

        /// <summary>
        /// The WM_INPUTLANGCHANGEREQUEST message is posted to the window with the focus when the user chooses a new input language, either with the hotkey (specified in the Keyboard control panel application) or from the indicator on the system taskbar. An application can accept the change by passing the message to the DefWindowProc function or reject the change (and prevent it from taking place) by returning immediately. 
        /// </summary>
        INPUTLANGCHANGEREQUEST = 0x0050,

        /// <summary>
        /// The WM_INPUTLANGCHANGE message is sent to the topmost affected window after an application's input language has been changed. You should make any application-specific settings and pass the message to the DefWindowProc function, which passes the message to all first-level child windows. These child windows can pass the message to DefWindowProc to have it pass the message to their child windows, and so on. 
        /// </summary>
        INPUTLANGCHANGE = 0x0051,

        /// <summary>
        /// Sent to an application that has initiated a training card with Microsoft Windows Help. The message informs the application when the user clicks an authorable button. An application initiates a training card by specifying the HELP_TCARD command in a call to the WinHelp function.
        /// </summary>
        TCARD = 0x0052,

        /// <summary>
        /// Indicates that the user pressed the F1 key. If a menu is active when F1 is pressed, WM_HELP is sent to the window associated with the menu; otherwise, WM_HELP is sent to the window that has the keyboard focus. If no window has the keyboard focus, WM_HELP is sent to the currently active window. 
        /// </summary>
        HELP = 0x0053,

        /// <summary>
        /// The WM_USERCHANGED message is sent to all windows after the user has logged on or off. When the user logs on or off, the system updates the user-specific settings. The system sends this message immediately after updating the settings.
        /// </summary>
        USERCHANGED = 0x0054,

        /// <summary>
        /// Determines if a window accepts ANSI or Unicode structures in the WM_NOTIFY notification message. WM_NOTIFYFORMAT messages are sent from a common control to its parent window and from the parent window to the common control.
        /// </summary>
        NOTIFYFORMAT = 0x0055,

        /// <summary>
        /// The WM_CONTEXTMENU message notifies a window that the user clicked the right mouse button (right-clicked) in the window.
        /// </summary>
        CONTEXTMENU = 0x007B,

        /// <summary>
        /// The WM_STYLECHANGING message is sent to a window when the SetWindowLong function is about to change one or more of the window's styles.
        /// </summary>
        STYLECHANGING = 0x007C,

        /// <summary>
        /// The WM_STYLECHANGED message is sent to a window after the SetWindowLong function has changed one or more of the window's styles
        /// </summary>
        STYLECHANGED = 0x007D,

        /// <summary>
        /// The WM_DISPLAYCHANGE message is sent to all windows when the display resolution has changed.
        /// </summary>
        DISPLAYCHANGE = 0x007E,

        /// <summary>
        /// The WM_GETICON message is sent to a window to retrieve a handle to the large or small icon associated with a window. The system displays the large icon in the ALT+TAB dialog, and the small icon in the window caption. 
        /// </summary>
        GETICON = 0x007F,

        /// <summary>
        /// An application sends the WM_SETICON message to associate a new large or small icon with a window. The system displays the large icon in the ALT+TAB dialog box, and the small icon in the window caption. 
        /// </summary>
        SETICON = 0x0080,

        /// <summary>
        /// The WM_NCCREATE message is sent prior to the WM_CREATE message when a window is first created.
        /// </summary>
        NCCREATE = 0x0081,

        /// <summary>
        /// The WM_NCDESTROY message informs a window that its nonclient area is being destroyed. The DestroyWindow function sends the WM_NCDESTROY message to the window following the WM_DESTROY message. WM_DESTROY is used to free the allocated memory object associated with the window. 
        /// The WM_NCDESTROY message is sent after the child windows have been destroyed. In contrast, WM_DESTROY is sent before the child windows are destroyed.
        /// </summary>
        NCDESTROY = 0x0082,

        /// <summary>
        /// The WM_NCCALCSIZE message is sent when the size and position of a window's client area must be calculated. By processing this message, an application can control the content of the window's client area when the size or position of the window changes.
        /// </summary>
        NCCALCSIZE = 0x0083,

        /// <summary>
        /// The WM_NCHITTEST message is sent to a window when the cursor moves, or when a mouse button is pressed or released. If the mouse is not captured, the message is sent to the window beneath the cursor. Otherwise, the message is sent to the window that has captured the mouse.
        /// </summary>
        NCHITTEST = 0x0084,

        /// <summary>
        /// The WM_NCPAINT message is sent to a window when its frame must be painted. 
        /// </summary>
        NCPAINT = 0x0085,

        /// <summary>
        /// The WM_NCACTIVATE message is sent to a window when its nonclient area needs to be changed to indicate an active or inactive state.
        /// </summary>
        NCACTIVATE = 0x0086,

        /// <summary>
        /// The WM_GETDLGCODE message is sent to the window procedure associated with a control. By default, the system handles all keyboard input to the control; the system interprets certain types of keyboard input as dialog box navigation keys. To override this default behavior, the control can respond to the WM_GETDLGCODE message to indicate the types of input it wants to process itself.
        /// </summary>
        GETDLGCODE = 0x0087,

        /// <summary>
        /// The WM_SYNCPAINT message is used to synchronize painting while avoiding linking independent GUI threads.
        /// </summary>
        SYNCPAINT = 0x0088,

        /// <summary>
        /// The WM_NCMOUSEMOVE message is posted to a window when the cursor is moved within the nonclient area of the window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCMOUSEMOVE = 0x00A0,

        /// <summary>
        /// The WM_NCLBUTTONDOWN message is posted when the user presses the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCLBUTTONDOWN = 0x00A1,

        /// <summary>
        /// The WM_NCLBUTTONUP message is posted when the user releases the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCLBUTTONUP = 0x00A2,

        /// <summary>
        /// The WM_NCLBUTTONDBLCLK message is posted when the user double-clicks the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCLBUTTONDBLCLK = 0x00A3,

        /// <summary>
        /// The WM_NCRBUTTONDOWN message is posted when the user presses the right mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCRBUTTONDOWN = 0x00A4,

        /// <summary>
        /// The WM_NCRBUTTONUP message is posted when the user releases the right mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCRBUTTONUP = 0x00A5,

        /// <summary>
        /// The WM_NCRBUTTONDBLCLK message is posted when the user double-clicks the right mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCRBUTTONDBLCLK = 0x00A6,

        /// <summary>
        /// The WM_NCMBUTTONDOWN message is posted when the user presses the middle mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCMBUTTONDOWN = 0x00A7,

        /// <summary>
        /// The WM_NCMBUTTONUP message is posted when the user releases the middle mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCMBUTTONUP = 0x00A8,

        /// <summary>
        /// The WM_NCMBUTTONDBLCLK message is posted when the user double-clicks the middle mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCMBUTTONDBLCLK = 0x00A9,

        /// <summary>
        /// The WM_NCXBUTTONDOWN message is posted when the user presses the first or second X button while the cursor is in the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCXBUTTONDOWN = 0x00AB,

        /// <summary>
        /// The WM_NCXBUTTONUP message is posted when the user releases the first or second X button while the cursor is in the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCXBUTTONUP = 0x00AC,

        /// <summary>
        /// The WM_NCXBUTTONDBLCLK message is posted when the user double-clicks the first or second X button while the cursor is in the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCXBUTTONDBLCLK = 0x00AD,

        /// <summary>
        /// The WM_INPUT_DEVICE_CHANGE message is sent to the window that registered to receive raw input. A window receives this message through its WindowProc function.
        /// </summary>
        INPUT_DEVICE_CHANGE = 0x00FE,

        /// <summary>
        /// The WM_INPUT message is sent to the window that is getting raw input. 
        /// </summary>
        INPUT = 0x00FF,

        /// <summary>
        /// This message filters for keyboard messages.
        /// </summary>
        KEYFIRST = 0x0100,

        /// <summary>
        /// The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed. 
        /// </summary>
        KEYDOWN = 0x0100,

        /// <summary>
        /// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed, or a keyboard key that is pressed when a window has the keyboard focus. 
        /// </summary>
        KEYUP = 0x0101,

        /// <summary>
        /// The WM_CHAR message is posted to the window with the keyboard focus when a WM_KEYDOWN message is translated by the TranslateMessage function. The WM_CHAR message contains the character code of the key that was pressed. 
        /// </summary>
        CHAR = 0x0102,

        /// <summary>
        /// The WM_DEADCHAR message is posted to the window with the keyboard focus when a WM_KEYUP message is translated by the TranslateMessage function. WM_DEADCHAR specifies a character code generated by a dead key. A dead key is a key that generates a character, such as the umlaut (double-dot), that is combined with another character to form a composite character. For example, the umlaut-O character (Ö) is generated by typing the dead key for the umlaut character, and then typing the O key. 
        /// </summary>
        DEADCHAR = 0x0103,

        /// <summary>
        /// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user presses the F10 key (which activates the menu bar) or holds down the ALT key and then presses another key. It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that receives the message can distinguish between these two contexts by checking the context code in the lParam parameter. 
        /// </summary>
        SYSKEYDOWN = 0x0104,

        /// <summary>
        /// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user releases a key that was pressed while the ALT key was held down. It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent to the active window. The window that receives the message can distinguish between these two contexts by checking the context code in the lParam parameter. 
        /// </summary>
        SYSKEYUP = 0x0105,

        /// <summary>
        /// The WM_SYSCHAR message is posted to the window with the keyboard focus when a WM_SYSKEYDOWN message is translated by the TranslateMessage function. It specifies the character code of a system character key — that is, a character key that is pressed while the ALT key is down. 
        /// </summary>
        SYSCHAR = 0x0106,

        /// <summary>
        /// The WM_SYSDEADCHAR message is sent to the window with the keyboard focus when a WM_SYSKEYDOWN message is translated by the TranslateMessage function. WM_SYSDEADCHAR specifies the character code of a system dead key — that is, a dead key that is pressed while holding down the ALT key. 
        /// </summary>
        SYSDEADCHAR = 0x0107,

        /// <summary>
        /// The WM_UNICHAR message is posted to the window with the keyboard focus when a WM_KEYDOWN message is translated by the TranslateMessage function. The WM_UNICHAR message contains the character code of the key that was pressed. 
        /// The WM_UNICHAR message is equivalent to WM_CHAR, but it uses Unicode Transformation Format (UTF)-32, whereas WM_CHAR uses UTF-16. It is designed to send or post Unicode characters to ANSI windows and it can can handle Unicode Supplementary Plane characters.
        /// </summary>
        UNICHAR = 0x0109,

        /// <summary>
        /// This message filters for keyboard messages.
        /// </summary>
        KEYLAST = 0x0109,

        /// <summary>
        /// Sent immediately before the IME generates the composition string as a result of a keystroke. A window receives this message through its WindowProc function. 
        /// </summary>
        IME_STARTCOMPOSITION = 0x010D,

        /// <summary>
        /// Sent to an application when the IME ends composition. A window receives this message through its WindowProc function. 
        /// </summary>
        IME_ENDCOMPOSITION = 0x010E,

        /// <summary>
        /// Sent to an application when the IME changes composition status as a result of a keystroke. A window receives this message through its WindowProc function. 
        /// </summary>
        IME_COMPOSITION = 0x010F,
        IME_KEYLAST = 0x010F,

        /// <summary>
        /// The WM_INITDIALOG message is sent to the dialog box procedure immediately before a dialog box is displayed. Dialog box procedures typically use this message to initialize controls and carry out any other initialization tasks that affect the appearance of the dialog box. 
        /// </summary>
        INITDIALOG = 0x0110,

        /// <summary>
        /// The WM_COMMAND message is sent when the user selects a command item from a menu, when a control sends a notification message to its parent window, or when an accelerator keystroke is translated. 
        /// </summary>
        COMMAND = 0x0111,

        /// <summary>
        /// A window receives this message when the user chooses a command from the Window menu, clicks the maximize button, minimize button, restore button, close button, or moves the form. You can stop the form from moving by filtering this out.
        /// </summary>
        SYSCOMMAND = 0x0112,

        /// <summary>
        /// The WM_TIMER message is posted to the installing thread's message queue when a timer expires. The message is posted by the GetMessage or PeekMessage function. 
        /// </summary>
        TIMER = 0x0113,

        /// <summary>
        /// The WM_HSCROLL message is sent to a window when a scroll event occurs in the window's standard horizontal scroll bar. This message is also sent to the owner of a horizontal scroll bar control when a scroll event occurs in the control. 
        /// </summary>
        HSCROLL = 0x0114,

        /// <summary>
        /// The WM_VSCROLL message is sent to a window when a scroll event occurs in the window's standard vertical scroll bar. This message is also sent to the owner of a vertical scroll bar control when a scroll event occurs in the control. 
        /// </summary>
        VSCROLL = 0x0115,

        /// <summary>
        /// The WM_INITMENU message is sent when a menu is about to become active. It occurs when the user clicks an item on the menu bar or presses a menu key. This allows the application to modify the menu before it is displayed. 
        /// </summary>
        INITMENU = 0x0116,

        /// <summary>
        /// The WM_INITMENUPOPUP message is sent when a drop-down menu or submenu is about to become active. This allows an application to modify the menu before it is displayed, without changing the entire menu. 
        /// </summary>
        INITMENUPOPUP = 0x0117,

        /// <summary>
        /// The WM_MENUSELECT message is sent to a menu's owner window when the user selects a menu item. 
        /// </summary>
        MENUSELECT = 0x011F,

        /// <summary>
        /// The WM_MENUCHAR message is sent when a menu is active and the user presses a key that does not correspond to any mnemonic or accelerator key. This message is sent to the window that owns the menu. 
        /// </summary>
        MENUCHAR = 0x0120,

        /// <summary>
        /// The WM_ENTERIDLE message is sent to the owner window of a modal dialog box or menu that is entering an idle state. A modal dialog box or menu enters an idle state when no messages are waiting in its queue after it has processed one or more previous messages. 
        /// </summary>
        ENTERIDLE = 0x0121,

        /// <summary>
        /// The WM_MENURBUTTONUP message is sent when the user releases the right mouse button while the cursor is on a menu item. 
        /// </summary>
        MENURBUTTONUP = 0x0122,

        /// <summary>
        /// The WM_MENUDRAG message is sent to the owner of a drag-and-drop menu when the user drags a menu item. 
        /// </summary>
        MENUDRAG = 0x0123,

        /// <summary>
        /// The WM_MENUGETOBJECT message is sent to the owner of a drag-and-drop menu when the mouse cursor enters a menu item or moves from the center of the item to the top or bottom of the item. 
        /// </summary>
        MENUGETOBJECT = 0x0124,

        /// <summary>
        /// The WM_UNINITMENUPOPUP message is sent when a drop-down menu or submenu has been destroyed. 
        /// </summary>
        UNINITMENUPOPUP = 0x0125,

        /// <summary>
        /// The WM_MENUCOMMAND message is sent when the user makes a selection from a menu. 
        /// </summary>
        MENUCOMMAND = 0x0126,

        /// <summary>
        /// An application sends the WM_CHANGEUISTATE message to indicate that the user interface (UI) state should be changed.
        /// </summary>
        CHANGEUISTATE = 0x0127,

        /// <summary>
        /// An application sends the WM_UPDATEUISTATE message to change the user interface (UI) state for the specified window and all its child windows.
        /// </summary>
        UPDATEUISTATE = 0x0128,

        /// <summary>
        /// An application sends the WM_QUERYUISTATE message to retrieve the user interface (UI) state for a window.
        /// </summary>
        QUERYUISTATE = 0x0129,

        /// <summary>
        /// The WM_CTLCOLORMSGBOX message is sent to the owner window of a message box before Windows draws the message box. By responding to this message, the owner window can set the text and background colors of the message box by using the given display device context handle. 
        /// </summary>
        CTLCOLORMSGBOX = 0x0132,

        /// <summary>
        /// An edit control that is not read-only or disabled sends the WM_CTLCOLOREDIT message to its parent window when the control is about to be drawn. By responding to this message, the parent window can use the specified device context handle to set the text and background colors of the edit control. 
        /// </summary>
        CTLCOLOREDIT = 0x0133,

        /// <summary>
        /// Sent to the parent window of a list box before the system draws the list box. By responding to this message, the parent window can set the text and background colors of the list box by using the specified display device context handle. 
        /// </summary>
        CTLCOLORLISTBOX = 0x0134,

        /// <summary>
        /// The WM_CTLCOLORBTN message is sent to the parent window of a button before drawing the button. The parent window can change the button's text and background colors. However, only owner-drawn buttons respond to the parent window processing this message. 
        /// </summary>
        CTLCOLORBTN = 0x0135,

        /// <summary>
        /// The WM_CTLCOLORDLG message is sent to a dialog box before the system draws the dialog box. By responding to this message, the dialog box can set its text and background colors using the specified display device context handle. 
        /// </summary>
        CTLCOLORDLG = 0x0136,

        /// <summary>
        /// The WM_CTLCOLORSCROLLBAR message is sent to the parent window of a scroll bar control when the control is about to be drawn. By responding to this message, the parent window can use the display context handle to set the background color of the scroll bar control. 
        /// </summary>
        CTLCOLORSCROLLBAR = 0x0137,

        /// <summary>
        /// A static control, or an edit control that is read-only or disabled, sends the WM_CTLCOLORSTATIC message to its parent window when the control is about to be drawn. By responding to this message, the parent window can use the specified device context handle to set the text and background colors of the static control. 
        /// </summary>
        CTLCOLORSTATIC = 0x0138,

        /// <summary>
        /// Use WM_MOUSEFIRST to specify the first mouse message. Use the PeekMessage() Function.
        /// </summary>
        MOUSEFIRST = 0x0200,

        /// <summary>
        /// The WM_MOUSEMOVE message is posted to a window when the cursor moves. If the mouse is not captured, the message is posted to the window that contains the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        MOUSEMOVE = 0x0200,

        /// <summary>
        /// The WM_LBUTTONDOWN message is posted when the user presses the left mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        LBUTTONDOWN = 0x0201,

        /// <summary>
        /// The WM_LBUTTONUP message is posted when the user releases the left mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        LBUTTONUP = 0x0202,

        /// <summary>
        /// The WM_LBUTTONDBLCLK message is posted when the user double-clicks the left mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        LBUTTONDBLCLK = 0x0203,

        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        RBUTTONDOWN = 0x0204,

        /// <summary>
        /// The WM_RBUTTONUP message is posted when the user releases the right mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        RBUTTONUP = 0x0205,

        /// <summary>
        /// The WM_RBUTTONDBLCLK message is posted when the user double-clicks the right mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        RBUTTONDBLCLK = 0x0206,

        /// <summary>
        /// The WM_MBUTTONDOWN message is posted when the user presses the middle mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        MBUTTONDOWN = 0x0207,

        /// <summary>
        /// The WM_MBUTTONUP message is posted when the user releases the middle mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        MBUTTONUP = 0x0208,

        /// <summary>
        /// The WM_MBUTTONDBLCLK message is posted when the user double-clicks the middle mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        MBUTTONDBLCLK = 0x0209,

        /// <summary>
        /// The WM_MOUSEWHEEL message is sent to the focus window when the mouse wheel is rotated. The DefWindowProc function propagates the message to the window's parent. There should be no internal forwarding of the message, since DefWindowProc propagates it up the parent chain until it finds a window that processes it.
        /// </summary>
        MOUSEWHEEL = 0x020A,

        /// <summary>
        /// The WM_XBUTTONDOWN message is posted when the user presses the first or second X button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse. 
        /// </summary>
        XBUTTONDOWN = 0x020B,

        /// <summary>
        /// The WM_XBUTTONUP message is posted when the user releases the first or second X button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        XBUTTONUP = 0x020C,

        /// <summary>
        /// The WM_XBUTTONDBLCLK message is posted when the user double-clicks the first or second X button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        XBUTTONDBLCLK = 0x020D,

        /// <summary>
        /// The WM_MOUSEHWHEEL message is sent to the focus window when the mouse's horizontal scroll wheel is tilted or rotated. The DefWindowProc function propagates the message to the window's parent. There should be no internal forwarding of the message, since DefWindowProc propagates it up the parent chain until it finds a window that processes it.
        /// </summary>
        MOUSEHWHEEL = 0x020E,

        /// <summary>
        /// Use WM_MOUSELAST to specify the last mouse message. Used with PeekMessage() Function.
        /// </summary>
        MOUSELAST = 0x020E,

        /// <summary>
        /// The WM_PARENTNOTIFY message is sent to the parent of a child window when the child window is created or destroyed, or when the user clicks a mouse button while the cursor is over the child window. When the child window is being created, the system sends WM_PARENTNOTIFY just before the CreateWindow or CreateWindowEx function that creates the window returns. When the child window is being destroyed, the system sends the message before any processing to destroy the window takes place.
        /// </summary>
        PARENTNOTIFY = 0x0210,

        /// <summary>
        /// The WM_ENTERMENULOOP message informs an application's main window procedure that a menu modal loop has been entered. 
        /// </summary>
        ENTERMENULOOP = 0x0211,

        /// <summary>
        /// The WM_EXITMENULOOP message informs an application's main window procedure that a menu modal loop has been exited. 
        /// </summary>
        EXITMENULOOP = 0x0212,

        /// <summary>
        /// The WM_NEXTMENU message is sent to an application when the right or left arrow key is used to switch between the menu bar and the system menu. 
        /// </summary>
        NEXTMENU = 0x0213,

        /// <summary>
        /// The WM_SIZING message is sent to a window that the user is resizing. By processing this message, an application can monitor the size and position of the drag rectangle and, if needed, change its size or position. 
        /// </summary>
        SIZING = 0x0214,

        /// <summary>
        /// The WM_CAPTURECHANGED message is sent to the window that is losing the mouse capture.
        /// </summary>
        CAPTURECHANGED = 0x0215,

        /// <summary>
        /// The WM_MOVING message is sent to a window that the user is moving. By processing this message, an application can monitor the position of the drag rectangle and, if needed, change its position.
        /// </summary>
        MOVING = 0x0216,

        /// <summary>
        /// Notifies applications that a power-management event has occurred.
        /// </summary>
        POWERBROADCAST = 0x0218,

        /// <summary>
        /// Notifies an application of a change to the hardware configuration of a device or the computer.
        /// </summary>
        DEVICECHANGE = 0x0219,

        /// <summary>
        /// An application sends the WM_MDICREATE message to a multiple-document interface (MDI) client window to create an MDI child window. 
        /// </summary>
        MDICREATE = 0x0220,

        /// <summary>
        /// An application sends the WM_MDIDESTROY message to a multiple-document interface (MDI) client window to close an MDI child window. 
        /// </summary>
        MDIDESTROY = 0x0221,

        /// <summary>
        /// An application sends the WM_MDIACTIVATE message to a multiple-document interface (MDI) client window to instruct the client window to activate a different MDI child window. 
        /// </summary>
        MDIACTIVATE = 0x0222,

        /// <summary>
        /// An application sends the WM_MDIRESTORE message to a multiple-document interface (MDI) client window to restore an MDI child window from maximized or minimized size. 
        /// </summary>
        MDIRESTORE = 0x0223,

        /// <summary>
        /// An application sends the WM_MDINEXT message to a multiple-document interface (MDI) client window to activate the next or previous child window. 
        /// </summary>
        MDINEXT = 0x0224,

        /// <summary>
        /// An application sends the WM_MDIMAXIMIZE message to a multiple-document interface (MDI) client window to maximize an MDI child window. The system resizes the child window to make its client area fill the client window. The system places the child window's window menu icon in the rightmost position of the frame window's menu bar, and places the child window's restore icon in the leftmost position. The system also appends the title bar text of the child window to that of the frame window. 
        /// </summary>
        MDIMAXIMIZE = 0x0225,

        /// <summary>
        /// An application sends the WM_MDITILE message to a multiple-document interface (MDI) client window to arrange all of its MDI child windows in a tile format. 
        /// </summary>
        MDITILE = 0x0226,

        /// <summary>
        /// An application sends the WM_MDICASCADE message to a multiple-document interface (MDI) client window to arrange all its child windows in a cascade format. 
        /// </summary>
        MDICASCADE = 0x0227,

        /// <summary>
        /// An application sends the WM_MDIICONARRANGE message to a multiple-document interface (MDI) client window to arrange all minimized MDI child windows. It does not affect child windows that are not minimized. 
        /// </summary>
        MDIICONARRANGE = 0x0228,

        /// <summary>
        /// An application sends the WM_MDIGETACTIVE message to a multiple-document interface (MDI) client window to retrieve the handle to the active MDI child window. 
        /// </summary>
        MDIGETACTIVE = 0x0229,

        /// <summary>
        /// An application sends the WM_MDISETMENU message to a multiple-document interface (MDI) client window to replace the entire menu of an MDI frame window, to replace the window menu of the frame window, or both. 
        /// </summary>
        MDISETMENU = 0x0230,

        /// <summary>
        /// The WM_ENTERSIZEMOVE message is sent one time to a window after it enters the moving or sizing modal loop. The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns. 
        /// The system sends the WM_ENTERSIZEMOVE message regardless of whether the dragging of full windows is enabled.
        /// </summary>
        ENTERSIZEMOVE = 0x0231,

        /// <summary>
        /// The WM_EXITSIZEMOVE message is sent one time to a window, after it has exited the moving or sizing modal loop. The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns. 
        /// </summary>
        EXITSIZEMOVE = 0x0232,

        /// <summary>
        /// Sent when the user drops a file on the window of an application that has registered itself as a recipient of dropped files.
        /// </summary>
        DROPFILES = 0x0233,

        /// <summary>
        /// An application sends the WM_MDIREFRESHMENU message to a multiple-document interface (MDI) client window to refresh the window menu of the MDI frame window. 
        /// </summary>
        MDIREFRESHMENU = 0x0234,

        /// <summary>
        /// Sent to an application when a window is activated. A window receives this message through its WindowProc function. 
        /// </summary>
        IME_SETCONTEXT = 0x0281,

        /// <summary>
        /// Sent to an application to notify it of changes to the IME window. A window receives this message through its WindowProc function. 
        /// </summary>
        IME_NOTIFY = 0x0282,

        /// <summary>
        /// Sent by an application to direct the IME window to carry out the requested command. The application uses this message to control the IME window that it has created. To send this message, the application calls the SendMessage function with the following parameters.
        /// </summary>
        IME_CONTROL = 0x0283,

        /// <summary>
        /// Sent to an application when the IME window finds no space to extend the area for the composition window. A window receives this message through its WindowProc function. 
        /// </summary>
        IME_COMPOSITIONFULL = 0x0284,

        /// <summary>
        /// Sent to an application when the operating system is about to change the current IME. A window receives this message through its WindowProc function. 
        /// </summary>
        IME_SELECT = 0x0285,

        /// <summary>
        /// Sent to an application when the IME gets a character of the conversion result. A window receives this message through its WindowProc function. 
        /// </summary>
        IME_CHAR = 0x0286,

        /// <summary>
        /// Sent to an application to provide commands and request information. A window receives this message through its WindowProc function. 
        /// </summary>
        IME_REQUEST = 0x0288,

        /// <summary>
        /// Sent to an application by the IME to notify the application of a key press and to keep message order. A window receives this message through its WindowProc function. 
        /// </summary>
        IME_KEYDOWN = 0x0290,

        /// <summary>
        /// Sent to an application by the IME to notify the application of a key release and to keep message order. A window receives this message through its WindowProc function. 
        /// </summary>
        IME_KEYUP = 0x0291,

        /// <summary>
        /// The WM_MOUSEHOVER message is posted to a window when the cursor hovers over the client area of the window for the period of time specified in a prior call to TrackMouseEvent.
        /// </summary>
        MOUSEHOVER = 0x02A1,

        /// <summary>
        /// The WM_MOUSELEAVE message is posted to a window when the cursor leaves the client area of the window specified in a prior call to TrackMouseEvent.
        /// </summary>
        MOUSELEAVE = 0x02A3,

        /// <summary>
        /// The WM_NCMOUSEHOVER message is posted to a window when the cursor hovers over the nonclient area of the window for the period of time specified in a prior call to TrackMouseEvent.
        /// </summary>
        NCMOUSEHOVER = 0x02A0,

        /// <summary>
        /// The WM_NCMOUSELEAVE message is posted to a window when the cursor leaves the nonclient area of the window specified in a prior call to TrackMouseEvent.
        /// </summary>
        NCMOUSELEAVE = 0x02A2,

        /// <summary>
        /// The WM_WTSSESSION_CHANGE message notifies applications of changes in session state.
        /// </summary>
        WTSSESSION_CHANGE = 0x02B1,
        TABLET_FIRST = 0x02c0,
        TABLET_LAST = 0x02df,

        /// <summary>
        /// An application sends a WM_CUT message to an edit control or combo box to delete (cut) the current selection, if any, in the edit control and copy the deleted text to the clipboard in CF_TEXT format. 
        /// </summary>
        CUT = 0x0300,

        /// <summary>
        /// An application sends the WM_COPY message to an edit control or combo box to copy the current selection to the clipboard in CF_TEXT format. 
        /// </summary>
        COPY = 0x0301,

        /// <summary>
        /// An application sends a WM_PASTE message to an edit control or combo box to copy the current content of the clipboard to the edit control at the current caret position. Data is inserted only if the clipboard contains data in CF_TEXT format. 
        /// </summary>
        PASTE = 0x0302,

        /// <summary>
        /// An application sends a WM_CLEAR message to an edit control or combo box to delete (clear) the current selection, if any, from the edit control. 
        /// </summary>
        CLEAR = 0x0303,

        /// <summary>
        /// An application sends a WM_UNDO message to an edit control to undo the last operation. When this message is sent to an edit control, the previously deleted text is restored or the previously added text is deleted.
        /// </summary>
        UNDO = 0x0304,

        /// <summary>
        /// The WM_RENDERFORMAT message is sent to the clipboard owner if it has delayed rendering a specific clipboard format and if an application has requested data in that format. The clipboard owner must render data in the specified format and place it on the clipboard by calling the SetClipboardData function. 
        /// </summary>
        RENDERFORMAT = 0x0305,

        /// <summary>
        /// The WM_RENDERALLFORMATS message is sent to the clipboard owner before it is destroyed, if the clipboard owner has delayed rendering one or more clipboard formats. For the content of the clipboard to remain available to other applications, the clipboard owner must render data in all the formats it is capable of generating, and place the data on the clipboard by calling the SetClipboardData function. 
        /// </summary>
        RENDERALLFORMATS = 0x0306,

        /// <summary>
        /// The WM_DESTROYCLIPBOARD message is sent to the clipboard owner when a call to the EmptyClipboard function empties the clipboard. 
        /// </summary>
        DESTROYCLIPBOARD = 0x0307,

        /// <summary>
        /// The WM_DRAWCLIPBOARD message is sent to the first window in the clipboard viewer chain when the content of the clipboard changes. This enables a clipboard viewer window to display the new content of the clipboard. 
        /// </summary>
        DRAWCLIPBOARD = 0x0308,

        /// <summary>
        /// The WM_PAINTCLIPBOARD message is sent to the clipboard owner by a clipboard viewer window when the clipboard contains data in the CF_OWNERDISPLAY format and the clipboard viewer's client area needs repainting. 
        /// </summary>
        PAINTCLIPBOARD = 0x0309,

        /// <summary>
        /// The WM_VSCROLLCLIPBOARD message is sent to the clipboard owner by a clipboard viewer window when the clipboard contains data in the CF_OWNERDISPLAY format and an event occurs in the clipboard viewer's vertical scroll bar. The owner should scroll the clipboard image and update the scroll bar values. 
        /// </summary>
        VSCROLLCLIPBOARD = 0x030A,

        /// <summary>
        /// The WM_SIZECLIPBOARD message is sent to the clipboard owner by a clipboard viewer window when the clipboard contains data in the CF_OWNERDISPLAY format and the clipboard viewer's client area has changed size. 
        /// </summary>
        SIZECLIPBOARD = 0x030B,

        /// <summary>
        /// The WM_ASKCBFORMATNAME message is sent to the clipboard owner by a clipboard viewer window to request the name of a CF_OWNERDISPLAY clipboard format.
        /// </summary>
        ASKCBFORMATNAME = 0x030C,

        /// <summary>
        /// The WM_CHANGECBCHAIN message is sent to the first window in the clipboard viewer chain when a window is being removed from the chain. 
        /// </summary>
        CHANGECBCHAIN = 0x030D,

        /// <summary>
        /// The WM_HSCROLLCLIPBOARD message is sent to the clipboard owner by a clipboard viewer window. This occurs when the clipboard contains data in the CF_OWNERDISPLAY format and an event occurs in the clipboard viewer's horizontal scroll bar. The owner should scroll the clipboard image and update the scroll bar values. 
        /// </summary>
        HSCROLLCLIPBOARD = 0x030E,

        /// <summary>
        /// This message informs a window that it is about to receive the keyboard focus, giving the window the opportunity to realize its logical palette when it receives the focus. 
        /// </summary>
        QUERYNEWPALETTE = 0x030F,

        /// <summary>
        /// The WM_PALETTEISCHANGING message informs applications that an application is going to realize its logical palette. 
        /// </summary>
        PALETTEISCHANGING = 0x0310,

        /// <summary>
        /// This message is sent by the OS to all top-level and overlapped windows after the window with the keyboard focus realizes its logical palette. 
        /// This message enables windows that do not have the keyboard focus to realize their logical palettes and update their client areas.
        /// </summary>
        PALETTECHANGED = 0x0311,

        /// <summary>
        /// The WM_HOTKEY message is posted when the user presses a hot key registered by the RegisterHotKey function. The message is placed at the top of the message queue associated with the thread that registered the hot key. 
        /// </summary>
        HOTKEY = 0x0312,

        /// <summary>
        /// The WM_PRINT message is sent to a window to request that it draw itself in the specified device context, most commonly in a printer device context.
        /// </summary>
        PRINT = 0x0317,

        /// <summary>
        /// The WM_PRINTCLIENT message is sent to a window to request that it draw its client area in the specified device context, most commonly in a printer device context.
        /// </summary>
        PRINTCLIENT = 0x0318,

        /// <summary>
        /// The WM_APPCOMMAND message notifies a window that the user generated an application command event, for example, by clicking an application command button using the mouse or typing an application command key on the keyboard.
        /// </summary>
        APPCOMMAND = 0x0319,

        /// <summary>
        /// The WM_THEMECHANGED message is broadcast to every window following a theme change event. Examples of theme change events are the activation of a theme, the deactivation of a theme, or a transition from one theme to another.
        /// </summary>
        THEMECHANGED = 0x031A,

        /// <summary>
        /// Sent when the contents of the clipboard have changed.
        /// </summary>
        CLIPBOARDUPDATE = 0x031D,

        /// <summary>
        /// The system will send a window the WM_DWMCOMPOSITIONCHANGED message to indicate that the availability of desktop composition has changed.
        /// </summary>
        DWMCOMPOSITIONCHANGED = 0x031E,

        /// <summary>
        /// WM_DWMNCRENDERINGCHANGED is called when the non-client area rendering status of a window has changed. Only windows that have set the flag DWM_BLURBEHIND.fTransitionOnMaximized to true will get this message. 
        /// </summary>
        DWMNCRENDERINGCHANGED = 0x031F,

        /// <summary>
        /// Sent to all top-level windows when the colorization color has changed. 
        /// </summary>
        DWMCOLORIZATIONCOLORCHANGED = 0x0320,

        /// <summary>
        /// WM_DWMWINDOWMAXIMIZEDCHANGE will let you know when a DWM composed window is maximized. You also have to register for this message as well. You'd have other windowd go opaque when this message is sent.
        /// </summary>
        DWMWINDOWMAXIMIZEDCHANGE = 0x0321,

        /// <summary>
        /// Sent to request extended title bar information. A window receives this message through its WindowProc function.
        /// </summary>
        GETTITLEBARINFOEX = 0x033F,
        HANDHELDFIRST = 0x0358,
        HANDHELDLAST = 0x035F,
        AFXFIRST = 0x0360,
        AFXLAST = 0x037F,
        PENWINFIRST = 0x0380,
        PENWINLAST = 0x038F,

        /// <summary>
        /// The WM_APP constant is used by applications to help define private messages, usually of the form WM_APP+X, where X is an integer value. 
        /// </summary>
        APP = 0x8000,

        /// <summary>
        /// The WM_USER constant is used by applications to help define private messages for use by private window classes, usually of the form WM_USER+X, where X is an integer value. 
        /// </summary>
        USER = 0x0400,

        /// <summary>
        /// An application sends the WM_CPL_LAUNCH message to Windows Control Panel to request that a Control Panel application be started. 
        /// </summary>
        CPL_LAUNCH = USER + 0x1000,

        /// <summary>
        /// The WM_CPL_LAUNCHED message is sent when a Control Panel application, started by the WM_CPL_LAUNCH message, has closed. The WM_CPL_LAUNCHED message is sent to the window identified by the wParam parameter of the WM_CPL_LAUNCH message that started the application. 
        /// </summary>
        CPL_LAUNCHED = USER + 0x1001,

        /// <summary>
        /// WM_SYSTIMER is a well-known yet still undocumented message. Windows uses WM_SYSTIMER for internal actions like scrolling.
        /// </summary>
        SYSTIMER = 0x118
    }

    public enum HookType : int
    {
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD = 2,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;

        public int mouseData;
        // be careful, this must be ints, not uints (was wrong before I changed it...). regards, cmew.

        public int flags;
        public int time;
        public UIntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public KBDLLHOOKSTRUCTFlags flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    [Flags]
    public enum KBDLLHOOKSTRUCTFlags : uint
    {
        LLKHF_EXTENDED = 0x01,
        LLKHF_INJECTED = 0x10,
        LLKHF_ALTDOWN = 0x20,
        LLKHF_UP = 0x80,
    }

    [Flags]
    public enum AllocationType
    {
        Commit = 0x1000,
        Reserve = 0x2000,
        Decommit = 0x4000,
        Release = 0x8000,
        Reset = 0x80000,
        Physical = 0x400000,
        TopDown = 0x100000,
        WriteWatch = 0x200000,
        LargePages = 0x20000000
    }

    [Flags]
    public enum MemoryProtection
    {
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        GuardModifierflag = 0x100,
        NoCacheModifierflag = 0x200,
        WriteCombineModifierflag = 0x400
    }

    [Flags]
    public enum ProcessAccess
    {
        /// <summary>
        /// Required to create a thread.
        /// </summary>
        CreateThread = 0x0002,

        /// <summary>
        ///
        /// </summary>
        SetSessionId = 0x0004,

        /// <summary>
        /// Required to perform an operation on the address space of a process
        /// </summary>
        VmOperation = 0x0008,

        /// <summary>
        /// Required to read memory in a process using ReadProcessMemory.
        /// </summary>
        VmRead = 0x0010,

        /// <summary>
        /// Required to write to memory in a process using WriteProcessMemory.
        /// </summary>
        VmWrite = 0x0020,

        /// <summary>
        /// Required to duplicate a handle using DuplicateHandle.
        /// </summary>
        DupHandle = 0x0040,

        /// <summary>
        /// Required to create a process.
        /// </summary>
        CreateProcess = 0x0080,

        /// <summary>
        /// Required to set memory limits using SetProcessWorkingSetSize.
        /// </summary>
        SetQuota = 0x0100,

        /// <summary>
        /// Required to set certain information about a process, such as its priority class (see SetPriorityClass).
        /// </summary>
        SetInformation = 0x0200,

        /// <summary>
        /// Required to retrieve certain information about a process, such as its token, exit code, and priority class (see OpenProcessToken).
        /// </summary>
        QueryInformation = 0x0400,

        /// <summary>
        /// Required to suspend or resume a process.
        /// </summary>
        SuspendResume = 0x0800,

        /// <summary>
        /// Required to retrieve certain information about a process (see GetExitCodeProcess, GetPriorityClass, IsProcessInJob, QueryFullProcessImageName).
        /// A handle that has the PROCESS_QUERY_INFORMATION access right is automatically granted PROCESS_QUERY_LIMITED_INFORMATION.
        /// </summary>
        QueryLimitedInformation = 0x1000,

        /// <summary>
        /// Required to wait for the process to terminate using the wait functions.
        /// </summary>
        Synchronize = 0x100000,

        /// <summary>
        /// Required to delete the object.
        /// </summary>
        Delete = 0x00010000,

        /// <summary>
        /// Required to read information in the security descriptor for the object, not including the information in the SACL.
        /// To read or write the SACL, you must request the ACCESS_SYSTEM_SECURITY access right. For more information, see SACL Access Right.
        /// </summary>
        ReadControl = 0x00020000,

        /// <summary>
        /// Required to modify the DACL in the security descriptor for the object.
        /// </summary>
        WriteDac = 0x00040000,

        /// <summary>
        /// Required to change the owner in the security descriptor for the object.
        /// </summary>
        WriteOwner = 0x00080000,

        StandardRightsRequired = 0x000F0000,

        /// <summary>
        /// All possible access rights for a process object.
        /// </summary>
        AllAccess = StandardRightsRequired | Synchronize | 0xFFFF
    }

    [Flags]
    public enum ProcessAccessFlags : uint
    {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VMOperation = 0x00000008,
        VMRead = 0x00000010,
        VMWrite = 0x00000020,
        DupHandle = 0x00000040,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        Synchronize = 0x00100000
    }


    public enum ColorConstants : uint
    {
        CLR_NONE = 0xFFFFFFFFU,
        CLR_DEFAULT = 0xFF000000U
    }

    public enum InitWindowsCommonControlsConstants : int
    {
        ICC_LISTVIEW_CLASSES = 0x00000001, // listview, header
        ICC_TREEVIEW_CLASSES = 0x00000002, // treeview, tooltips
        ICC_BAR_CLASSES = 0x00000004, // toolbar, statusbar, trackbar, tooltips
        ICC_TAB_CLASSES = 0x00000008, // tab, tooltips
        ICC_UPDOWN_CLASS = 0x00000010, // updown
        ICC_PROGRESS_CLASS = 0x00000020, // progress
        ICC_HOTKEY_CLASS = 0x00000040, // hotkey
        ICC_ANIMATE_CLASS = 0x00000080, // animate
        ICC_WIN95_CLASSES = 0x000000FF,
        ICC_DATE_CLASSES = 0x00000100, // month picker, date picker, time picker, updown
        ICC_USEREX_CLASSES = 0x00000200, // comboex
        ICC_COOL_CLASSES = 0x00000400, // rebar (coolbar) control
        ICC_INTERNET_CLASSES = 0x00000800,
        ICC_PAGESCROLLER_CLASS = 0x00001000, // page scroller
        ICC_NATIVEFNTCTL_CLASS = 0x00002000, // native font control
        ICC_STANDARD_CLASSES = 0x00004000,
        ICC_LINK_CLASS = 0x00008000,
    }

    [Flags]
    public enum RebarBandStyleConstants : uint
    {
        RBBS_BREAK = 0x00000001, // break to new line
        RBBS_FIXEDSIZE = 0x00000002, // band can't be sized
        RBBS_CHILDEDGE = 0x00000004, // edge around top & bottom of child window
        RBBS_HIDDEN = 0x00000008, // don't show
        RBBS_NOVERT = 0x00000010, // don't show when vertical
        RBBS_FIXEDBMP = 0x00000020, // bitmap doesn't move during band resize
        RBBS_VARIABLEHEIGHT = 0x00000040, // allow autosizing of this child vertically
        RBBS_GRIPPERALWAYS = 0x00000080, // always show the gripper
        RBBS_NOGRIPPER = 0x00000100, // never show the gripper
        RBBS_USECHEVRON = 0x00000200, // display drop-down button for this band if it's sized smaller than ideal width
        RBBS_HIDETITLE = 0x00000400, // keep band title hidden
        RBBS_TOPALIGN = 0x00000800 // keep band title hidden
    }

    [Flags]
    public enum RebarBandInfoConstants : uint
    {
        RBBIM_STYLE = 0x00000001,
        RBBIM_COLORS = 0x00000002,
        RBBIM_TEXT = 0x00000004,
        RBBIM_IMAGE = 0x00000008,
        RBBIM_CHILD = 0x00000010,
        RBBIM_CHILDSIZE = 0x00000020,
        RBBIM_SIZE = 0x00000040,
        RBBIM_BACKGROUND = 0x00000080,
        RBBIM_ID = 0x00000100,
        RBBIM_IDEALSIZE = 0x00000200,
        RBBIM_LPARAM = 0x00000400,
        RBBIM_HEADERSIZE = 0x00000800  // control the size of the header
    }

    [Flags]
    public enum RebarImageListConstants : uint
    {
        RBIM_IMAGELIST = 0x00000001
    }

    [Flags]
    public enum RebarSizeToRectConstants : uint
    {
        RBSTR_CHANGERECT = 0x0001   // flags for RB_SIZETORECT
    }

    [Flags]
    public enum RedrawWindowConstants : uint
    {
        RDW_INVALIDATE = 0x0001,
        RDW_INTERNALPAINT = 0x0002,
        RDW_ERASE = 0x0004,
        RDW_VALIDATE = 0x0008,
        RDW_NOINTERNALPAINT = 0x0010,
        RDW_NOERASE = 0x0020,
        RDW_NOCHILDREN = 0x0040,
        RDW_ALLCHILDREN = 0x0080,
        RDW_UPDATENOW = 0x0100,
        RDW_ERASENOW = 0x0200,
        RDW_FRAME = 0x0400,
        RDW_NOFRAME = 0x0800
    }

    [Flags]
    public enum SetWindowPosConstants : uint
    {
        SWP_NOSIZE = 0x0001U,
        SWP_NOMOVE = 0x0002U,
        SWP_NOZORDER = 0x0004U,
        SWP_NOREDRAW = 0x0008U,
        SWP_NOACTIVATE = 0x0010U,
        SWP_FRAMECHANGED = 0x0020U,  /* The frame changed: send WM_NCCALCSIZE */
        SWP_SHOWWINDOW = 0x0040U,
        SWP_HIDEWINDOW = 0x0080U,
        SWP_NOCOPYBITS = 0x0100U,
        SWP_NOOWNERZORDER = 0x0200U,  /* Don't do owner Z ordering */
        SWP_NOSENDCHANGING = 0x0400U,  /* Don't send WM_WINDOWPOSCHANGING */
        SWP_DRAWFRAME = SWP_FRAMECHANGED,
        SWP_NOREPOSITION = SWP_NOOWNERZORDER,
        SWP_DEFERERASE = 0x2000U,
        SWP_ASYNCWINDOWPOS = 0x4000U,
        SWP_REDRAWONLY = (SWP_NOSIZE | SWP_NOMOVE |
            SWP_NOZORDER | SWP_NOACTIVATE | SWP_NOCOPYBITS |
            SWP_NOOWNERZORDER | SWP_NOSENDCHANGING)
    }

    public enum WindowsHitTestConstants : int
    {
        HTERROR = (-2),
        HTTRANSPARENT = (-1),
        HTNOWHERE = 0,
        HTCLIENT = 1,
        HTCAPTION = 2,
        HTSYSMENU = 3,
        HTGROWBOX = 4,
        HTSIZE = HTGROWBOX,
        HTMENU = 5,
        HTHSCROLL = 6,
        HTVSCROLL = 7,
        HTMINBUTTON = 8,
        HTMAXBUTTON = 9,
        HTLEFT = 10,
        HTRIGHT = 11,
        HTTOP = 12,
        HTTOPLEFT = 13,
        HTTOPRIGHT = 14,
        HTBOTTOM = 15,
        HTBOTTOMLEFT = 16,
        HTBOTTOMRIGHT = 17,
        HTBORDER = 18,
        HTREDUCE = HTMINBUTTON,
        HTZOOM = HTMAXBUTTON,
        HTSIZEFIRST = HTLEFT,
        HTSIZELAST = HTBOTTOMRIGHT,
        HTOBJECT = 19,
        HTCLOSE = 20,
        HTHELP = 21,
    }

    public enum WindowLongConstants : int
    {
        GWL_WNDPROC = (-4),
        GWL_HINSTANCE = (-6),
        GWL_HWNDPARENT = (-8),
        GWL_STYLE = (-16),
        GWL_EXSTYLE = (-20),
        GWL_USERDATA = (-21),
        GWL_ID = (-12),
        GWLP_WNDPROC = (-4),
        GWLP_HINSTANCE = (-6),
        GWLP_HWNDPARENT = (-8),
        GWLP_USERDATA = (-21),
        GWLP_ID = (-12),
    }

    public enum WindowsNotifyConstants : int
    {
        NM_FIRST = (0 - 0),       // generic to all controls
        NM_LAST = (0 - 99),
        LVN_FIRST = (0 - 100),       // listview
        LVN_LAST = (0 - 199),
        HDN_FIRST = (0 - 300),       // header
        HDN_LAST = (0 - 399),
        TVN_FIRST = (0 - 400),       // treeview
        TVN_LAST = (0 - 499),
        TTN_FIRST = (0 - 520),       // tooltips
        TTN_LAST = (0 - 549),
        TCN_FIRST = (0 - 550),       // tab control
        TCN_LAST = (0 - 580),
        CDN_FIRST = (0 - 601),       // common dialog (new)
        CDN_LAST = (0 - 699),
        TBN_FIRST = (0 - 700),       // toolbar
        TBN_LAST = (0 - 720),
        UDN_FIRST = (0 - 721),        // updown
        UDN_LAST = (0 - 740),
        MCN_FIRST = (0 - 750),       // monthcal
        MCN_LAST = (0 - 759),
        DTN_FIRST = (0 - 760),       // datetimepick
        DTN_LAST = (0 - 799),
        CBEN_FIRST = (0 - 800),       // combo box ex
        CBEN_LAST = (0 - 830),
        RBN_FIRST = (0 - 831),       // rebar
        RBN_LAST = (0 - 859),
        IPN_FIRST = (0 - 860),       // internet address
        IPN_LAST = (0 - 879),       // internet address
        SBN_FIRST = (0 - 880),       // status bar
        SBN_LAST = (0 - 899),
        PGN_FIRST = (0 - 900),       // Pager Control
        PGN_LAST = (0 - 950),
        WMN_FIRST = (0 - 1000),
        WMN_LAST = (0 - 1200),
        BCN_FIRST = (0 - 1250),
        BCN_LAST = (0 - 1350),
        NM_OUTOFMEMORY = (NM_FIRST - 1),
        NM_CLICK = (NM_FIRST - 2),    // uses NMCLICK struct
        NM_DBLCLK = (NM_FIRST - 3),
        NM_RETURN = (NM_FIRST - 4),
        NM_RCLICK = (NM_FIRST - 5),    // uses NMCLICK struct
        NM_RDBLCLK = (NM_FIRST - 6),
        NM_SETFOCUS = (NM_FIRST - 7),
        NM_KILLFOCUS = (NM_FIRST - 8),
        NM_CUSTOMDRAW = (NM_FIRST - 12),
        NM_HOVER = (NM_FIRST - 13),
        NM_NCHITTEST = (NM_FIRST - 14),   // uses NMMOUSE struct
        NM_KEYDOWN = (NM_FIRST - 15),   // uses NMKEY struct
        NM_RELEASEDCAPTURE = (NM_FIRST - 16),
        NM_SETCURSOR = (NM_FIRST - 17),   // uses NMMOUSE struct
        NM_CHAR = (NM_FIRST - 18),   // uses NMCHAR struct
        NM_TOOLTIPSCREATED = (NM_FIRST - 19),   // notify of when the tooltips window is create
        NM_LDOWN = (NM_FIRST - 20),
        NM_RDOWN = (NM_FIRST - 21),
        NM_THEMECHANGED = (NM_FIRST - 22),
        RBN_HEIGHTCHANGE = (RBN_FIRST - 0),
        RBN_GETOBJECT = (RBN_FIRST - 1),
        RBN_LAYOUTCHANGED = (RBN_FIRST - 2),
        RBN_AUTOSIZE = (RBN_FIRST - 3),
        RBN_BEGINDRAG = (RBN_FIRST - 4),
        RBN_ENDDRAG = (RBN_FIRST - 5),
        RBN_DELETINGBAND = (RBN_FIRST - 6),     // Uses NMREBAR
        RBN_DELETEDBAND = (RBN_FIRST - 7),     // Uses NMREBAR
        RBN_CHILDSIZE = (RBN_FIRST - 8),
        RBN_CHEVRONPUSHED = (RBN_FIRST - 10),
        RBN_MINMAX = (RBN_FIRST - 21),
        RBN_AUTOBREAK = (RBN_FIRST - 22),
    }
    /*
        public enum blah: int
        {
            ODT_HEADER = 100,
            ODT_TAB = 101,
            ODT_LISTVIEW = 102,
        }
    */

    [Flags]
    public enum WindowsStyleConstants : uint
    {
        CCS_TOP = 0x00000001U,
        CCS_NOMOVEY = 0x00000002U,
        CCS_BOTTOM = 0x00000003U,
        CCS_NORESIZE = 0x00000004U,
        CCS_NOPARENTALIGN = 0x00000008U,
        CCS_ADJUSTABLE = 0x00000020U,
        CCS_NODIVIDER = 0x00000040U,
        CCS_VERT = 0x00000080U,
        CCS_LEFT = (CCS_VERT | CCS_TOP),
        CCS_RIGHT = (CCS_VERT | CCS_BOTTOM),
        CCS_NOMOVEX = (CCS_VERT | CCS_NOMOVEY),
        RBS_TOOLTIPS = 0x0100,
        RBS_VARHEIGHT = 0x0200,
        RBS_BANDBORDERS = 0x0400,
        RBS_FIXEDORDER = 0x0800,
        RBS_REGISTERDROP = 0x1000,
        RBS_AUTOSIZE = 0x2000,
        RBS_VERTICALGRIPPER = 0x4000, // this always has the vertical gripper (default for horizontal mode)
        RBS_DBLCLKTOGGLE = 0x8000,
        WS_OVERLAPPED = 0x00000000U,
        WS_POPUP = 0x80000000U,
        WS_CHILD = 0x40000000U,
        WS_MINIMIZE = 0x20000000U,
        WS_VISIBLE = 0x10000000U,
        WS_DISABLED = 0x08000000U,
        WS_CLIPSIBLINGS = 0x04000000U,
        WS_CLIPCHILDREN = 0x02000000U,
        WS_MAXIMIZE = 0x01000000U,
        WS_CAPTION = 0x00C00000U,    /* WS_BORDER | WS_DLGFRAME  */
        WS_BORDER = 0x00800000U,
        WS_DLGFRAME = 0x00400000U,
        WS_VSCROLL = 0x00200000U,
        WS_HSCROLL = 0x00100000U,
        WS_SYSMENU = 0x00080000U,
        WS_THICKFRAME = 0x00040000U,
        WS_GROUP = 0x00020000U,
        WS_TABSTOP = 0x00010000U,
        WS_MINIMIZEBOX = 0x00020000U,
        WS_MAXIMIZEBOX = 0x00010000U,
        WS_TILED = WS_OVERLAPPED,
        WS_ICONIC = WS_MINIMIZE,
        WS_SIZEBOX = WS_THICKFRAME,
        WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
        WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED |
            WS_CAPTION |
            WS_SYSMENU |
            WS_THICKFRAME |
            WS_MINIMIZEBOX |
            WS_MAXIMIZEBOX),
        WS_POPUPWINDOW = (WS_POPUP |
            WS_BORDER |
            WS_SYSMENU),
        WS_CHILDWINDOW = (WS_CHILD)
    }

    public enum WindowZOrderConstants : int
    {
        HWND_TOP = 0,
        HWND_BOTTOM = 1,
        HWND_TOPMOST = -1,
        HWND_NOTOPMOST = -2
    }

    /// <summary>
    /// For use with ChildWindowFromPointEx
    /// </summary>
    [Flags]
    public enum WindowFromPointFlags
    {
        /// <summary>
        /// Does not skip any child windows
        /// </summary>
        CWP_ALL = 0x0000,
        /// <summary>
        /// Skips invisible child windows
        /// </summary>
        CWP_SKIPINVISIBLE = 0x0001,
        /// <summary>
        /// Skips disabled child windows
        /// </summary>
        CWP_SKIPDISABLED = 0x0002,
        /// <summary>
        /// Skips transparent child windows
        /// </summary>
        CWP_SKIPTRANSPARENT = 0x0004
    }

    public enum ShowWindowCommands : int
    {
        /// <summary>
        /// Hides the window and activates another window.
        /// </summary>
        Hide = 0,
        /// <summary>
        /// Activates and displays a window. If the window is minimized or
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when displaying the window
        /// for the first time.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Activates the window and displays it as a minimized window.
        /// </summary>
        ShowMinimized = 2,
        /// <summary>
        /// Maximizes the specified window.
        /// </summary>
        Maximize = 3, // is this the right value?
        /// <summary>
        /// Activates the window and displays it as a maximized window.
        /// </summary>      
        ShowMaximized = 3,
        /// <summary>
        /// Displays a window in its most recent size and position. This value
        /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except
        /// the window is not activated.
        /// </summary>
        ShowNoActivate = 4,
        /// <summary>
        /// Activates the window and displays it in its current size and position.
        /// </summary>
        Show = 5,
        /// <summary>
        /// Minimizes the specified window and activates the next top-level
        /// window in the Z order.
        /// </summary>
        Minimize = 6,
        /// <summary>
        /// Displays the window as a minimized window. This value is similar to
        /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the
        /// window is not activated.
        /// </summary>
        ShowMinNoActive = 7,
        /// <summary>
        /// Displays the window in its current size and position. This value is
        /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the
        /// window is not activated.
        /// </summary>
        ShowNA = 8,
        /// <summary>
        /// Activates and displays the window. If the window is minimized or
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when restoring a minimized window.
        /// </summary>
        Restore = 9,
        /// <summary>
        /// Sets the show state based on the SW_* value specified in the
        /// STARTUPINFO structure passed to the CreateProcess function by the
        /// program that started the application.
        /// </summary>
        ShowDefault = 10,
        /// <summary>
        ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
        /// that owns the window is not responding. This flag should only be
        /// used when minimizing windows from a different thread.
        /// </summary>
        ForceMinimize = 11
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct APPBARDATA
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uCallbackMessage;
        public uint uEdge;
        public RECT rc;
        public bool lParam;
    }

    /// <summary>
    /// The MONITORINFOEX structure contains information about a display monitor.
    /// The GetMonitorInfo function stores information into a MONITORINFOEX structure or a MONITORINFO structure.
    /// The MONITORINFOEX structure is a superset of the MONITORINFO structure. The MONITORINFOEX structure adds a string member to contain a name
    /// for the display monitor.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct MonitorInfoEx
    {

        // size of a device name string
        public const int CCHDEVICENAME = 32;

        /// <summary>
        /// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFOEX) (72) before calling the GetMonitorInfo function.
        /// Doing so lets the function determine the type of structure you are passing to it.
        /// </summary>
        public int Size;

        /// <summary>
        /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RectStruct Monitor;

        /// <summary>
        /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications,
        /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor.
        /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RectStruct WorkArea;

        /// <summary>
        /// The attributes of the display monitor.
        ///
        /// This member can be the following value:
        ///   1 : MONITORINFOF_PRIMARY
        /// </summary>
        public uint Flags;

        /// <summary>
        /// A string that specifies the device name of the monitor being used. Most applications have no use for a display monitor name,
        /// and so can save some bytes by using a MONITORINFO structure.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string DeviceName;

        public void Init()
        {
            this.Size = 40 + 2 * CCHDEVICENAME;
            this.DeviceName = string.Empty;
        }
    }

    /// <summary>
    /// The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
    /// </summary>
    /// <see cref="http://msdn.microsoft.com/en-us/library/dd162897%28VS.85%29.aspx"/>
    /// <remarks>
    /// By convention, the right and bottom edges of the rectangle are normally considered exclusive.
    /// In other words, the pixel whose coordinates are ( right, bottom ) lies immediately outside of the the rectangle.
    /// For example, when RECT is passed to the FillRect function, the rectangle is filled up to, but not including,
    /// the right column and bottom row of pixels. This structure is identical to the RECTL structure.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct RectStruct
    {
        /// <summary>
        /// The x-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public int Left;

        /// <summary>
        /// The y-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public int Top;

        /// <summary>
        /// The x-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public int Right;

        /// <summary>
        /// The y-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MonitorInfo
    {
        /// <summary>
        /// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFO) (40) before calling the GetMonitorInfo function.
        /// Doing so lets the function determine the type of structure you are passing to it.
        /// </summary>
        public int Size;

        /// <summary>
        /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RectStruct Monitor;

        /// <summary>
        /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications,
        /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor.
        /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RectStruct WorkArea;

        /// <summary>
        /// The attributes of the display monitor.
        ///
        /// This member can be the following value:
        ///   1 : MONITORINFOF_PRIMARY
        /// </summary>
        public uint Flags;

        public void Init()
        {
            this.Size = 40;
        }
    }

    public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

    public delegate IntPtr SubclassProc(
        IntPtr hWnd, uint uMsg, uint wParam, uint lParam, IntPtr uIdSubclass, IntPtr dwRefData);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

    public class User32Dll
    {
        private const string User32DllName = "user32.dll";
        private User32Dll() { }

        [DllImport(User32DllName, CharSet = CharSet.Auto)]
        public static extern uint SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

        [DllImport(User32DllName, CharSet = CharSet.Auto)]
        public static extern uint SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(User32DllName, CharSet = CharSet.Auto)]
        public static extern uint SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        [DllImport(User32DllName, CharSet = CharSet.Auto)]
        public static extern uint SendMessage(IntPtr hWnd, int Msg, int wParam, ref REBARINFO lParam);

        [DllImport(User32DllName, CharSet = CharSet.Auto)]
        public static extern uint SendMessage(IntPtr hWnd, int Msg, int wParam, ref REBARBANDINFO lParam);

        [DllImport(User32DllName, CharSet = CharSet.Auto)]
        public static extern uint SendMessage(IntPtr hWnd, int Msg, int wParam, ref COLORSCHEME lParam);

        [DllImport(User32DllName, CharSet = CharSet.Auto)]
        public static extern uint SendMessage(IntPtr hWnd, int Msg, int wParam, COLORREF lParam);

        [DllImport(User32DllName, CharSet = CharSet.Auto)]
        public static extern uint SendMessage(IntPtr hWnd, int Msg, int wParam, ref RECT lParam);

        [DllImport(User32DllName, CharSet = CharSet.Auto)]
        public static extern uint SendMessage(IntPtr hWnd, int Msg, int wParam, ref MARGINS lParam);

        [DllImport(User32DllName, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string className, string windowText);

        [DllImport(User32DllName, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);

        [DllImport(User32DllName, CharSet = CharSet.Auto)]
        public static extern IntPtr SetParent(IntPtr hwndChild, IntPtr hwndNewParent);

        [DllImport(User32DllName, SetLastError = true)]
        public static extern IntPtr GetThreadDesktop(uint dwThreadId);

        [DllImport(User32DllName, SetLastError = true)]
        public static extern bool GetUserObjectInformation(IntPtr hObj, int nIndex,
                                                           [Out] byte[] pvInfo, uint nLength, out uint lpnLengthNeeded);

        [DllImport(User32DllName, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport(User32DllName, SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // When you don't want the ProcessId, use this overload and pass IntPtr.Zero for the second parameter
        [DllImport(User32DllName)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(User32DllName, SetLastError = true)]
        public static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(User32DllName)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(User32DllName, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport(User32DllName)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        // overload for use with LowLevelKeyboardProc
        [DllImport(User32DllName)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, WM wParam, [In] KBDLLHOOKSTRUCT lParam);

        // overload for use with LowLevelMouseProc
        [DllImport(User32DllName)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, WM wParam, [In] MSLLHOOKSTRUCT lParam);

        [DllImport(User32DllName)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport(User32DllName)]
        public static extern bool GetWindowRect(IntPtr hWnd, [Out] RectWin r);

        [DllImport(User32DllName)]
        public static extern uint RegisterWindowMessage(string Name);

        [DllImport(User32DllName)]
        public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport(User32DllName, SetLastError = true)]
        public static extern int MapWindowPoints(IntPtr hwndFrom, IntPtr hwndTo, ref POINT lpPoints, [MarshalAs(UnmanagedType.U4)] int cPoints);

        [DllImport(User32DllName, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport(User32DllName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport(User32DllName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumThreadWindows(uint dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport(User32DllName)]
        public static extern uint GetMessagePos();

        [DllImport(User32DllName)]
        public static extern IntPtr ChildWindowFromPointEx(IntPtr hWndParent, POINT pt, uint uFlags);

        [DllImport(User32DllName)]
        public static extern bool IntersectRect(out RECT lprcDst, [In] ref RECT lprcSrc1,
           [In] ref RECT lprcSrc2);

        [DllImport(User32DllName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

        [DllImport(User32DllName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport(User32DllName)]
        public static extern IntPtr MonitorFromRect([In] ref RECT lprc, uint dwFlags);

        [DllImport(User32DllName, CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

        [DllImport(User32DllName)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);
    }	// class User32Dll

    public class Kernel32Dll
    {
        private const string Kernel32DllName = "kernel32.dll";
        private Kernel32Dll() { }

        [DllImport(Kernel32DllName, CharSet = CharSet.Auto)]
        public static extern uint GetLastError();

        [DllImport(Kernel32DllName, CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport(Kernel32DllName, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport(Kernel32DllName, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
           uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport(Kernel32DllName, SetLastError = true)]
        public static extern bool ReadProcessMemory(
          IntPtr hProcess,
          IntPtr lpBaseAddress,
          [Out] byte[] lpBuffer,
          int dwSize,
          out int lpNumberOfBytesRead
         );

        [DllImport(Kernel32DllName, SetLastError = true)]
        public static extern bool ReadProcessMemory(
         IntPtr hProcess,
         IntPtr lpBaseAddress,
         [Out, MarshalAs(UnmanagedType.AsAny)] object lpBuffer,
         int dwSize,
         out int lpNumberOfBytesRead
        );

        [DllImport(Kernel32DllName, SetLastError = true)]
        public static extern bool ReadProcessMemory(
         IntPtr hProcess,
         IntPtr lpBaseAddress,
         IntPtr lpBuffer,
         int dwSize,
         out int lpNumberOfBytesRead
        );

        [DllImport(Kernel32DllName, SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
            byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport(Kernel32DllName)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport(Kernel32DllName)]
        public static extern uint GetVersion();
    }

    public class Comctl32Dll
    {
        private const string Comctl32DllName = "comctl32.dll";
        private Comctl32Dll() { }

        [DllImport(Comctl32DllName)]
        public static extern bool SetWindowSubclass(IntPtr hWnd, SubclassProc pfnSubclass, IntPtr uIdSubclass, IntPtr dwRefData);
    }

    public class Gdi32Dll
    {
        private const string Gdi32DllName = "gdi32.dll";
        private Gdi32Dll() { }

        [DllImport(Gdi32DllName)]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect,
           int nBottomRect);

        [DllImport(Gdi32DllName)]
        public static extern bool FillRgn(IntPtr hdc, IntPtr hrgn, IntPtr hbr);

        [DllImport(Gdi32DllName)]
        public static extern IntPtr CreateSolidBrush(uint crColor);

        [DllImport(Gdi32DllName)]
        public static extern bool DeleteObject(IntPtr hObject);
    }

    public class Shell32Dll
    {
        private const string Shell32DllName = "shell32.dll";
        private Shell32Dll() { }

        [DllImport(Shell32DllName)]
        public static extern IntPtr SHAppBarMessage(uint dwMessage,
           [In] ref APPBARDATA pData);
    }
}
