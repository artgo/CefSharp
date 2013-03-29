using AppDirect.WindowsClient.Browser.Control;
using System;
using System.Collections.Generic;
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
        private const string CefClientExe = @"cefclient.exe";

        public void Load(string cachePath, string currentDir)
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

            var mainArgs = new CefMainArgs(new string[] { });
            var cefApp = new AppDirectCefApp();

            var exitCode = CefRuntime.ExecuteProcess(mainArgs, cefApp);
            if (exitCode != -1)
            {
                ErrorAndExit(new Exception("CEF Failed to load"));
            }

            var cefSettings = new CefSettings
            {
                BrowserSubprocessPath = currentDir + Path.DirectorySeparatorChar + CefClientExe,
                SingleProcess = false,
                MultiThreadedMessageLoop = true,
                LogSeverity = CefLogSeverity.Error,
                PersistSessionCookies = true,
                LogFile = "cef.log",
                CachePath = cachePath,
                IgnoreCertificateErrors = true
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

        public void Initialize()
        {
            var currentDirectory = Environment.CurrentDirectory;
            var cachePath = currentDirectory + Path.DirectorySeparatorChar + CacheDirectory +
                            Path.DirectorySeparatorChar + DefaultId;

            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }

            Load(cachePath, currentDirectory);

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

        private class CookiesSetTask : CefTask
        {
            private readonly IList<Cookie> _cookies;

            internal CookiesSetTask(IList<Cookie> cookies)
            {
                _cookies = cookies;
            }

            protected override void Execute()
            {
                foreach (var cookie in _cookies)
                {
                    CefCookieManager.Global.SetCookie("https://" + cookie.Domain + cookie.Path, new CefCookie()
                    {
                        Creation = cookie.TimeStamp,
                        Domain = cookie.Domain,
                        Expires = cookie.Expires,
                        HttpOnly = cookie.HttpOnly,
                        LastAccess = cookie.TimeStamp,
                        Name = cookie.Name,
                        Path = cookie.Path,
                        Secure = cookie.Secure,
                        Value = cookie.Value
                    });
                }
            }
        }

        public void SetCookies(IList<Cookie> cookies)
        {
            var cookiesTask = new CookiesSetTask(cookies);
            CefRuntime.PostTask(CefThreadId.IO, cookiesTask);
        }
    }
}