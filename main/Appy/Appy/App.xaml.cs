using System;
using System.Windows;
using AppDirect.WindowsClient.InteropAPI;
using AppDirect.WindowsClient.InteropAPI.Internal;
using AppDirect.WindowsClient.UI;

namespace AppDirect.WindowsClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
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

            ServiceLocator.IpcCommunicator.Start();

            var size = NativeDll.GetInitialADButtonSize();

            var buttons = new Deskband();
            TaskbarApi.Instance.InsertTaskbarWindow(buttons, buttons, (int)buttons.Width);

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceLocator.IpcCommunicator.Exit();

            TaskbarApi.Instance.RemoveTaskbarWindow();
            TaskbarApi.Cleanup();

            base.OnExit(e);
        }
    }
}
