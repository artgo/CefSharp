using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Browser.Interaction;
using AppDirect.WindowsClient.Browser.MainApp;
using AppDirect.WindowsClient.Browser.Properties;
using AppDirect.WindowsClient.Browser.Session;
using AppDirect.WindowsClient.Common.API;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;

namespace AppDirect.WindowsClient.Browser
{
    internal static class Program
    {
        private static readonly BrowserObject BrowserObject = new BrowserObject();
        private static readonly BrowserWindowsManager BrowserWindowsManager = new BrowserWindowsManager(BrowserObject);

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
                MessageBox.Show(String.Format(Resources.Failed_to_initialize_browser_error_message, e.Message));
                return;
            }

            var client = GetSessionAndApplications();
            var api = new BrowsersManagerApi() {BrowserWindowsManager = BrowserWindowsManager};
            var apiStarter = new IpcMainWindowStarter(api);

            var sessionKeeper = new SessionKeeper(BrowserWindowsManager);

            try
            {
                var app = new App();
                app.InitializeComponent();
                sessionKeeper.Start();
                apiStarter.Start();
                app.Run();
            }
            finally
            {
                apiStarter.Stop();
                sessionKeeper.Stop();

                BrowserObject.Unload();

                if ((client != null) && (client.State == CommunicationState.Opened))
                {
                    client.Close();
                }
            }
        }

        private static MainApplicationClient GetSessionAndApplications()
        {
            MainApplicationClient mainApplicationClient;

            try
            {
                mainApplicationClient = new MainApplicationClient();
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format(Resources.Communications_can_t_be_established_error_message, e.Message));
                return null;
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
                MessageBox.Show(String.Format(Resources.Error_getting_data_error_message, e.Message));
                return null;
            }

            if (applications == null)
            {
                MessageBox.Show(String.Format(Resources.No_app_data_transfered_error_message));
                return null;
            }

            if (session == null)
            {
                MessageBox.Show(String.Format(Resources.No_session_data_transfered_error_message));
                return null;
            }

            SetCookies(session);

            BrowserWindowsManager.Applications = applications;
            BrowserWindowsManager.Session = session;

            return mainApplicationClient;
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