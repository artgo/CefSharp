using System;
using System.Runtime.InteropServices;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class NativeDll : IDisposable
    {
        private IntPtr _dllHandle;

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate bool BoolAction();
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate bool BoolActionOnIntPtr(IntPtr hwnd);

        BoolActionOnIntPtr _setupSubclassFunc;
        BoolAction _tearDownFunc;
        BoolAction _isSubclassedFunc;

        public NativeDll(string path)
        {
            _dllHandle = Kernel32Dll.LoadLibrary(path);

            if (_dllHandle == IntPtr.Zero)
            {
                throw new InteropException("LoadLibrary failed to load native.dll");
            }
        
            IntPtr procAddress = IntPtr.Zero;
            if ((procAddress = Kernel32Dll.GetProcAddress(_dllHandle, "SetupSubclass")) == IntPtr.Zero) 
            {
                throw new InteropException("SetupSubclass function is not present in native.dll");
            }
            _setupSubclassFunc = (BoolActionOnIntPtr)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(BoolActionOnIntPtr));
            
            if ((procAddress = Kernel32Dll.GetProcAddress(_dllHandle, "TearDownSubclass")) == IntPtr.Zero)
            {
                throw new InteropException("TearDownSubclass function is not present in native.dll");
            }
            _tearDownFunc = (BoolAction)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(BoolAction));
            
            if ((procAddress = Kernel32Dll.GetProcAddress(_dllHandle, "IsSubclassed")) == IntPtr.Zero)
            {
                throw new InteropException("TearDownSubclass function is not present in native.dll");
            }
            _isSubclassedFunc = (BoolAction)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(BoolAction)); 
        }

        public bool IsValid()
        {
            return _dllHandle != IntPtr.Zero;
        }

        public bool SetupSubclass(IntPtr hwndAdButton)
        {
            return (_setupSubclassFunc != null) ? _setupSubclassFunc.Invoke(hwndAdButton) : false;
        }

        public bool TearDownSubclass()
        {
            return (_tearDownFunc != null) ? _tearDownFunc.Invoke() : false;
        }

        public bool IsSubclassed()
        {
            return (_isSubclassedFunc != null) ? _isSubclassedFunc.Invoke() : false;
        }

        public void Dispose()
        {
            if (_dllHandle != IntPtr.Zero)
            {
                Kernel32Dll.FreeLibrary(_dllHandle);
            }
        }
    }
}