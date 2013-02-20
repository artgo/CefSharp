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
        private static MainWindow MainWindow { get; set; }

        private static void RefreshApps()
        {
            while (true)
            {
                Thread.Sleep(Helper.RefreshAppsTimeSpan);

                if (ServiceLocator.LocalStorage.HasCredentials && !ServiceLocator.CachedAppDirectApi.IsAuthenticated)
                {
                    try
                    {
                        if (!ServiceLocator.CachedAppDirectApi.Authenticate(ServiceLocator.LocalStorage.LoginInfo.Username,
                                                                            ServiceLocator.LocalStorage.LoginInfo.Password))
                        {
                            ServiceLocator.LocalStorage.ClearLoginCredentials();
                        }
                    }
                    catch (CryptographicException e)
                    {
                        ServiceLocator.LocalStorage.ClearLoginCredentials();
                        MessageBox.Show("Credentials were present, but there was an error decrypting: " + e.Message);
                    }
                    catch (Exception)
                    {
                        if (System.Windows.Application.Current != null)
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                                                                                            MainWindow.ViewModel.LoginFailedMessage = Resources.NetworkProblemError));
                        }
                    }
                }
                
                MainWindow.ViewModel.GetMyApplications();
                MainWindow.ViewModel.GetSuggestedApplicationsWithApiCall();
                ServiceLocator.LocalStorage.SaveAppSettings();
            }
        }

        public static void Start(MainWindow mainWindow)
        {
            MainWindow = mainWindow;

            var getUpdateThread = new Thread(RefreshApps);
            getUpdateThread.Start();
        }


    }
}
