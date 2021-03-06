﻿using System;
using System.Threading;

namespace AppDirect.WindowsClient.UI
{
    public static class AppSessionRefresher
    {
        private static readonly TimeSpan RefreshAppsTimeSpan = TimeSpan.FromMinutes(25);
        private static readonly Thread RefreshAppSessionThread = new Thread(RefreshApps);

        private static volatile MainWindow _mainWindow;

        private static void RefreshApps()
        {
            while (true)
            {
                _mainWindow.ViewModel.SyncMyApplications(false, true);
                _mainWindow.ViewModel.GetSuggestedApplicationsWithApiCall();

                Thread.Sleep(RefreshAppsTimeSpan);
            }
        }

        public static void Start(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            RefreshAppSessionThread.Start();
        }

        public static void Stop()
        {
            if (RefreshAppSessionThread.IsAlive)
            {
                RefreshAppSessionThread.Abort();
            }
        }
    }
}
