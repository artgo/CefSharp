using AppDirect.WindowsClient.Browser.Control;
using AppDirect.WindowsClient.Common.Log;
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
    public class BrowserObject : IBrowserObject
    {
        private const string CacheDirectory = @"Cache";
        private const string DefaultId = @"Default";
        private const string CefClientExe = @"cefclient.exe";

        private readonly ILogger _log;

        public BrowserObject(ILogger log)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            _log = log;
        }

        private void Load(string cachePath, string currentDir)
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
                LogSeverity = CefLogSeverity.Verbose,
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

        private void ErrorAndExit(Exception ex)
        {
            _log.ErrorException("Error during browser initialization", ex);

            MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
        }

        public void Unload()
        {
            try
            {
                // Shutdown CEF
                CefRuntime.Shutdown();
            }
            catch (Exception e)
            {
                _log.ErrorException("Error during browser shutdown", e);
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