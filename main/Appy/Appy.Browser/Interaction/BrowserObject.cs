using AppDirect.WindowsClient.Browser.Control;
using System;
using System.IO;
using System.Windows;
using Xilium.CefGlue;
using Cookie = System.Net.Cookie;

namespace AppDirect.WindowsClient.Browser.Interaction
{
    /// <summary>
    /// Interaction with underlying browser system
    /// </summary>
    public class BrowserObject
    {
        private static readonly long InfiniteDate = (new DateTime(2100, 1, 1)).ToBinary();
        private const string CacheDirectory = @"Cache";
        private const string DefaultId = @"Default";
        private const int RestoreSession = 3;

        public void Load()
        {
            try
            {
                CefRuntime.Load();
            }
            catch (DllNotFoundException ex)
            {
                ErrorAndExit(ex);
            }
            catch (CefRuntimeException ex)
            {
                ErrorAndExit(ex);
            }
            catch (Exception ex)
            {
                ErrorAndExit(ex);
            }

            var mainArgs = new CefMainArgs(new string[] {});
            var cefApp = new AppDirectCefApp();

            var exitCode = CefRuntime.ExecuteProcess(mainArgs, cefApp);
            if (exitCode != -1)
            {
                ErrorAndExit(new Exception("CEF Failed to load"));
            }

            var cefSettings = new CefSettings
            {
                // BrowserSubprocessPath = browserSubprocessPath,
                SingleProcess = false,
                MultiThreadedMessageLoop = true,
                LogSeverity = CefLogSeverity.Error,
                PersistSessionCookies = true,
                LogFile = "cef.log",
            };

            try
            {
                CefRuntime.Initialize(mainArgs, cefSettings, cefApp);
            }
            catch (CefRuntimeException ex)
            {
                ErrorAndExit(ex);
            }
        }

        private static void ErrorAndExit(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
        }

        internal void Unload()
        {
            try
            {
                // Shutdown CEF
                CefRuntime.Shutdown();
            }
            catch (Exception ex)
            {
                // Do nothing for now.
            }
        }
        public void Initialize(string appId)
        {
            var safeAppId = string.IsNullOrEmpty(appId) ? DefaultId : appId;
            var currentDirectory = Environment.CurrentDirectory;
            var cachePath = currentDirectory + Path.DirectorySeparatorChar + CacheDirectory +
                            Path.DirectorySeparatorChar + safeAppId;

            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }

            Load();

            ResurrectCookies();
        }

        private void ResurrectCookies()
        {
        }

        private void RestoreBrowserSession()
        {
//            Guid iid = typeof(nsISessionStore).GUID;
//            Guid guid = new Guid("59bfaf00-e3d8-4728-b4f0-cc0b9dfb4806");
//            IntPtr ptr = Xpcom.ServiceManager.GetService(ref iid, ref iid);
//            nsISessionStore sessionStore = (nsISessionStore)Xpcom.GetObjectForIUnknown(ptr);
//            sessionStore.RestoreLastSession();
        }


        private class CookieSetTask : CefTask
        {
            private readonly Cookie _cookie;

            internal CookieSetTask(Cookie cookie)
            {
                _cookie = cookie;
            }

            protected override void Execute()
            {
                CefCookieManager.Global.SetCookie("https://" + _cookie.Domain + _cookie.Path, new CefCookie()
                {
                    Creation = _cookie.TimeStamp,
                    Domain = _cookie.Domain,
                    Expires = _cookie.Expires,
                    HttpOnly = _cookie.HttpOnly,
                    LastAccess = _cookie.TimeStamp,
                    Name = _cookie.Name,
                    Path = _cookie.Path,
                    Secure = _cookie.Secure,
                    Value = _cookie.Value
                });
            }
        }

        public void SetCookie(Cookie cookie)
        {
            var cookieTask = new CookieSetTask(cookie);
            CefRuntime.PostTask(CefThreadId.IO, cookieTask);
        }
    }
}