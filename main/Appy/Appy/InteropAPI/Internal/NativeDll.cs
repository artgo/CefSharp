using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class NativeDll
    {
        private const string NativeDllName = "native.dll";
        private NativeDll() {}

        // TODO: -1 tmp code: to be rewritten into C#
        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FindTaskBar();

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FindRebar();

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetRebarThread();

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern Point GetTaskbarPos();

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern Size GetStartButtonSize();

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern Size GetInitialADButtonSize();

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InjectExplrorerExe();

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DetachHooks();

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SetupHooks2(int code, IntPtr wParam, IntPtr lParam);
    }
}