using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Browser.Interaction;
using AppDirect.WindowsClient.Browser.MainApp;
using AppDirect.WindowsClient.Browser.Properties;
using AppDirect.WindowsClient.Browser.Session;
using AppDirect.WindowsClient.Browser.UI;
using AppDirect.WindowsClient.Common.API;
using CommandLine;
using System;
using System.ServiceModel;
using System.Windows;

namespace AppDirect.WindowsClient.Browser
{
    static class Program
    {
        private static readonly BrowserObject BrowserObject = new BrowserObject();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var appId = ExtractAppId(args);
            try
            {
                BrowserObject.Initialize(appId);
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format(Resources.Failed_to_initialize_browser_error_message, e.Message));
            }

            var browserViewModel = BuildBrowserViewModel(appId);

            SessionKeeper sessionKeeper = null;
            if ((browserViewModel != null) && (browserViewModel.Application != null))
            {
                var url = browserViewModel.Application.UrlString;
                if (!string.IsNullOrEmpty(url))
                {
                    sessionKeeper = new SessionKeeper(url);
                    sessionKeeper.Start();
                }
            }

            Window browserWindow = new BrowserWindow(browserViewModel);

            var app = new App(browserWindow);
            app.InitializeComponent();
            app.Run();

            if (sessionKeeper != null)
            {
                sessionKeeper.Stop();
            }

            BrowserObject.Unload();
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

        private static BrowserViewModel BuildBrowserViewModel(string appId)
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

            var browser = new BrowserViewModel() { Application = app, Session = session };

            return browser;
        }

        private static void SetCookies(IAppDirectSession session)
        {
            if (session != null)
            {
                foreach (var cookie in session.Cookies)
                {
                    BrowserObject.SetCookie(cookie);
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
