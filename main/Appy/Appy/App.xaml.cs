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
        private MainWindow _mainWindow;

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
            _mainWindow = new MainWindow(mainViewModel);
            UpdateDownloader.Start(_mainWindow);
            AppSessionRefresher.Start(_mainWindow);

            var taskbarPanel = new TaskbarPanel(_mainWindow);
            taskbarPanel.InitializeButtons(TaskbarApi.Instance.TaskbarPosition, TaskbarApi.Instance.TaskbarIconsSize);
            TaskbarApi.Instance.InsertTaskbarWindow(taskbarPanel, taskbarPanel, taskbarPanel.GetCurrentDimension());

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceLocator.IpcCommunicator.Exit();
            UpdateDownloader.Stop();
            AppSessionRefresher.Stop();

            base.OnExit(e);
        }

        private void App_OnDeactivated(object sender, EventArgs e)
        {
            _mainWindow.Hide();
        }
    }
}
