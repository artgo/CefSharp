using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Browser.Interaction;
using AppDirect.WindowsClient.Browser.MainApp;
using AppDirect.WindowsClient.Browser.Properties;
using AppDirect.WindowsClient.Browser.Session;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;
using System;
using System.Collections.Generic;
using System.Windows;

namespace AppDirect.WindowsClient.Browser
{
    internal static class Program
    {
        private static readonly ILogger _log = new NLogLogger("Browser.Program");
        private static readonly IBrowserObject BrowserObject = new BrowserObject(new NLogLogger("BrowserObject"));
        private static readonly IUiHelper UiHelper = new UiHelper(new NLogLogger("UiHelper"));
        private static readonly IBrowserWindowsManager BrowserWindowsManager = new BrowserWindowsManager(BrowserObject, UiHelper);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                BrowserObject.Initialize();
            }
            catch (Exception e)
            {
                _log.ErrorException("Failed to initialize the browser", e);

                MessageBox.Show(String.Format(Resources.Failed_to_initialize_browser_error_message, e.Message));
                return;
            }

            GetSessionAndApplications();
            var api = new BrowsersManagerApi(BrowserWindowsManager, UiHelper);
            var apiStarter = new IpcMainWindowStarter(api);

            var sessionKeeper = new SessionKeeper(BrowserWindowsManager, new NLogLogger("Browser.SessionKeeper"));

            try
            {
                var app = new App();
                app.InitializeComponent();
                UiHelper.IgnoreException(sessionKeeper.Start);

                bool hadStartException = false;
                try
                {
                    UiHelper.IgnoreException(apiStarter.Start);
                }
                catch (Exception e)
                {
                    _log.ErrorException("Failed to start server communication", e);

                    hadStartException = true;
                }

                if (!hadStartException)
                {
                    app.Run();
                }
            }
            finally
            {
                UiHelper.IgnoreException(apiStarter.Stop);
                UiHelper.IgnoreException(sessionKeeper.Stop);
                UiHelper.IgnoreException(BrowserObject.Unload);
            }
        }

        private static void GetSessionAndApplications()
        {
            MainApplicationClient mainApplicationClient;

            try
            {
                mainApplicationClient = new MainApplicationClient();
            }
            catch (Exception e)
            {
                _log.ErrorException("Failed to establish connection with main app", e);

                MessageBox.Show(String.Format(Resources.Communications_can_t_be_established_error_message, e.Message));
                return;
            }

            IEnumerable<IApplication> applications;
            IAppDirectSession session;
            try
            {
                var data = (IInitData)mainApplicationClient.Initialized();
                applications = data.Applications;
                session = data.Session;
            }
            catch (Exception e)
            {
                _log.ErrorException("Failed to establish connection with main app", e);

                MessageBox.Show(String.Format(Resources.Error_getting_data_error_message, e.Message));
                return;
            }

            if (applications == null)
            {
                _log.Error("Applications passed to the browser manager were null");

                MessageBox.Show(String.Format(Resources.No_app_data_transfered_error_message));
                return;
            }

            if (session == null)
            {
                _log.Error("Session passed to the browser manager is null");

                MessageBox.Show(String.Format(Resources.No_session_data_transfered_error_message));
                return;
            }

            try
            {
                SetCookies(session);
            }
            catch (Exception e)
            {
                _log.ErrorException("Failed to set cookies", e);
            }

            BrowserWindowsManager.Applications = applications;
            BrowserWindowsManager.Session = session;
        }

        private static void SetCookies(IAppDirectSession session)
        {
            if (session != null)
            {
                BrowserObject.SetCookies(session.Cookies);
            }
        }
    }
}