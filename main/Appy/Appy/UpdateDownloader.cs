using System;
using System.Threading;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.UI;

namespace AppDirect.WindowsClient
{
    public static class UpdateDownloader
    {
        private static readonly TimeSpan CheckForUpdatesTimeSpan = TimeSpan.FromDays(1);
        private static readonly TimeSpan DelayAfterStartup = TimeSpan.FromMinutes(5);
        private static readonly Thread GetUpdateThread = new Thread(DownloadAvailableUpdates);

        private static volatile MainWindow _mainWindow;

        private static void DownloadAvailableUpdates()
        {
            Thread.Sleep(DelayAfterStartup);

            while (true)
            {
                bool updateAvailable = ServiceLocator.Updater.GetUpdates(Helper.ApplicationVersion);

                if (updateAvailable)
                {
                    _mainWindow.UpdateAvailable(true);
                }

                Thread.Sleep(CheckForUpdatesTimeSpan);
            }
        }

        public static void Start(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            GetUpdateThread.Start();
        }

        public static void Stop()
        {
            if (GetUpdateThread.IsAlive)
            {
                GetUpdateThread.Abort();
            }
        }
    }
}