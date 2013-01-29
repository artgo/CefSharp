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
                (CommandLineParser.Default.ParseArguments(e.Args, options)))
            {
                var appId = options.AppId;
                var callback = new MainApplicationCallback();
                var context = new InstanceContext(callback);
                context.Faulted += ErrorOnServer;
                var client = new MainApplicationClient(context);
                var app = (IApplication) client.GetApplicationById(appId);

                var window = new ChromiumWindow()
                    {
                        UrlAddress = app.UrlString,
                        Session = (IAppDirectSession) client.GetCurrentSession()
                    };
                MainWindow = window;
                window.Show();
            }
            else
            {
                StartupUri = new Uri("UI/ChromiumWindow.xaml");
            }

            base.OnStartup(e);
        }

        private void ErrorOnServer(object sender, System.EventArgs e)
        {
            MessageBox.Show(e.ToString());
            throw new Exception();
        }
    }
}
