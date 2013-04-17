using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.UI;
using AppDirect.WindowsClient.UI;
using System;
using System.Threading;

namespace AppDirect.WindowsClient
{
    public static class UpdateManager
    {
        private static readonly TimeSpan CheckForUpdatesTimeSpan = TimeSpan.FromDays(1);
        private static readonly TimeSpan CheckForIdleTimeSpan = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan DelayAfterStartup = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan MinimumIdleInterval = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan MaximumWaitToUpdateInterval = TimeSpan.FromDays(1);
        private static readonly Thread DownloadUpdateThread = new Thread(ManageUpdate);

        private static volatile MainWindow _mainWindow;

        private static void ManageUpdate()
        {
            Thread.Sleep(DelayAfterStartup);

            while (true)
            {
                while (!ServiceLocator.LocalStorage.UpdateDownloaded)
                {
                    bool updateAvailable = ServiceLocator.Updater.GetUpdates(Helper.ApplicationVersion);

                    if (updateAvailable)
                    {
                        ServiceLocator.UiHelper.IgnoreException(() => Helper.PerformInUiThread(() => _mainWindow.ViewModel.ResetUpdateText()));
                        break;
                    }

                    ServiceLocator.UiHelper.Sleep(CheckForUpdatesTimeSpan);
                }

                ServiceLocator.UiHelper.IgnoreException(InstallUpdateOnIdle);
            }
        }

        private static void InstallUpdateOnIdle()
        {
            var updateStarted = Helper.PerformWhenIdle(() => ServiceLocator.Updater.InstallUpdates(), MinimumIdleInterval, CheckForIdleTimeSpan, MaximumWaitToUpdateInterval);

            if (!updateStarted)
            {
                ServiceLocator.Updater.InstallUpdates();
            }
        }

        public static void Start(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            DownloadUpdateThread.Start();
        }

        public static void Stop()
        {
            if (DownloadUpdateThread.IsAlive)
            {
                DownloadUpdateThread.Abort();
            }
        }
    }
}