using System;
using System.ServiceModel;
using System.Windows.Forms;
using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Browser.MainApp;
using AppDirect.WindowsClient.Browser.Properties;
using AppDirect.WindowsClient.Browser.UI;
using AppDirect.WindowsClient.Common.API;
using CommandLine;
using Gecko;

namespace AppDirect.WindowsClient.Browser
{
    static class Program
    {
        private const string DefaultUrl = @"http://localhost";
        private const string GfxFontRenderingGraphiteEnabled = "gfx.font_rendering.graphite.enabled";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Xpcom.Initialize(XULRunnerLocator.GetXULRunnerLocation());

            // Uncomment the follow line to enable CustomPrompt's
            // GeckoPreferences.User["browser.xul.error_pages.enabled"] = false;
            GeckoPreferences.User[GfxFontRenderingGraphiteEnabled] = true;

            Application.ApplicationExit += (sender, e) => Xpcom.Shutdown();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var browser = BuildBrowserWindow(args);

            Application.Run(browser);
        }

        private static BrowserWindow BuildBrowserWindow(string[] args)
        {
            var options = new Options();

            if ((args != null) && (args.Length > 0) &&
                (CommandLineParser.Default.ParseArguments(args, options)) &&
                !string.IsNullOrEmpty(options.AppId))
            {
                var browser = ProcessApplicationId(options);
                if (browser != null)
                {
                    return browser;
                }
            }

            return new BrowserWindow(DefaultUrl, null);
        }

        private static BrowserWindow ProcessApplicationId(Options options)
        {
            MainApplicationClient client;
            try
            {
                var callback = new MainApplicationCallback();
                var context = new InstanceContext(callback);
                context.Faulted += ErrorOnServer;
                client = new MainApplicationClient(context);
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format(Resources.Communications_can_t_be_established_error_message, e.Message));
                return null;
            }

            IApplication app;
            IAppDirectSession session;
            try
            {
                app = (IApplication)client.GetApplicationById(options.AppId);
                session = (IAppDirectSession)client.GetCurrentSession();
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format(Resources.Error_getting_data_error_message, options.AppId, e.Message));
                return null;
            }

            if (app == null)
            {
                MessageBox.Show(String.Format(Resources.No_app_data_transfered_error_message, options.AppId));
                return null;
            }

            if (session == null)
            {
                MessageBox.Show(String.Format(Resources.No_session_data_transfered_error_message, options.AppId));
                return null;
            }

            if (string.IsNullOrEmpty(app.UrlString))
            {
                MessageBox.Show(String.Format(Resources.Url_for_the_application_is_empty_error_message, options.AppId));
                return null;
            }

            SetCookies(session);

            var browser = new BrowserWindow(app.UrlString, session);
            return browser;
        }

        private static void SetCookies(IAppDirectSession session)
        {
            if (session != null)
            {
                foreach (var cookie in session.Cookies)
                {
                    CookieManager.Add(cookie.Domain, cookie.Path, cookie.Name, cookie.Value, cookie.Secure, cookie.HttpOnly, false, new DateTime(2100, 1, 1).ToBinary());
                }
            }
        }

        private static void ErrorOnServer(object sender, System.EventArgs e)
        {
            MessageBox.Show(e.ToString());
            throw new Exception();
        }
    }
}
