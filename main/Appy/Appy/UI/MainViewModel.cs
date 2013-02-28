using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Windows;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Properties;
using AppDirect.WindowsClient.Storage;
using Application = AppDirect.WindowsClient.Common.API.Application;

namespace AppDirect.WindowsClient.UI
{
    ///<summary>
    /// Represents the data and methods that are used to create the View
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private const int MyAppDisplayLimit = 10;
        private string _myAppsLoadError = String.Empty;
        private string _suggestedAppsLoadError = String.Empty;
        private string _loginFailedMessage = Properties.Resources.CredentialsProblemError;
        private string _loginHeaderText = Properties.Resources.LoginHeaderDefault;
        private bool _registrationInProgress = false;

        public EventHandler ApplicationAddedNotifier;
        public EventHandler ApplicationRemovedNotifier;
        
        public string VersionString
        {
            get
            {
                return Properties.Resources.AboutString + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string UpdateString
        {
            get
            {
                if (ServiceLocator.LocalStorage.UpdateDownloaded)
                {
                    return Properties.Resources.InstallUpdateString;
                }
                else
                {
                    return Properties.Resources.GetUpdateString;
                }
            }
            set
            {
                NotifyPropertyChanged("CloudSyncVisibility");
            }
        }

        public Visibility CloudSyncVisibility
        {
            get
            {
                if (ServiceLocator.LocalStorage.HasCredentials)
                {
                    return Visibility.Hidden;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            set
            {
                NotifyPropertyChanged("CloudSyncVisibility");
            }
        }

        public Visibility VerifyEmailVisibility
        {
            get
            {
                if (_registrationInProgress)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
            set
            {
                NotifyPropertyChanged("VerifyEmailVisibility");
            }
        }

        public Visibility LogOutVisibility
        {
            get
            {
                if (!ServiceLocator.LocalStorage.HasCredentials)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            set
            {
                NotifyPropertyChanged("LogOutVisibility");
            }
        }

        public string MyAppsLoadError
        {
            get { return _myAppsLoadError; }
            set
            {
                if (_myAppsLoadError == value)
                {
                    return;
                }

                _myAppsLoadError = value;
                NotifyPropertyChanged("MyAppsLoadError");
            }
        }

        public string SuggestedAppsLoadError
        {
            get { return _suggestedAppsLoadError;}
            set
            {
                _suggestedAppsLoadError = value;
                NotifyPropertyChanged("SuggestedAppsLoadError");
            }
        }
        
        public string LoginHeaderText
        {
            get { return _loginHeaderText; }
            set
            {
                _loginHeaderText = value;
                NotifyPropertyChanged("LoginHeaderText");
            }
        }

        public string LoginFailedMessage
        {
            get { return _loginFailedMessage; }
            set
            {
                _loginFailedMessage = value;
                NotifyPropertyChanged("LoginFailedMessage");
            }
        }

        public ObservableCollection<Application> MyApplications { get; set; }
        public ObservableCollection<Application> SuggestedApplications { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public void SyncAppsWithApi()
        {
            SyncMyApplications();
            GetSuggestedApplicationsWithApiCall();
        }

        public bool Login(string username, string password)
        {
            if (ServiceLocator.CachedAppDirectApi.Authenticate(username, password))
            {
                lock (ServiceLocator.LocalStorage.Locker)
                {
                    ServiceLocator.LocalStorage.SetCredentials(username, password);
                }

                SyncAppsWithApi();

                CloudSyncVisibility = Visibility.Hidden;
                LogOutVisibility = Visibility.Visible;
                return true;
            }

            return false;
        }

        public void Logout()
        {
            lock (ServiceLocator.LocalStorage.Locker)
            {
                ServiceLocator.CachedAppDirectApi.UnAuthenticate();
                ServiceLocator.LocalStorage.ClearLoginCredentials();
            }

            SyncAppsWithApi();
        }

        public void Install(Application application)
        {
            if (application.IsLocalApp)
            {
                RemoveFromSuggestedApps(application, false);
                AddToMyApps(application, false);
                lock (ServiceLocator.LocalStorage.Locker)
                {
                    ServiceLocator.LocalStorage.SaveAppSettings();
                }
            }
            else
            {
                System.Diagnostics.Process.Start(Properties.Resources.InstallAppTarget + application.Id);
            }
        }

        public void Uninstall(Application application)
        {
            if (application.IsLocalApp)
            {
                RemoveFromMyApps(application, false);
                AddToSuggestedApps(application, false);
                lock (ServiceLocator.LocalStorage.Locker)
                {
                    ServiceLocator.LocalStorage.SaveAppSettings();
                }
            }
            else
            {
                MessageBox.Show(application.Name + " can not be removed.");
            }
        }

        public void InitializeAppsLists()
        {
            lock (ServiceLocator.LocalStorage.Locker)
            {
                if (!ServiceLocator.LocalStorage.InstalledLocalApps.Contains(LocalApplications.AppStoreApp))
                {
                    ServiceLocator.LocalStorage.InstalledLocalApps.Insert(0, LocalApplications.AppStoreApp);
                }

                ServiceLocator.LocalStorage.LastSuggestedApps.RemoveAll(
                    a => ServiceLocator.LocalStorage.AllInstalledApplications.Contains(a));

                var missingLocalApps =
                    LocalApplications.LocalApplicationsList.Except(ServiceLocator.LocalStorage.LastSuggestedApps)
                                     .Except(ServiceLocator.LocalStorage.AllInstalledApplications)
                                     .ToList();

                ServiceLocator.LocalStorage.LastSuggestedApps.AddRange(missingLocalApps);

                MyApplications =
                    new ObservableCollection<Application>(
                        ServiceLocator.LocalStorage.AllInstalledApplications.Take(MyAppDisplayLimit));
                SuggestedApplications =
                    new ObservableCollection<Application>(ServiceLocator.LocalStorage.LastSuggestedApps);

            }
        }

        private void SyncMyApplications()
        {
            try
            {
                var apiApps = new List<Application>();

                if (Helper.Authenticate())
                {
                    apiApps = ServiceLocator.CachedAppDirectApi.MyApps.ToList();
                }

                var newApps = apiApps.Except(MyApplications).ToList();
                var expiredApps = MyApplications.Where(a => !a.IsLocalApp).Except(apiApps).ToList();

                foreach (var application in expiredApps)
                {
                    RemoveFromMyApps(application, false);
                }

                foreach (var application in newApps)
                {
                    application.LocalImagePath = ServiceLocator.LocalStorage.SaveAppIcon(application.ImagePath,
                                                                                            application.Id);
                    AddToMyApps(application, false);
                }

                lock (ServiceLocator.LocalStorage.Locker)
                {
                    ServiceLocator.LocalStorage.SaveAppSettings();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void GetSuggestedApplicationsWithApiCall()
        {
            try
            {
                lock (ServiceLocator.LocalStorage.Locker)
                {
                    var apiSuggestedApps =
                        ServiceLocator.CachedAppDirectApi.SuggestedApps.Except(
                            ServiceLocator.LocalStorage.AllInstalledApplications).ToList();


                    apiSuggestedApps.RemoveAll(a => !a.Price.Contains("Free"));

                    var newApps = apiSuggestedApps.Except(SuggestedApplications.Where(a => !a.IsLocalApp)).ToList();
                    var expiredApps = SuggestedApplications.Where(a => !a.IsLocalApp).Except(apiSuggestedApps).ToList();

                    foreach (var application in expiredApps)
                    {
                        RemoveFromSuggestedApps(application, false);
                    }

                    foreach (var application in newApps)
                    {
                        application.LocalImagePath = ServiceLocator.LocalStorage.SaveAppIcon(application.ImagePath,
                                                                                             application.Id);
                        AddToSuggestedApps(application, false);
                    }

                    ServiceLocator.LocalStorage.SaveAppSettings();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void AddToMyApps(Application application, bool saveLocalStorage = true)
        {
            application.PinnedToTaskbar = true;

            lock (ServiceLocator.LocalStorage.Locker)
            {
                if (application.IsLocalApp)
                {

                    ServiceLocator.LocalStorage.InstalledLocalApps.Add(application);
                }

                else
                {

                    ServiceLocator.LocalStorage.InstalledAppDirectApps.Add(application);
                }

                if (saveLocalStorage)
                {
                    ServiceLocator.LocalStorage.SaveAppSettings();
                }
            }

            if (System.Windows.Application.Current == null || Thread.CurrentThread == System.Windows.Application.Current.Dispatcher.Thread)
            {
                MyApplications.Add(application);
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(
                    new Action(() => MyApplications.Add(application)));
            }

            if (ApplicationAddedNotifier != null)
            {
                ApplicationAddedNotifier(application, null);
            }
        }

        private void RemoveFromMyApps(Application application, bool saveLocalStorage = true)
        {
            lock (ServiceLocator.LocalStorage.Locker)
            {
                if (application.IsLocalApp)
                {
                    ServiceLocator.LocalStorage.InstalledLocalApps.Remove(application);
                }
                else
                {
                    ServiceLocator.LocalStorage.InstalledAppDirectApps.Remove(application);
                }

                if (saveLocalStorage)
                {
                    ServiceLocator.LocalStorage.SaveAppSettings();
                }
            }

            if (System.Windows.Application.Current == null || Thread.CurrentThread == System.Windows.Application.Current.Dispatcher.Thread)
            {
                MyApplications.Remove(application);
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(
                    new Action(() => MyApplications.Remove(application)));
            }

            if (ApplicationRemovedNotifier != null)
            {
                ApplicationRemovedNotifier(application, null);
            }
        }

        private void AddToSuggestedApps(Application application, bool saveLocalStorage = true)
        {
            application.PinnedToTaskbar = false;

            lock (ServiceLocator.LocalStorage.Locker)
            {
                ServiceLocator.LocalStorage.LastSuggestedApps.Add(application);

                if (saveLocalStorage)
                {
                    ServiceLocator.LocalStorage.SaveAppSettings();
                }
            }

            if (System.Windows.Application.Current == null || Thread.CurrentThread == System.Windows.Application.Current.Dispatcher.Thread)
            {
                SuggestedApplications.Add(application);
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => SuggestedApplications.Add(application)));
            }
        }

        private void RemoveFromSuggestedApps(Application application, bool saveLocalStorage = true)
        {
            lock (ServiceLocator.LocalStorage.Locker)
                {
            ServiceLocator.LocalStorage.LastSuggestedApps.Remove(application);

            if (saveLocalStorage)
            {
                ServiceLocator.LocalStorage.SaveAppSettings();
            }
                }

            if (System.Windows.Application.Current == null || Thread.CurrentThread == System.Windows.Application.Current.Dispatcher.Thread)
            {
                SuggestedApplications.Remove(application);
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(
                    new Action(() => SuggestedApplications.Remove(application)));
            }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
