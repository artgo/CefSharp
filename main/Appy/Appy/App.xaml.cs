using System;
using System.Threading;
using System.Windows;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.InteropAPI;
using AppDirect.WindowsClient.UI;

namespace AppDirect.WindowsClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private volatile Mutex _instanceMutex = null;
        private volatile MainWindow _mainWindow;
        private ThreadStart _ipcCommunicatorStart;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;
            _instanceMutex = new Mutex(true, @"AppDirect.WindowsClient Application Manager Mutex", out createdNew);
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

            ServiceLocator.LocalStorage.LoadStorage();
            _ipcCommunicatorStart = ServiceLocator.IpcCommunicator.Start;
            (new Thread(_ipcCommunicatorStart)).Start();
            if (ServiceLocator.LocalStorage.HasCredentials)
            {
                Helper.Authenticate();
            }

            var mainViewModel = new MainViewModel();
            mainViewModel.InitializeAppsLists();
            _mainWindow = new MainWindow(mainViewModel);
            UpdateManager.Start(_mainWindow);
            AppSessionRefresher.Start(_mainWindow);

            var taskbarPanel = new TaskbarPanel(_mainWindow);
            taskbarPanel.InitializeButtons(TaskbarApi.Instance.TaskbarPosition, TaskbarApi.Instance.TaskbarIconsSize);
            TaskbarApi.Instance.InsertTaskbarWindow(taskbarPanel, taskbarPanel, taskbarPanel.GetCurrentDimension());

            if (!ServiceLocator.LocalStorage.IsLoadedFromFile)
            {
                _mainWindow.Show();
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_instanceMutex != null)
            {
                _instanceMutex.ReleaseMutex();
                ServiceLocator.IpcCommunicator.Stop();
                UpdateManager.Stop();
                AppSessionRefresher.Stop();
                ServiceLocator.BrowserWindowsCommunicator.Stop();
            }

            base.OnExit(e);

            _ipcCommunicatorStart = null;
        }

        private void App_OnDeactivated(object sender, EventArgs e)
        {
            _mainWindow.Hide();
        }
    }
}
