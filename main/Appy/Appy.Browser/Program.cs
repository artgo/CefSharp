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
    internal static class Program
    {
        private static readonly BrowserObject BrowserObject = new BrowserObject();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            var appId = ExtractAppId(args);
            try
            {
                BrowserObject.Initialize(appId);
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format(Resources.Failed_to_initialize_browser_error_message, e.Message));
                return;
            }

            var browserViewModelAndApi = BuildBrowserViewModel(appId);
            var browserViewModel = browserViewModelAndApi.BrowserViewModel;

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

            var browserWindow = new BrowserWindow(browserViewModel);
            browserViewModelAndApi.MainApplicationCallback.BrowserWindow = browserWindow;

            var app = new App(browserWindow);
            app.InitializeComponent();
            app.Run();

            var client = browserViewModelAndApi.MainApplicationClient;
            if ((client != null) && (client.State == CommunicationState.Opened))
            {
                client.BrowserWasClosed();
                client.Close();
            }

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

        private static BrowserViewModelAndApi BuildBrowserViewModel(string appId)
        {
            MainApplicationClient mainApplicationProxy;
            MainApplicationCallback mainApplicationCallback;

            try
            {
                mainApplicationCallback = new MainApplicationCallback();
                var context = new InstanceContext(mainApplicationCallback);
                context.Faulted += ErrorOnServer;
                mainApplicationProxy = new MainApplicationClient(context);
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
                app = (IApplication)mainApplicationProxy.GetApplicationById(appId);
                session = (IAppDirectSession)mainApplicationProxy.GetCurrentSession();
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

            var browserViewModel = new BrowserViewModel() { Application = app, Session = session };

            var browserAndApi = new BrowserViewModelAndApi()
                {
                    BrowserViewModel = browserViewModel,
                    MainApplicationClient = mainApplicationProxy,
                    MainApplicationCallback = mainApplicationCallback
                };

            return browserAndApi;
        }

        private static void SetCookies(IAppDirectSession session)
        {
            if (session != null)
            {
                BrowserObject.SetCookies(session.Cookies);
            }
        }

        private static void ErrorOnServer(object sender, System.EventArgs e)
        {
            MessageBox.Show(e.ToString());
            throw new Exception();
        }
    }
}