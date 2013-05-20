using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Browser.Interaction;
using AppDirect.WindowsClient.Browser.Properties;
using AppDirect.WindowsClient.Browser.Session;
using AppDirect.WindowsClient.Browser.UI;
using AppDirect.WindowsClient.Common;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;
using System;
using System.Threading;
using System.Windows;

namespace AppDirect.WindowsClient.Browser
{
    internal static class Program
    {
        private static readonly UnhandledExceptionEventHandler ExceptionHandler = CurrentDomainOnUnhandledException;
        private static readonly ILogger Log = new NLogLogger("Browser.Program");
        private static readonly IBrowserObject BrowserObject = new BrowserObject(new NLogLogger("BrowserObject"));
        private static readonly IUiHelper UiHelper = new UiHelper(new NLogLogger("UiHelper"));
        private static readonly IBrowserWindowsBuilder<IBrowserWindow> BrowserWindowsBuilder = new BrowserWindowsBuilder();
        private static readonly IBrowserWindowsManager BrowserWindowsManager = new BrowserWindowsManager(BrowserObject, UiHelper, BrowserWindowsBuilder);
        private static volatile Mutex _instanceMutex = null;
        private const string _mainApplicationName = "AppDirectClient";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += ExceptionHandler;

            bool createdNew;
            _instanceMutex = new Mutex(true, @"AppDirect.WindowsClient Browser Mutex", out createdNew);
            if (!createdNew)
            {
                Log.Info("Instance already exists, exit.");
                _instanceMutex = null;
                Environment.Exit(0);
            }

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

            var appDirectClientProcessWatcher = new ProcessWatcher(_mainApplicationName);

            var sessionKeeper = new SessionKeeper(mainAppClient, BrowserWindowsManager, BrowserWindowsBuilder, new NLogLogger("Browser.SessionKeeper"), UiHelper);

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

                try
                {
                    appDirectClientProcessWatcher.Start();
                }
                catch (Exception e)
                {
                    Log.ErrorException("Failed to start appDirectClient Process Watcher", e);
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
                UiHelper.IgnoreException(appDirectClientProcessWatcher.Stop);
                UiHelper.IgnoreException(sessionKeeper.Stop);
                UiHelper.IgnoreException(BrowserObject.Unload);
                UiHelper.IgnoreException(_instanceMutex.ReleaseMutex);
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

            var hadException = true;
            while (hadException)
            {
                try
                {
                    mainAppClient.Initialized();
                    hadException = false;
                }
                catch (Exception e)
                {
                    Log.ErrorException("Failed to establish connection with main app", e);
                }
            }

            return true;
        }
    }
}