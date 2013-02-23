using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Properties;

namespace AppDirect.WindowsClient.UI
{
    public static class AppSessionRefresher
    {
        private const int RefreshAppsIntervalMins = 25;
        private static TimeSpan RefreshAppsTimeSpan = TimeSpan.FromMinutes(RefreshAppsIntervalMins);

        private static volatile MainWindow _mainWindow;
        
        private static void RefreshApps()
        {
            while (true)
            {
                Thread.Sleep(RefreshAppsTimeSpan);

                _mainWindow.ViewModel.SyncAppsWithApi();
            }
        }

        public static void Start(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            var getUpdateThread = new Thread(RefreshApps);
            getUpdateThread.Start();
        }
    }
}
