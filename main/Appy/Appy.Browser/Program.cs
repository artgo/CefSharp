using System;
using System.ServiceModel;
using System.Windows.Forms;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Browser.Interaction;
using AppDirect.WindowsClient.Browser.MainApp;
using AppDirect.WindowsClient.Browser.Properties;
using AppDirect.WindowsClient.Browser.Session;
using AppDirect.WindowsClient.Browser.UI;
using AppDirect.WindowsClient.Common.API;
using CommandLine;
using Application = System.Windows.Forms.Application;

namespace AppDirect.WindowsClient.Browser
{
    static class Program
    {
        private const string DefaultUrl = @"http://localhost";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var appId = ExtractAppId(args);
            try
            {
                BrowserObject.Instance.Initialize(appId);
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format(Resources.Failed_to_initialize_browser_error_message, e.Message));
            }

            var browser = BuildBrowserWindow(appId);

            var url = browser.BrowserUrl;
            SessionKeeper sessionKeeper = null;
            if (!string.IsNullOrEmpty(url))
            {
                sessionKeeper = new SessionKeeper(url);
                sessionKeeper.Start();
            }

            Application.Run(browser);

            if (sessionKeeper != null)
            {
                sessionKeeper.Stop();
            }
        }

        private static string ExtractAppId(string[] args)
        {
            var options = new Options();

            if ((args != null) && (args.Length > 0) &&
                (CommandLineParser.Default.ParseArguments(args, options)) &&
                !string.IsNullOrEmpty(options.AppId))
            {
                return options.AppId;
            }

            return null;
        }


        private static BrowserWindow BuildBrowserWindow(string appId)
        {
            var browser = ProcessApplicationId(appId);
            if (browser != null)
            {
                return browser;
            }

            return new BrowserWindow(DefaultUrl, null, Helper.DefaultBrowserWidth, Helper.DefaultBrowserHeight, Helper.DefaultBrowserResizable);
        }

        private static BrowserWindow ProcessApplicationId(string appId)
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
                app = (IApplication)client.GetApplicationById(appId);
                session = (IAppDirectSession)client.GetCurrentSession();
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format(Resources.Error_getting_data_error_message, appId, e.Message));
                return null;
            }

            if (app == null)
            {
                MessageBox.Show(String.Format(Resources.No_app_data_transfered_error_message, appId));
                return null;
            }

            if (session == null)
            {
                MessageBox.Show(String.Format(Resources.No_session_data_transfered_error_message, appId));
                return null;
            }

            if (string.IsNullOrEmpty(app.UrlString))
            {
                MessageBox.Show(String.Format(Resources.Url_for_the_application_is_empty_error_message, appId));
                return null;
            }

            SetCookies(session);

            var browser = new BrowserWindow(app.UrlString, session, app.BrowserWidth, app.BrowserHeight, app.BrowserResizable);

            return browser;
        }

        private static void SetCookies(IAppDirectSession session)
        {
            if (session != null)
            {
                foreach (var cookie in session.Cookies)
                {
                    BrowserObject.Instance.SetCookie(cookie);
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
