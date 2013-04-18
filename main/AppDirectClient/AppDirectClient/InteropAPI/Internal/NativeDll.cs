﻿using System;
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
        public static extern void InjectExplrorerExe();

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DetachHooks();

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SetupHooks2(int code, IntPtr wParam, IntPtr lParam);

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FindTaskBar();

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FindRebar();

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetRebarThread();

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetExitMsg();

        [DllImport(NativeDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetUpdatePositionMsg();
    }
}