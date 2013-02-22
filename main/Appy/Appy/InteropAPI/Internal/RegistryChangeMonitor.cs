using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public delegate void RegistryChangeHandler(object sender, RegistryChangeEventArgs e);

    public class RegistryChangeMonitor : IDisposable
    {
        private readonly REG_NOTIFY_CHANGE _filter;
        private readonly string _registryPath;
        private volatile RegistryKey _monitorKey;
        private volatile Thread _monitorThread;
        private volatile IntPtr _keyPtr = IntPtr.Zero;

        public RegistryChangeMonitor(string registryPath)
            : this(registryPath, REG_NOTIFY_CHANGE.LAST_SET)
        {
        }

        public RegistryChangeMonitor(string registryPath, REG_NOTIFY_CHANGE filter)
        {
            _registryPath = registryPath.ToUpper();
            _filter = filter;
        }

        public bool Monitoring
        {
            get
            {
                if (_monitorThread != null)
                {
                    return _monitorThread.IsAlive;
                }

                return false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public event RegistryChangeHandler Changed;

        public event RegistryChangeHandler Error;

        ~RegistryChangeMonitor()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            Stop();
        }

        public void Start()
        {
            lock (this)
            {
                ThreadStart ts = MonitorThread;
                _monitorThread = new Thread(ts) {IsBackground = true};
                _keyPtr = Init();
                if (_keyPtr != IntPtr.Zero)
                {
                    _monitorThread.Start();
                } 
                else
                {
                    throw new Exception("Key not found");
                }
            }
        }

        public void Stop(bool hard = true)
        {
            lock (this)
            {
                Cleanup();

                if (_monitorThread == null)
                {
                    return;
                }

                if (hard)
                {
                    if (_monitorThread.IsAlive)
                    {
                        _monitorThread.Abort();
                    }
                    _monitorThread = null;
                }
            }
        }

        private void Cleanup()
        {
            // The "Close()" will trigger RegNotifyChangeKeyValue if it is still listening
            if (_monitorKey != null)
            {
                _monitorKey.Close();
                _monitorKey = null;
            }

            _keyPtr = IntPtr.Zero;
        }

        private void MonitorThread()
        {
            try
            {
                if (_keyPtr != IntPtr.Zero)
                {
                    while (true)
                    {
                        lock (this)
                        {
                            // If _monitorThread is null that probably means Dispose is being called. Don't monitor anymore.
                            if ((_monitorThread == null) || (_monitorKey == null) || (_keyPtr == IntPtr.Zero))
                            {
                                break;
                            }
                        }

                        // RegNotifyChangeKeyValue blocks until a change occurs.
                        var result = Advapi32Dll.RegNotifyChangeKeyValue(_keyPtr, true, _filter, IntPtr.Zero, false);

                        lock (this)
                        {
                            if ((_monitorThread == null) || (_monitorKey == null) || (_keyPtr == IntPtr.Zero))
                            {
                                break;
                            }
                        }

                        if (result == 0)
                        {
                            if (Changed != null)
                            {
                                var e = new RegistryChangeEventArgs(this, null);
                                Changed(this, e);

                                if (e.Stop)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (Error != null)
                            {
                                var ex = new Win32Exception();

                                // Unless the exception is thrown, nobody is nice enough to set a good stacktrace for us. Set it ourselves.
                                typeof (Exception).InvokeMember(
                                    "_stackTrace",
                                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField,
                                    null,
                                    ex,
                                    new object[] {new StackTrace(true)}
                                    );

                                var e = new RegistryChangeEventArgs(this, ex);
                                Error(this, e);
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Error != null)
                {
                    var e = new RegistryChangeEventArgs(this, ex);
                    Error(this, e);
                }
            }
            finally
            {
                Cleanup();
            }
        }

        private IntPtr Init()
        {
            if (_registryPath.StartsWith("HKEY_CLASSES_ROOT"))
                _monitorKey = Registry.ClassesRoot.OpenSubKey(_registryPath.Substring(18));
            else if (_registryPath.StartsWith("HKCR"))
                _monitorKey = Registry.ClassesRoot.OpenSubKey(_registryPath.Substring(5));
            else if (_registryPath.StartsWith("HKEY_CURRENT_USER"))
                _monitorKey = Registry.CurrentUser.OpenSubKey(_registryPath.Substring(18));
            else if (_registryPath.StartsWith("HKCU"))
                _monitorKey = Registry.CurrentUser.OpenSubKey(_registryPath.Substring(5));
            else if (_registryPath.StartsWith("HKEY_LOCAL_MACHINE"))
                _monitorKey = Registry.LocalMachine.OpenSubKey(_registryPath.Substring(19));
            else if (_registryPath.StartsWith("HKLM"))
                _monitorKey = Registry.LocalMachine.OpenSubKey(_registryPath.Substring(5));
            else if (_registryPath.StartsWith("HKEY_USERS"))
                _monitorKey = Registry.Users.OpenSubKey(_registryPath.Substring(11));
            else if (_registryPath.StartsWith("HKU"))
                _monitorKey = Registry.Users.OpenSubKey(_registryPath.Substring(4));
            else if (_registryPath.StartsWith("HKEY_CURRENT_CONFIG"))
                _monitorKey = Registry.CurrentConfig.OpenSubKey(_registryPath.Substring(20));
            else if (_registryPath.StartsWith("HKCC"))
                _monitorKey = Registry.CurrentConfig.OpenSubKey(_registryPath.Substring(5));

            // Fetch the native handle
            if (_monitorKey != null)
            {
                object hkey = typeof (RegistryKey).InvokeMember(
                    "hkey",
                    BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    _monitorKey,
                    null
                    );

                _keyPtr = (IntPtr) typeof (SafeHandle).InvokeMember(
                    "handle",
                    BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    hkey,
                    null);
            }

            return _keyPtr;
        }
    }
}