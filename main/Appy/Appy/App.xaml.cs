using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.InteropAPI;
using AppDirect.WindowsClient.UI;
using System;
using System.Threading;
using System.Windows;

namespace AppDirect.WindowsClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private readonly UnhandledExceptionEventHandler _exceptionHandler;
        private readonly ILogger _log = new NLogLogger("MainApp");
        private volatile Mutex _instanceMutex = null;
        private volatile MainWindow _mainWindow;
        private volatile ThreadStart _ipcCommunicatorStart;

        public App()
        {
            _exceptionHandler = CurrentDomainOnUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += _exceptionHandler;

            var startTicks = Environment.TickCount;
            bool createdNew;
            _instanceMutex = new Mutex(true, @"AppDirect.WindowsClient Application Manager Mutex", out createdNew);
            if (!createdNew)
            {
                _log.Info("Instance already exists, exit.");
                _instanceMutex = null;
                Current.Shutdown();
                Environment.Exit(0);
            }

            try
            {
                ServiceLocator.Initialize();
            }
            catch (Exception ex)
            {
                _log.ErrorException("Failed to initialize", ex);
                MessageBox.Show(ex.Message);
            }

            ServiceLocator.LocalStorage.LoadStorage();
            _ipcCommunicatorStart = ServiceLocator.IpcCommunicator.Start;
            new Thread(_ipcCommunicatorStart).Start();

            var mainViewModel = new MainViewModel();
            mainViewModel.InitializeAppsLists();
            _mainWindow = new MainWindow(mainViewModel);
            UpdateManager.Start(_mainWindow);
            AppSessionRefresher.Start(_mainWindow);

            var taskbarPanel = new TaskbarPanel(_mainWindow);
            taskbarPanel.InitializeButtons(TaskbarApi.Instance.TaskbarPosition, TaskbarApi.Instance.TaskbarIconsSize);

            try
            {
                TaskbarApi.Instance.InsertTaskbarWindow(taskbarPanel, taskbarPanel, taskbarPanel.GetCurrentDimension());
            }
            catch (Exception ex)
            {
                _log.ErrorException("Failed to initialize taskbar module", ex);
                MessageBox.Show(ex.ToString());
                Environment.Exit(0);
            }

            ServiceLocator.UiHelper.IgnoreException(ServiceLocator.BrowserWindowsCommunicator.Start);

            if (!ServiceLocator.LocalStorage.IsLoadedFromFile)
            {
                _mainWindow.Show();
            }

            base.OnStartup(e);
            var stopTicks = Environment.TickCount;

            _log.Debug("Application startup completed in " + (stopTicks - startTicks) + "ms.");
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            _log.ErrorException("Exception during runtime", unhandledExceptionEventArgs.ExceptionObject as Exception);
            MessageBox.Show(unhandledExceptionEventArgs.ExceptionObject.ToString());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_instanceMutex != null)
            {
                ServiceLocator.UiHelper.IgnoreException(_instanceMutex.ReleaseMutex);
                ServiceLocator.UiHelper.IgnoreException(ServiceLocator.BrowserWindowsCommunicator.Stop);
                ServiceLocator.UiHelper.IgnoreException(ServiceLocator.IpcCommunicator.Stop);
                ServiceLocator.UiHelper.IgnoreException(UpdateManager.Stop);
                ServiceLocator.UiHelper.IgnoreException(AppSessionRefresher.Stop);
            }

            base.OnExit(e);

            _ipcCommunicatorStart = null;

            Environment.Exit(0);
        }

        private void App_OnDeactivated(object sender, EventArgs e)
        {
            _mainWindow.Hide();
        }
    }
}