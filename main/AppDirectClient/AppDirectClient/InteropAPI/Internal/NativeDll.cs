using System;
using System.Runtime.InteropServices;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class NativeDll
    {
        private const string NativeDllName = "native.dll";

        private NativeDll()
        {
        }

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetupSubclass(IntPtr hwndAdButton);

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool TearDownSubclass();
    }
}