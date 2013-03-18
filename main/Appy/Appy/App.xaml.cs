using System;
using System.Threading;
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
        private Mutex _instanceMutex = null;
        private MainWindow _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;
            _instanceMutex = new Mutex(true, @"AppDirect.WindowsClient Application Manager Mutex ", out createdNew);
            if (!createdNew)
            {
                _instanceMutex = null;
                Current.Shutdown();
                return;
            }

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
            UpdateManager.Start(_mainWindow);
            AppSessionRefresher.Start(_mainWindow);

            var taskbarPanel = new TaskbarPanel(_mainWindow);
            taskbarPanel.InitializeButtons(TaskbarApi.Instance.TaskbarPosition, TaskbarApi.Instance.TaskbarIconsSize);
            TaskbarApi.Instance.InsertTaskbarWindow(taskbarPanel, taskbarPanel, taskbarPanel.GetCurrentDimension());

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_instanceMutex != null)
            {
                _instanceMutex.ReleaseMutex();
                ServiceLocator.IpcCommunicator.Exit();
                UpdateManager.Stop();
                AppSessionRefresher.Stop();
            }

            base.OnExit(e);
        }

        private void App_OnDeactivated(object sender, EventArgs e)
        {
            _mainWindow.Hide();
        }
    }
}
