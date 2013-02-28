using System;
using System.Windows;
using AppDirect.WindowsClient.InteropAPI;
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
            ServiceLocator.LocalStorage.LoadStorage();

            var mainViewModel = new MainViewModel();
            mainViewModel.InitializeAppsLists();
            mainViewModel.SyncAppsWithApi();
            var mainWindow = new MainWindow(mainViewModel);
            UpdateDownloader.Start(mainWindow);
            AppSessionRefresher.Start(mainWindow);
            
            var taskbarPanel = new TaskbarPanel(mainWindow);
            taskbarPanel.InitializeButtons(TaskbarApi.Instance.TaskbarPosition, TaskbarApi.Instance.TaskbarIconsSize);
            TaskbarApi.Instance.InsertTaskbarWindow(taskbarPanel, taskbarPanel, (int)taskbarPanel.Width);

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceLocator.IpcCommunicator.Exit();
            UpdateDownloader.Stop();
            AppSessionRefresher.Stop();
            TaskbarApi.Instance.RemoveTaskbarWindow();
            TaskbarApi.Cleanup();

            base.OnExit(e);
        }
    }
}
