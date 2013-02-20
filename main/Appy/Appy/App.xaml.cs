using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using AppDirect.WindowsClient.API;
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
        private MainWindow mainWindow;

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

            var mainWindow = new MainWindow();
            UpdateDownloader.Start(mainWindow);
            AppSessionRefresher.Start(mainWindow);
            var taskbarPanel = new TaskbarPanel(mainWindow);

            TaskbarApi.Instance.InsertTaskbarWindow(taskbarPanel, taskbarPanel, (int)taskbarPanel.Width);

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
