using System;
using System.Threading;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.UI;

namespace AppDirect.WindowsClient
{
    public static class UpdateDownloader
    {
        private static volatile MainWindow _mainWindow;
        private static readonly TimeSpan CheckForUpdatesTimeSpan = TimeSpan.FromDays(1);

        private static void DownloadAvailableUpdates()
        {
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

            var getUpdateThread = new Thread(DownloadAvailableUpdates);
            getUpdateThread.Start();
        }
    }
}