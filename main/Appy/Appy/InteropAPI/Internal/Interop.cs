using Accessibility;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class IconsSize
    {
        public const int LARGE = 0;
        public const int SMALL = 1;
    }

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
    public struct LASTINPUTINFO
    {
        public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 cbSize;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dwTime;
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

    public class HresultCodes
    {
        public const uint S_OK = 0x00000000;
        public const uint E_ABORT = 0x80004004;
        public const uint E_ACCESSDENIED = 0x80070005;
        public const uint E_FAIL = 0x80004005;
        public const uint E_HANDLE = 0x80070006;
        public const uint E_INVALIDARG = 0x80070057;
        public const uint E_NOINTERFACE = 0x80004002;
        public const uint E_NOTIMPL = 0x80004001;
        public const uint E_OUTOFMEMORY = 0x8007000E;
        public const uint E_POINTER = 0x80004003;
        public const uint E_UNEXPECTED = 0x8000FFFF;
    }

    [Flags]
    public enum WinEventHookFlags : uint
    {
        WINEVENT_OUTOFCONTEXT   = 0x0000,  // Events are ASYNC
        WINEVENT_SKIPOWNTHREAD  = 0x0001,  // Don't call back for events on installer's thread
        WINEVENT_SKIPOWNPROCESS = 0x0002,  // Don't call back for events on installer's process
        WINEVENT_INCONTEXT      = 0x0004  // Events are SYNC, this causes your dll to be injected into every process
    }

    public enum EventConstants : uint
    {
        EVENT_MIN = 0x00000001,
        EVENT_MAX = 0x7FFFFFFF,
        EVENT_SYSTEM_SOUND = 0x0001,
        EVENT_SYSTEM_ALERT = 0x0002,
        EVENT_SYSTEM_FOREGROUND = 0x0003,
        EVENT_SYSTEM_MENUSTART = 0x0004,
        EVENT_SYSTEM_MENUEND = 0x0005,
        EVENT_SYSTEM_MENUPOPUPSTART = 0x0006,
        EVENT_SYSTEM_MENUPOPUPEND = 0x0007,
        EVENT_SYSTEM_CAPTURESTART = 0x0008,
        EVENT_SYSTEM_CAPTUREEND = 0x0009,
        EVENT_SYSTEM_MOVESIZESTART = 0x000A,
        EVENT_SYSTEM_MOVESIZEEND = 0x000B,
        EVENT_SYSTEM_CONTEXTHELPSTART = 0x000C,
        EVENT_SYSTEM_CONTEXTHELPEND = 0x000D,
        EVENT_SYSTEM_DRAGDROPSTART = 0x000E,
        EVENT_SYSTEM_DRAGDROPEND = 0x000F,
        EVENT_SYSTEM_DIALOGSTART = 0x0010,
        EVENT_SYSTEM_DIALOGEND = 0x0011,
        EVENT_SYSTEM_SCROLLINGSTART = 0x0012,
        EVENT_SYSTEM_SCROLLINGEND = 0x0013,
        EVENT_SYSTEM_SWITCHSTART = 0x0014,
        EVENT_SYSTEM_SWITCHEND = 0x0015,
        EVENT_SYSTEM_MINIMIZESTART = 0x0016,
        EVENT_SYSTEM_MINIMIZEEND = 0x0017,
        EVENT_SYSTEM_DESKTOPSWITCH = 0x0020,
        EVENT_SYSTEM_END = 0x00FF,
        EVENT_OEM_DEFINED_START = 0x0101,
        EVENT_OEM_DEFINED_END = 0x01FF,
        EVENT_UIA_EVENTID_START = 0x4E00,
        EVENT_UIA_EVENTID_END = 0x4EFF,
        EVENT_UIA_PROPID_START = 0x7500,
        EVENT_UIA_PROPID_END = 0x75FF,
        EVENT_CONSOLE_CARET = 0x4001,
        EVENT_CONSOLE_UPDATE_REGION = 0x4002,
        EVENT_CONSOLE_UPDATE_SIMPLE = 0x4003,
        EVENT_CONSOLE_UPDATE_SCROLL = 0x4004,
        EVENT_CONSOLE_LAYOUT = 0x4005,
        EVENT_CONSOLE_START_APPLICATION = 0x4006,
        EVENT_CONSOLE_END_APPLICATION = 0x4007,
        CONSOLE_APPLICATION_16BIT = 0x0000,
        CONSOLE_CARET_SELECTION = 0x0001,
        CONSOLE_CARET_VISIBLE = 0x0002,
        EVENT_CONSOLE_END = 0x40FF,
        EVENT_OBJECT_CREATE = 0x8000,  // hwnd + ID + idChild is created item
        EVENT_OBJECT_DESTROY = 0x8001,  // hwnd + ID + idChild is destroyed item
        EVENT_OBJECT_SHOW = 0x8002,  // hwnd + ID + idChild is shown item
        EVENT_OBJECT_HIDE = 0x8003,  // hwnd + ID + idChild is hidden item
        EVENT_OBJECT_REORDER = 0x8004,  // hwnd + ID + idChild is parent of zordering children
        EVENT_OBJECT_FOCUS = 0x8005,  // hwnd + ID + idChild is focused item
        EVENT_OBJECT_SELECTION = 0x8006,  // hwnd + ID + idChild is selected item (if only one), or idChild is OBJID_WINDOW if complex
        EVENT_OBJECT_SELECTIONADD = 0x8007,  // hwnd + ID + idChild is item added
        EVENT_OBJECT_SELECTIONREMOVE = 0x8008,  // hwnd + ID + idChild is item removed
        EVENT_OBJECT_SELECTIONWITHIN = 0x8009,  // hwnd + ID + idChild is parent of changed selected items
        EVENT_OBJECT_STATECHANGE = 0x800A,  // hwnd + ID + idChild is item w/ state change
        EVENT_OBJECT_LOCATIONCHANGE = 0x800B,  // hwnd + ID + idChild is moved/sized item
        EVENT_OBJECT_NAMECHANGE = 0x800C,  // hwnd + ID + idChild is item w/ name change
        EVENT_OBJECT_DESCRIPTIONCHANGE = 0x800D,  // hwnd + ID + idChild is item w/ desc change
        EVENT_OBJECT_VALUECHANGE = 0x800E,  // hwnd + ID + idChild is item w/ value change
        EVENT_OBJECT_PARENTCHANGE = 0x800F,  // hwnd + ID + idChild is item w/ new parent
        EVENT_OBJECT_HELPCHANGE = 0x8010,  // hwnd + ID + idChild is item w/ help change
        EVENT_OBJECT_DEFACTIONCHANGE = 0x8011,  // hwnd + ID + idChild is item w/ def action change
        EVENT_OBJECT_ACCELERATORCHANGE = 0x8012,  // hwnd + ID + idChild is item w/ keybd accel change
        EVENT_OBJECT_INVOKED = 0x8013,  // hwnd + ID + idChild is item invoked
        EVENT_OBJECT_TEXTSELECTIONCHANGED = 0x8014,  // hwnd + ID + idChild is item w? test selection change
        EVENT_OBJECT_CONTENTSCROLLED = 0x8015,
        EVENT_SYSTEM_ARRANGMENTPREVIEW = 0x8016,
        EVENT_OBJECT_END = 0x80FF,
        EVENT_AIA_START = 0xA000,
        EVENT_AIA_END = 0xAFFF
    }

    public enum OBJID : uint
    {
        WINDOW = 0x00000000,
        SYSMENU = 0xFFFFFFFF,
        TITLEBAR = 0xFFFFFFFE,
        MENU = 0xFFFFFFFD,
        CLIENT = 0xFFFFFFFC,
        VSCROLL = 0xFFFFFFFB,
        HSCROLL = 0xFFFFFFFA,
        SIZEGRIP = 0xFFFFFFF9,
        CARET = 0xFFFFFFF8,
        CURSOR = 0xFFFFFFF7,
        ALERT = 0xFFFFFFF6,
        SOUND = 0xFFFFFFF5,
    }

    public static class ChildIds
    {
        public static int CHILDID_SELF = 0x00;
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

    public enum WM : uint
    {
        NULL = 0x0000,
        CREATE = 0x0001,
        DESTROY = 0x0002,
        MOVE = 0x0003,
        SIZE = 0x0005,
        ACTIVATE = 0x0006,
        SETFOCUS = 0x0007,
        KILLFOCUS = 0x0008,
        ENABLE = 0x000A,
        SETREDRAW = 0x000B,
        SETTEXT = 0x000C,
        GETTEXT = 0x000D,
        GETTEXTLENGTH = 0x000E,
        PAINT = 0x000F,
        CLOSE = 0x0010,
        QUERYENDSESSION = 0x0011,
        QUERYOPEN = 0x0013,
        ENDSESSION = 0x0016,
        QUIT = 0x0012,
        ERASEBKGND = 0x0014,
        SYSCOLORCHANGE = 0x0015,
        SHOWWINDOW = 0x0018,
        WININICHANGE = 0x001A,
        SETTINGCHANGE = WININICHANGE,
        DEVMODECHANGE = 0x001B,
        ACTIVATEAPP = 0x001C,
        FONTCHANGE = 0x001D,
        TIMECHANGE = 0x001E,
        CANCELMODE = 0x001F,
        SETCURSOR = 0x0020,
        MOUSEACTIVATE = 0x0021,
        CHILDACTIVATE = 0x0022,
        QUEUESYNC = 0x0023,
        GETMINMAXINFO = 0x0024,
        PAINTICON = 0x0026,
        ICONERASEBKGND = 0x0027,
        NEXTDLGCTL = 0x0028,
        SPOOLERSTATUS = 0x002A,
        DRAWITEM = 0x002B,
        MEASUREITEM = 0x002C,
        DELETEITEM = 0x002D,
        VKEYTOITEM = 0x002E,
        CHARTOITEM = 0x002F,
        SETFONT = 0x0030,
        GETFONT = 0x0031,
        SETHOTKEY = 0x0032,
        GETHOTKEY = 0x0033,
        QUERYDRAGICON = 0x0037,
        COMPAREITEM = 0x0039,
        GETOBJECT = 0x003D,
        COMPACTING = 0x0041,
        [Obsolete]
        COMMNOTIFY = 0x0044,
        WINDOWPOSCHANGING = 0x0046,
        WINDOWPOSCHANGED = 0x0047,
        [Obsolete]
        POWER = 0x0048,
        COPYDATA = 0x004A,
        CANCELJOURNAL = 0x004B,
        NOTIFY = 0x004E,
        INPUTLANGCHANGEREQUEST = 0x0050,
        INPUTLANGCHANGE = 0x0051,
        TCARD = 0x0052,
        HELP = 0x0053,
        USERCHANGED = 0x0054,
        NOTIFYFORMAT = 0x0055,
        CONTEXTMENU = 0x007B,
        STYLECHANGING = 0x007C,
        STYLECHANGED = 0x007D,
        DISPLAYCHANGE = 0x007E,
        GETICON = 0x007F,
        SETICON = 0x0080,
        NCCREATE = 0x0081,
        NCDESTROY = 0x0082,
        NCCALCSIZE = 0x0083,
        NCHITTEST = 0x0084,
        NCPAINT = 0x0085,
        NCACTIVATE = 0x0086,
        GETDLGCODE = 0x0087,
        SYNCPAINT = 0x0088,
        NCMOUSEMOVE = 0x00A0,
        NCLBUTTONDOWN = 0x00A1,
        NCLBUTTONUP = 0x00A2,
        NCLBUTTONDBLCLK = 0x00A3,
        NCRBUTTONDOWN = 0x00A4,
        NCRBUTTONUP = 0x00A5,
        NCRBUTTONDBLCLK = 0x00A6,
        NCMBUTTONDOWN = 0x00A7,
        NCMBUTTONUP = 0x00A8,
        NCMBUTTONDBLCLK = 0x00A9,
        NCXBUTTONDOWN = 0x00AB,
        NCXBUTTONUP = 0x00AC,
        NCXBUTTONDBLCLK = 0x00AD,
        INPUT_DEVICE_CHANGE = 0x00FE,
        INPUT = 0x00FF,
        KEYFIRST = 0x0100,
        KEYDOWN = 0x0100,
        KEYUP = 0x0101,
        CHAR = 0x0102,
        DEADCHAR = 0x0103,
        SYSKEYDOWN = 0x0104,
        SYSKEYUP = 0x0105,
        SYSCHAR = 0x0106,
        SYSDEADCHAR = 0x0107,
        UNICHAR = 0x0109,
        KEYLAST = 0x0109,
        IME_STARTCOMPOSITION = 0x010D,
        IME_ENDCOMPOSITION = 0x010E,
        IME_COMPOSITION = 0x010F,
        IME_KEYLAST = 0x010F,
        INITDIALOG = 0x0110,
        COMMAND = 0x0111,
        SYSCOMMAND = 0x0112,
        TIMER = 0x0113,
        HSCROLL = 0x0114,
        VSCROLL = 0x0115,
        INITMENU = 0x0116,
        INITMENUPOPUP = 0x0117,
        MENUSELECT = 0x011F,
        MENUCHAR = 0x0120,
        ENTERIDLE = 0x0121,
        MENURBUTTONUP = 0x0122,
        MENUDRAG = 0x0123,
        MENUGETOBJECT = 0x0124,
        UNINITMENUPOPUP = 0x0125,
        MENUCOMMAND = 0x0126,
        CHANGEUISTATE = 0x0127,
        UPDATEUISTATE = 0x0128,
        QUERYUISTATE = 0x0129,
        CTLCOLORMSGBOX = 0x0132,
        CTLCOLOREDIT = 0x0133,
        CTLCOLORLISTBOX = 0x0134,
        CTLCOLORBTN = 0x0135,
        CTLCOLORDLG = 0x0136,
        CTLCOLORSCROLLBAR = 0x0137,
        CTLCOLORSTATIC = 0x0138,
        MOUSEFIRST = 0x0200,
        MOUSEMOVE = 0x0200,
        LBUTTONDOWN = 0x0201,
        LBUTTONUP = 0x0202,
        LBUTTONDBLCLK = 0x0203,
        RBUTTONDOWN = 0x0204,
        RBUTTONUP = 0x0205,
        RBUTTONDBLCLK = 0x0206,
        MBUTTONDOWN = 0x0207,
        MBUTTONUP = 0x0208,
        MBUTTONDBLCLK = 0x0209,
        MOUSEWHEEL = 0x020A,
        XBUTTONDOWN = 0x020B,
        XBUTTONUP = 0x020C,
        XBUTTONDBLCLK = 0x020D,
        MOUSEHWHEEL = 0x020E,
        MOUSELAST = 0x020E,
        PARENTNOTIFY = 0x0210,
        ENTERMENULOOP = 0x0211,
        EXITMENULOOP = 0x0212,
        NEXTMENU = 0x0213,
        SIZING = 0x0214,
        CAPTURECHANGED = 0x0215,
        MOVING = 0x0216,
        POWERBROADCAST = 0x0218,
        DEVICECHANGE = 0x0219,
        MDICREATE = 0x0220,
        MDIDESTROY = 0x0221,
        MDIACTIVATE = 0x0222,
        MDIRESTORE = 0x0223,
        MDINEXT = 0x0224,
        MDIMAXIMIZE = 0x0225,
        MDITILE = 0x0226,
        MDICASCADE = 0x0227,
        MDIICONARRANGE = 0x0228,
        MDIGETACTIVE = 0x0229,
        MDISETMENU = 0x0230,
        ENTERSIZEMOVE = 0x0231,
        EXITSIZEMOVE = 0x0232,
        DROPFILES = 0x0233,
        MDIREFRESHMENU = 0x0234,
        IME_SETCONTEXT = 0x0281,
        IME_NOTIFY = 0x0282,
        IME_CONTROL = 0x0283,
        IME_COMPOSITIONFULL = 0x0284,
        IME_SELECT = 0x0285,
        IME_CHAR = 0x0286,
        IME_REQUEST = 0x0288,
        IME_KEYDOWN = 0x0290,
        IME_KEYUP = 0x0291,
        MOUSEHOVER = 0x02A1,
        MOUSELEAVE = 0x02A3,
        NCMOUSEHOVER = 0x02A0,
        NCMOUSELEAVE = 0x02A2,
        WTSSESSION_CHANGE = 0x02B1,
        TABLET_FIRST = 0x02c0,
        TABLET_LAST = 0x02df,
        CUT = 0x0300,
        COPY = 0x0301,
        PASTE = 0x0302,
        CLEAR = 0x0303,
        UNDO = 0x0304,
        RENDERFORMAT = 0x0305,
        RENDERALLFORMATS = 0x0306,
        DESTROYCLIPBOARD = 0x0307,
        DRAWCLIPBOARD = 0x0308,
        PAINTCLIPBOARD = 0x0309,
        VSCROLLCLIPBOARD = 0x030A,
        SIZECLIPBOARD = 0x030B,
        ASKCBFORMATNAME = 0x030C,
        CHANGECBCHAIN = 0x030D,
        HSCROLLCLIPBOARD = 0x030E,
        QUERYNEWPALETTE = 0x030F,
        PALETTEISCHANGING = 0x0310,
        PALETTECHANGED = 0x0311,
        HOTKEY = 0x0312,
        PRINT = 0x0317,
        PRINTCLIENT = 0x0318,
        APPCOMMAND = 0x0319,
        THEMECHANGED = 0x031A,
        CLIPBOARDUPDATE = 0x031D,
        DWMCOMPOSITIONCHANGED = 0x031E,
        DWMNCRENDERINGCHANGED = 0x031F,
        DWMCOLORIZATIONCOLORCHANGED = 0x0320,
        DWMWINDOWMAXIMIZEDCHANGE = 0x0321,
        GETTITLEBARINFOEX = 0x033F,
        HANDHELDFIRST = 0x0358,
        HANDHELDLAST = 0x035F,
        AFXFIRST = 0x0360,
        AFXLAST = 0x037F,
        PENWINFIRST = 0x0380,
        PENWINLAST = 0x038F,
        APP = 0x8000,
        USER = 0x0400,
        CPL_LAUNCH = USER + 0x1000,
        CPL_LAUNCHED = USER + 0x1001,
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
        CreateThread = 0x0002,
        SetSessionId = 0x0004,
        VmOperation = 0x0008,
        VmRead = 0x0010,
        VmWrite = 0x0020,
        DupHandle = 0x0040,
        CreateProcess = 0x0080,
        SetQuota = 0x0100,
        SetInformation = 0x0200,
        QueryInformation = 0x0400,
        SuspendResume = 0x0800,
        QueryLimitedInformation = 0x1000,
        Synchronize = 0x100000,
        Delete = 0x00010000,
        ReadControl = 0x00020000,
        WriteDac = 0x00040000,
        WriteOwner = 0x00080000,
        StandardRightsRequired = 0x000F0000,

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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class WINDOWPOS
    {
      public IntPtr hwnd;
      public IntPtr hwndInsertAfter;
      public int x;
      public int y;
      public int cx;
      public int cy;
      public uint flags;
    }

    public enum ChangeWindowMessageFilterFlags : uint
    {
        Add = 1,
        Remove = 2
    }

    public enum MessageFilterInfo : uint
    {
        None = 0,
        AlreadyAllowed = 1,
        AlreadyDisAllowed = 2,
        AllowedHigher = 3
    }

    public enum ChangeWindowMessageFilterExAction : uint
    {
        Reset = 0,
        Allow = 1,
        DisAllow = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CHANGEFILTERSTRUCT
    {
        public uint size;
        public MessageFilterInfo info;
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

    [Flags]
    public enum WindowFromPointFlags
    {
        CWP_ALL = 0x0000,
        CWP_SKIPINVISIBLE = 0x0001,
        CWP_SKIPDISABLED = 0x0002,
        CWP_SKIPTRANSPARENT = 0x0004
    }

    public enum ShowWindowCommands : int
    {
        Hide = 0,
        Normal = 1,
        ShowMinimized = 2,
        Maximize = 3, // is this the right value?
        ShowMaximized = 3,
        ShowNoActivate = 4,
        Show = 5,
        Minimize = 6,
        ShowMinNoActive = 7,
        ShowNA = 8,
        Restore = 9,
        ShowDefault = 10,
        ForceMinimize = 11
    }

	public class TaskbarPlacement		// Shellapi.h
	{
	    public const uint ABE_LEFT = 0;
        public const uint ABE_TOP = 1;
        public const uint ABE_RIGHT = 2;
        public const uint ABE_BOTTOM = 3;
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

    public enum AppBarMessages : uint
    {
        ABM_NEW = 0x00000000,
        ABM_REMOVE = 0x00000001,
        ABM_QUERYPOS = 0x00000002,
        ABM_SETPOS = 0x00000003,
        ABM_GETSTATE = 0x00000004,
        ABM_GETTASKBARPOS = 0x00000005,
        ABM_ACTIVATE = 0x00000006, // lParam == TRUE/FALSE means activate/deactivate
        ABM_GETAUTOHIDEBAR = 0x00000007,
        ABM_SETAUTOHIDEBAR = 0x00000008, // this can fail at any time.  MUST check the result
        // lParam = TRUE/FALSE  Set/Unset
        // uEdge = what edge
        ABM_WINDOWPOSCHANGED = 0x0000009,
        ABM_SETSTATE = 0x0000000a
    }

    // these are put in the wparam of callback messages
    public enum AppBarNotifications : uint
    {
        ABN_STATECHANGE = 0x0000000,
        ABN_POSCHANGED = 0x0000001,
        ABN_FULLSCREENAPP = 0x0000002,
        ABN_WINDOWARRANGE = 0x0000003 // lParam == TRUE means hide
    }

    // flags for get state
    public enum AppBarStates : uint
    {
        ABS_AUTOHIDE = 0x0000001,
        ABS_ALWAYSONTOP = 0x0000002
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct MonitorInfoEx
    {
        // size of a device name string
        public const int CCHDEVICENAME = 32;
        public int Size;
        public RectStruct Monitor;
        public RectStruct WorkArea;
        public uint Flags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string DeviceName;

        public void Init()
        {
            this.Size = 40 + 2 * CCHDEVICENAME;
            this.DeviceName = string.Empty;
        }
    }

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
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MonitorInfo
    {
        public int Size;
        public RectStruct Monitor;
        public RectStruct WorkArea;
        public uint Flags;
        public void Init()
        {
            this.Size = 40;
        }
    }

    public class MonitorConstants
    {
        private MonitorConstants() { }

        public const int MONITOR_DEFAULTTONULL = 0;
        public const int MONITOR_DEFAULTTOPRIMARY = 1;
        public const int MONITOR_DEFAULTTONEAREST = 2;        
    }

    [Flags]
    public enum REG_NOTIFY_CHANGE : uint
    {
        NAME = 0x1,
        ATTRIBUTES = 0x2,
        LAST_SET = 0x4,
        SECURITY = 0x8
    }

    [Flags]
    public enum DwmWindowAttribute : uint
    {
        DWMWA_NCRENDERING_ENABLED = 1,
        DWMWA_NCRENDERING_POLICY,
        DWMWA_TRANSITIONS_FORCEDISABLED,
        DWMWA_ALLOW_NCPAINT,
        DWMWA_CAPTION_BUTTON_BOUNDS,
        DWMWA_NONCLIENT_RTL_LAYOUT,
        DWMWA_FORCE_ICONIC_REPRESENTATION,
        DWMWA_FLIP3D_POLICY,
        DWMWA_EXTENDED_FRAME_BOUNDS,
        DWMWA_HAS_ICONIC_BITMAP,
        DWMWA_DISALLOW_PEEK,
        DWMWA_EXCLUDED_FROM_PEEK,
        DWMWA_LAST
    }

    [Flags]
    public enum DWMNCRenderingPolicy : uint
    {
        UseWindowStyle,
        Disabled,
        Enabled,
        Last
    }

    public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

    public delegate IntPtr SubclassProc(
        IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, IntPtr dwRefData);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

    public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, 
        int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    public class User32Dll
    {
        private const string User32DllName = "user32.dll";
        private User32Dll() { }

        [DllImport(User32DllName, CharSet = CharSet.Auto)]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

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
        public static extern bool GetUserObjectInformation(IntPtr hObj, int nIndex, [Out] byte[] pvInfo, 
            uint nLength, out uint lpnLengthNeeded);

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
        public static extern uint RegisterWindowMessage(string name);

        [DllImport(User32DllName)]
        public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport(User32DllName, SetLastError = true)]
        public static extern int MapWindowPoints(IntPtr hwndFrom, IntPtr hwndTo, ref POINT lpPoints,
            [MarshalAs(UnmanagedType.U4)] int cPoints);

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
        public static extern bool IntersectRect(out RECT lprcDst, [In] ref RECT lprcSrc1, [In] ref RECT lprcSrc2);

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

        [DllImport(User32DllName)]
        public static extern void NotifyWinEvent(uint eventId, IntPtr hwnd, int idObject, int idChild);

        [DllImport(User32DllName)]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, 
            WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport(User32DllName)]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

		[DllImport(User32DllName)]
		public static extern long GetWindowLong(IntPtr hwnd, int nIndex);

		[DllImport(User32DllName)]
        public static extern IntPtr SetWindowLong(IntPtr hwnd, int nIndex, long dwNewLong);

		[DllImport(User32DllName)]
		public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, UInt32 crKey, Byte bAlpha, UInt32 dwFlags);

        [DllImport(User32DllName)]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport(User32DllName)]
        public static extern bool RedrawWindow(IntPtr hWnd, [In] ref RECT lprcUpdate, IntPtr hrgnUpdate, RedrawWindowConstants flags);

        [DllImport(User32DllName)]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowConstants flags);

        [DllImport(User32DllName)]
        public static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport(User32DllName)]
        public static extern bool ChangeWindowMessageFilter(uint msg, ChangeWindowMessageFilterFlags flags);

        [DllImport(User32DllName, SetLastError = true)]
        public static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, uint msg, ChangeWindowMessageFilterExAction action, ref CHANGEFILTERSTRUCT changeInfo);
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
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
          int dwSize, out int lpNumberOfBytesRead);

        [DllImport(Kernel32DllName, SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, 
            [Out, MarshalAs(UnmanagedType.AsAny)] object lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport(Kernel32DllName, SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer,
            int dwSize, out int lpNumberOfBytesRead);

        [DllImport(Kernel32DllName, SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
            byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport(Kernel32DllName)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

        [DllImport(Kernel32DllName)]
        public static extern uint GetVersion();

		[DllImport(Kernel32DllName)]
        public static extern IntPtr CreateRemoteThread(
			  IntPtr hProcess,
			  IntPtr lpThreadAttributes,	// _In_   LPSECURITY_ATTRIBUTES
			  uint dwStackSize,				// _In_   SIZE_T 
			  IntPtr lpStartAddress,		// _In_   LPTHREAD_START_ROUTINE 
			  IntPtr lpParameter,			// _In_   LPVOID
			  uint dwCreationFlags,
			  out uint lpThreadId
			);

		[DllImport(Kernel32DllName)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		[DllImport(Kernel32DllName)]
		public static extern bool CloseHandle(IntPtr theHandle);
    }

    public class Comctl32Dll
    {
        private const string Comctl32DllName = "comctl32.dll";
        private Comctl32Dll() { }

        [DllImport(Comctl32DllName)]
        public static extern bool SetWindowSubclass(IntPtr hWnd, SubclassProc pfnSubclass, IntPtr uIdSubclass, IntPtr dwRefData);

        [DllImport(Comctl32DllName)]
        public static extern IntPtr DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport(Comctl32DllName)]
        public static extern bool RemoveWindowSubclass(IntPtr hWnd, SubclassProc pfnSubclass, IntPtr uIdSubclass);
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
        public static extern IntPtr SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);
    }

    public class OleaccDll
    {
        private const string OleaccDllName = "oleacc.dll";
        private OleaccDll() { }

        [DllImport(OleaccDllName)]
        public static extern uint AccessibleObjectFromEvent(IntPtr hwnd, uint dwObjectID, uint dwChildID, 
            out IAccessible ppacc, [MarshalAs(UnmanagedType.Struct)] out object pvarChild);
    }

    public class Advapi32Dll
    {
        private const string Advapi32DllName = "advapi32.dll";
        private Advapi32Dll() { }

        [DllImport(Advapi32DllName)]
        public static extern int RegNotifyChangeKeyValue(IntPtr hKey, bool watchSubtree, REG_NOTIFY_CHANGE notifyFilter,
           IntPtr hEvent, bool asynchronous);
    }

    public class DwmapiDll
    {
        private const string DwmapiDllName = "dwmapi.dll";
        private DwmapiDll() { }

        [DllImport(DwmapiDllName, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DwmIsCompositionEnabled();

        [DllImport(DwmapiDllName, PreserveSig = false)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwmAttribute,
                                                       IntPtr pvAttribute, uint cbAttribute);
    }
}
