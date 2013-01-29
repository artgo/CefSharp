using System;
using System.ServiceModel;
using System.Windows;

namespace AppDirect.WindowsClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private volatile ServiceHost _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                ServiceLocator.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            _host = new ServiceHost(typeof(MainApplication));
            _host.Open();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.Close();

            base.OnExit(e);
        }
    }
}
