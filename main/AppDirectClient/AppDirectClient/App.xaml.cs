using System.Diagnostics;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.InteropAPI;
using AppDirect.WindowsClient.InteropAPI.Internal;
using AppDirect.WindowsClient.UI;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private volatile ILatch _mainWindowReadyLatch = new Latch();
        private volatile ProcessWatcher _watcher;

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
                ServiceLocator.UiHelper.IgnoreException(_instanceMutex.ReleaseMutex);
                _instanceMutex = null;
                Current.Shutdown();
                Environment.Exit(0);
            }

            var helper = ServiceLocator.UiHelper;

            ServiceLocator.LocalStorage.LoadStorage();
            helper.StartAsynchronously(ServiceLocator.IpcCommunicator.Start);

            var mainViewModel = new MainViewModel();
            mainViewModel.InitializeAppsLists();

            var taskbarPanel = new TaskbarPanel(_mainWindowReadyLatch, new NLogLogger("TaskbarPanel"), mainViewModel);

            taskbarPanel.InitializeButtons(TaskbarApi.Instance.TaskbarPosition, TaskbarApi.Instance.TaskbarIconsSize);

            InsertTaskbarWindow(taskbarPanel);

            helper.StartAsynchronously(() => ServiceLocator.UiHelper.IgnoreException(ServiceLocator.BrowserWindowsCommunicator.Start));

            helper.StartAsynchronously(() => InitializeMainWindow(mainViewModel, taskbarPanel));

            base.OnStartup(e);

            var timeElapsed = Environment.TickCount - startTicks;
            helper.StartAsynchronously(() => _log.Warn("Application startup completed in " + timeElapsed + "ms."));
            ServiceLocator.Analytics.Notify("ClientStarted", "StartedIn", timeElapsed);

            _watcher = new ProcessWatcher("BrowserManager");
            _watcher.Start();

            ExplorerWatcher.Start(() => Helper.PerformInUiThread(() =>
                {
                    InsertTaskbarWindow(taskbarPanel);
                }
            ));
        }

        private void InsertTaskbarWindow(TaskbarPanel taskbarPanel)
        {
            try
            {
                TaskbarApi.Instance.InsertTaskbarWindow(taskbarPanel, taskbarPanel, taskbarPanel.GetCurrentDimension());
            }
            catch (Exception ex)
            {
                _log.ErrorException("Failed to initialize taskbar module", ex);
                MessageBox.Show(ex.ToString());
                ServiceLocator.UiHelper.IgnoreException(_instanceMutex.ReleaseMutex);
                _instanceMutex = null;
                Current.Shutdown();
                Environment.Exit(0);
            }
        }

        private void InitializeMainWindow(MainViewModel mainViewModel, TaskbarPanel taskbarPanel)
        {
            ServiceLocator.UiHelper.PerformInUiThread(() => _mainWindow = new MainWindow(mainViewModel, taskbarPanel));
            UpdateManager.Start(_mainWindow);
            AppSessionRefresher.Start(_mainWindow);
            taskbarPanel.ApplicationWindow = _mainWindow;
            _mainWindowReadyLatch.Unlock();

            if (!ServiceLocator.LocalStorage.IsLoadedFromFile)
            {
                ServiceLocator.UiHelper.PerformInUiThread(() => _mainWindow.Show());
            }

            ResurrectBrowserWindows();
        }

        private void ResurrectBrowserWindows()
        {
            if ((ServiceLocator.LocalStorage.AppsToReopen != null) && ServiceLocator.LocalStorage.AppsToReopen.Any() && ServiceLocator.LocalStorage.HasCredentials)
            {
                IDictionary<string, Common.API.IApplication> apps = new Dictionary<string, Common.API.IApplication>();
                lock (ServiceLocator.LocalStorage.Locker)
                {
                    foreach (var application in ServiceLocator.LocalStorage.AllInstalledApplications)
                    {
                        apps[application.Id] = application;
                    }
                }

                var appsWithData = new List<IApplicationWithState>();

                foreach (var app in ServiceLocator.LocalStorage.AppsToReopen)
                {
                    if (apps.Keys.Contains(app.ApplicationId))
                    {
                        var appWithData = new ApplicationWithState()
                            {
                                Application = apps[app.ApplicationId],
                                WindowState = app.WindowState
                            };

                        appsWithData.Add(appWithData);
                    }
                }

                ServiceLocator.BrowserWindowsCommunicator.DisplayApplications(appsWithData);
            }

            ServiceLocator.LocalStorage.AppsToReopen = null;
            ServiceLocator.UiHelper.StartAsynchronously(() => ServiceLocator.LocalStorage.SaveAppSettings());
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            _log.ErrorException("Exception during runtime", unhandledExceptionEventArgs.ExceptionObject as Exception);
            ServiceLocator.Analytics.Notify("Crash", unhandledExceptionEventArgs.ExceptionObject.GetType().ToString(), 0);
            MessageBox.Show(unhandledExceptionEventArgs.ExceptionObject.ToString());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_instanceMutex != null)
            {
                ServiceLocator.UiHelper.IgnoreException(_watcher.Stop);
                ServiceLocator.UiHelper.IgnoreException(_instanceMutex.ReleaseMutex);
                ServiceLocator.UiHelper.IgnoreException(ServiceLocator.BrowserWindowsCommunicator.CloseBrowserProcess);
                ServiceLocator.UiHelper.IgnoreException(ServiceLocator.BrowserWindowsCommunicator.Stop);
                ServiceLocator.UiHelper.IgnoreException(ServiceLocator.IpcCommunicator.Stop);
                ServiceLocator.UiHelper.IgnoreException(UpdateManager.Stop);
                ServiceLocator.UiHelper.IgnoreException(AppSessionRefresher.Stop);
            }

            base.OnExit(e);

            Environment.Exit(0);
        }

        private void App_OnDeactivated(object sender, EventArgs e)
        {
            if (_mainWindow != null)
            {
                _mainWindow.Hide();
            }
        }
    }
}