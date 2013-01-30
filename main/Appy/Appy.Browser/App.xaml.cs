using System;
using System.ServiceModel;
using System.Windows;
using AppDirect.WindowsClient.Browser.API;
using AppDirect.WindowsClient.Browser.MainApp;
using AppDirect.WindowsClient.Browser.UI;
using AppDirect.WindowsClient.Common.API;
using CommandLine;

namespace AppDirect.WindowsClient.Browser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var options = new Options();

            if ((e != null) && (e.Args != null) && (e.Args.Length > 0) &&
                (CommandLineParser.Default.ParseArguments(e.Args, options)) &&
                !string.IsNullOrEmpty(options.AppId))
            {
                ProcessApplicationId(options);
            }
            else
            {
                StartupUri = new Uri("UI/ChromiumWindow.xaml");
            }

            base.OnStartup(e);
        }

        private void ProcessApplicationId(Options options)
        {
            MainApplicationClient client;
            try
            {
                var callback = new MainApplicationCallback();
                var context = new InstanceContext(callback);
                context.Faulted += ErrorOnServer;
                client = new MainApplicationClient(context);
            }
            catch (Exception)
            {
                MessageBox.Show("Communications can't be established");
                return;
            }

            IApplication app;
            IAppDirectSession session;
            try
            {
                app = (IApplication)client.GetApplicationById(options.AppId);
                session = (IAppDirectSession)client.GetCurrentSession();
            }
            catch (Exception)
            {
                MessageBox.Show("Error getting data");
                return;
            }

            if (app == null)
            {
                MessageBox.Show("No app data transfered for application with id " + options.AppId);
                return;
            }

            if (session == null)
            {
                MessageBox.Show("No session data transfered for application with id " + options.AppId);
                return;
            }

            if (string.IsNullOrEmpty(app.UrlString))
            {
                MessageBox.Show("Url for the application with id " + options.AppId + " is empty");
                return;
            }

            var window = new ChromiumWindow()
                {
                    UrlAddress = app.UrlString,
                    Session = session
                };
            MainWindow = window;
            window.Show();
        }

        private void ErrorOnServer(object sender, System.EventArgs e)
        {
            MessageBox.Show(e.ToString());
            throw new Exception();
        }
    }
}
