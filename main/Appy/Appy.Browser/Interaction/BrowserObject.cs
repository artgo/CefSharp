using System;
using System.IO;
using System.Windows.Forms;
using Gecko;
using Cookie = System.Net.Cookie;

namespace AppDirect.WindowsClient.Browser.Interaction
{
    /// <summary>
    /// Interaction with underlying browser system
    /// </summary>
    public class BrowserObject
    {
        private static readonly object SyncObject = new object();
        private static volatile BrowserObject _browserObject = null;
        private const string GfxFontRenderingGraphiteEnabled = @"gfx.font_rendering.graphite.enabled";
        private static readonly long InfiniteDate = (new DateTime(2100, 1, 1)).ToBinary();
        private const string CacheDirectory = @"Cache";
        private const string DefaultId = @"Default";
        private const string CacheDiskEnable = @"browser.cache.disk.enable";
        private const string CacheMemoryEnable = @"browser.cache.memory.enable";
        private const string CacheDiskParentDirectory = @"browser.cache.disk.parent_directory";
        private const string StartupPage = @"browser.startup.page";
        private const int RestoreSession = 3;
        private const string SessionStoreRestoreFromCrash = @"browser.sessionstore.resume_from_crash";

        private BrowserObject() { }

        public static BrowserObject Instance
        {
            get
            {
                lock (SyncObject)
                {
                    if (_browserObject == null)
                    {
                        _browserObject = new BrowserObject();
                    }
                }

                return _browserObject;
            }
        }

        public void Initialize(string appId)
        {
            var safeAppId = string.IsNullOrEmpty(appId) ? DefaultId : appId;
            var currentDirectory = Environment.CurrentDirectory;
            var cachePath = currentDirectory + Path.DirectorySeparatorChar + CacheDirectory +
                            Path.DirectorySeparatorChar + safeAppId;

            Xpcom.Initialize(currentDirectory);

            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }

            GeckoPreferences.User[CacheDiskEnable] = true;
            GeckoPreferences.User[CacheMemoryEnable] = true;
            GeckoPreferences.User[CacheDiskParentDirectory] = cachePath;
            GeckoPreferences.User[SessionStoreRestoreFromCrash] = true;
            GeckoPreferences.User[StartupPage] = RestoreSession;

            // Uncomment the follow line to enable CustomPrompt's
            // GeckoPreferences.User["browser.xul.error_pages.enabled"] = false;
            GeckoPreferences.User[GfxFontRenderingGraphiteEnabled] = true;

            RestoreBrowserSession();

            Application.ApplicationExit += (sender, e) => Xpcom.Shutdown();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ResurrectCookies();
        }

        private void ResurrectCookies()
        {
            var cookiesEnumerator = CookieManager.GetEnumerator();

            while (cookiesEnumerator.MoveNext())
            {
                var cookie = cookiesEnumerator.Current;
                if (cookie != null)
                {
                    CookieManager.Add(cookie.Host, cookie.Path, cookie.Name, cookie.Value, cookie.IsSecure,
                                      cookie.IsHttpOnly, cookie.IsSession, InfiniteDate);
                }
            }
        }

        private void RestoreBrowserSession()
        {
//            Guid iid = typeof(nsISessionStore).GUID;
//            Guid guid = new Guid("59bfaf00-e3d8-4728-b4f0-cc0b9dfb4806");
//            IntPtr ptr = Xpcom.ServiceManager.GetService(ref iid, ref iid);
//            nsISessionStore sessionStore = (nsISessionStore)Xpcom.GetObjectForIUnknown(ptr);
//            sessionStore.RestoreLastSession();
        }

        public void SetCookie(Cookie cookie)
        {
            CookieManager.Add(cookie.Domain, cookie.Path, cookie.Name, cookie.Value,
                cookie.Secure, cookie.HttpOnly, false, InfiniteDate);
        }
    }
}