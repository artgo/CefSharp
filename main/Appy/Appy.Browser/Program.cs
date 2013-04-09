using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Browser.Interaction;
using AppDirect.WindowsClient.Browser.Properties;
using AppDirect.WindowsClient.Browser.Session;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;
using System;
using System.Windows;

namespace AppDirect.WindowsClient.Browser
{
    internal static class Program
    {
        private static readonly UnhandledExceptionEventHandler ExceptionHandler = CurrentDomainOnUnhandledException;
        private static readonly ILogger Log = new NLogLogger("Browser.Program");
        private static readonly IBrowserObject BrowserObject = new BrowserObject(new NLogLogger("BrowserObject"));
        private static readonly IUiHelper UiHelper = new UiHelper(new NLogLogger("UiHelper"));
        private static readonly IBrowserWindowsManager BrowserWindowsManager = new BrowserWindowsManager(BrowserObject, UiHelper);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += ExceptionHandler;

            try
            {
                BrowserObject.Initialize();
            }
            catch (Exception e)
            {
                Log.ErrorException("Failed to initialize the browser", e);

                MessageBox.Show(String.Format(Resources.Failed_to_initialize_browser_error_message, e.Message));
                return;
            }

            var api = new BrowsersManagerApi(BrowserWindowsManager, UiHelper);
            var apiStarter = new IpcMainWindowStarter(api);

            var mainAppClient = new MainApplicationServiceClient(new MainApplicationClientServiceStarter(), UiHelper,
                                                                 new NLogLogger("MainApplicationServiceClient"));

            var sessionKeeper = new SessionKeeper(mainAppClient, BrowserWindowsManager, new NLogLogger("Browser.SessionKeeper"));

            try
            {
                var app = new App();
                app.InitializeComponent();
                UiHelper.IgnoreException(sessionKeeper.Start);

                bool hadStartException = false;
                try
                {
                    apiStarter.Start();
                }
                catch (Exception e)
                {
                    Log.ErrorException("Failed to start server communication", e);

                    hadStartException = true;
                }

                if (!hadStartException)
                {
                    var wasInitialized = InitializeClient(mainAppClient);

                    if (wasInitialized)
                    {
                        app.Run();
                    }
                }
            }
            finally
            {
                UiHelper.IgnoreException(apiStarter.Stop);
                UiHelper.IgnoreException(sessionKeeper.Stop);
                UiHelper.IgnoreException(BrowserObject.Unload);
            }
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Log.ErrorException("Exception during runtime", unhandledExceptionEventArgs.ExceptionObject as Exception);
        }

        private static bool InitializeClient(MainApplicationServiceClient mainAppClient)
        {
            if (mainAppClient == null)
            {
                throw new ArgumentNullException("mainAppClient");
            }

            try
            {
                mainAppClient.Start();
            }
            catch (Exception e)
            {
                Log.ErrorException("Failed to establish connection with main app", e);

                MessageBox.Show(String.Format(Resources.Communications_can_t_be_established_error_message, e.Message));
                return false;
            }

            try
            {
                mainAppClient.Initialized();
            }
            catch (Exception e)
            {
                Log.ErrorException("Failed to establish connection with main app", e);

                MessageBox.Show(String.Format(Resources.Error_getting_data_error_message, e.Message));
                return false;
            }

            return true;
        }
    }
}