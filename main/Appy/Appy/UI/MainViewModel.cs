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
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
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

        public MainViewModel()
        {
            InitializeAppsLists();
            SyncAppsWithApi();
        }
        
        private void InitializeAppsLists()
        {
            if (!ServiceLocator.LocalStorage.InstalledLocalApps.Contains(LocalApplications.AppStoreApp))
            {
                ServiceLocator.LocalStorage.InstalledLocalApps.Insert(0, LocalApplications.AppStoreApp);
            }

            ServiceLocator.LocalStorage.LastSuggestedApps.RemoveAll(a => ServiceLocator.LocalStorage.AllInstalledApplications.Contains(a));

            var missingLocalApps = LocalApplications.LocalApplicationsList.Except(ServiceLocator.LocalStorage.LastSuggestedApps).Except(ServiceLocator.LocalStorage.AllInstalledApplications).ToList();

            ServiceLocator.LocalStorage.LastSuggestedApps.AddRange(missingLocalApps);

            MyApplications = new ObservableCollection<Application>(ServiceLocator.LocalStorage.AllInstalledApplications.Take(MyAppDisplayLimit));
            SuggestedApplications = new ObservableCollection<Application>(ServiceLocator.LocalStorage.LastSuggestedApps);
        }

        public void SyncMyApplications()
        {
            try
            {
                var apiApps = new List<Application>();

                if (AuthenticateSession())
                {
                    apiApps = ServiceLocator.CachedAppDirectApi.MyApps.ToList();
                }

                var newApps = apiApps.Except(ServiceLocator.LocalStorage.InstalledAppDirectApps).ToList();
                var expiredApps = ServiceLocator.LocalStorage.InstalledAppDirectApps.Except(apiApps).ToList();

                foreach (var application in expiredApps)
                {
                    RemoveAppFromMyApps(application, false);
                }

                foreach (var application in newApps)
                {
                    application.LocalImagePath = ServiceLocator.LocalStorage.SaveAppIcon(application.ImagePath,
                                                                                            application.Id);
                    AddAppToMyApps(application, false);
                }

                ServiceLocator.LocalStorage.SaveAppSettings();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// MUST BE WRAPPED IN TRY-CATCH Throws exceptions for network errors or API Errors
        /// </summary>
        /// <returns></returns>
        private bool AuthenticateSession()
        {
            if (ServiceLocator.LocalStorage.HasCredentials)
            {
                if (ServiceLocator.CachedAppDirectApi.IsAuthenticated)
                {
                    return true;
                }
                if (ServiceLocator.CachedAppDirectApi.Authenticate(ServiceLocator.LocalStorage.LoginInfo.Username,
                                                                   ServiceLocator.LocalStorage.LoginInfo.Password))
                {
                    return true;
                }
            }

            ServiceLocator.LocalStorage.ClearLoginCredentials();
            return false;
        }
        
        public void SyncAppsWithApi()
        {
            SyncMyApplications();
            GetSuggestedApplicationsWithApiCall();
        }

        public void GetSuggestedApplicationsWithApiCall()
        {
            try
            {
                var apiSuggestedApps = ServiceLocator.CachedAppDirectApi.SuggestedApps.Except(ServiceLocator.LocalStorage.AllInstalledApplications).ToList();
                
                var newApps = apiSuggestedApps.Except(ServiceLocator.LocalStorage.LastSuggestedApps).ToList();
                var expiredApps = ServiceLocator.LocalStorage.LastSuggestedApps.Where(a => !a.IsLocalApp).Except(apiSuggestedApps).ToList();

                foreach (var application in expiredApps)
                {
                    RemoveAppFromSuggestedApps(application, false);
                }

                foreach (var application in newApps)
                {
                    application.LocalImagePath = ServiceLocator.LocalStorage.SaveAppIcon(application.ImagePath,
                                                                                            application.Id);
                    AddAppToSuggestedApps(application, false);
                }

                ServiceLocator.LocalStorage.SaveAppSettings();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public bool Login(string username, string password)
        {
            if (ServiceLocator.CachedAppDirectApi.Authenticate(username, password))
            {
                ServiceLocator.LocalStorage.SetCredentials(username, password);

                SyncAppsWithApi();

                CloudSyncVisibility = Visibility.Hidden;
                LogOutVisibility = Visibility.Visible;
                return true;
            }

            return false;
        }

        public void Uninstall(Application application)
        {
            if (application.IsLocalApp)
            {
                application.PinnedToTaskbar = false;
                RemoveAppFromMyApps(application);
                AddAppToSuggestedApps(application);
            }
            else
            {
                MessageBox.Show(application.Name + " can not be removed.");
            }
        }

        public void Install(Application application)
        {
            if (application.IsLocalApp)
            {
                AddAppToMyApps(application);
                RemoveAppFromSuggestedApps(application);
            }
            else
            {
                System.Diagnostics.Process.Start(Properties.Resources.InstallAppTarget + application.Id);
            }
        }

        private void RemoveAppFromSuggestedApps(Application application, bool saveLocalStorage = true)
        {
            ServiceLocator.LocalStorage.LastSuggestedApps.Remove(application);

            if (saveLocalStorage)
            {
                ServiceLocator.LocalStorage.SaveAppSettings();
            }

            if (Thread.CurrentThread == System.Windows.Application.Current.Dispatcher.Thread)
            {
                SuggestedApplications.Remove(application);
            }
            else if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(
                    new Action(() => SuggestedApplications.Remove(application)));
            }
        }

        private void AddAppToMyApps(Application application, bool saveLocalStorage = true)
        {
            application.PinnedToTaskbar = true;

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

            if (Thread.CurrentThread == System.Windows.Application.Current.Dispatcher.Thread)
            {
                MyApplications.Add(application);
            }
            else if (System.Windows.Application.Current != null )
            {
                System.Windows.Application.Current.Dispatcher.Invoke(
                    new Action(() => MyApplications.Add(application)));
            }

            if (ApplicationAddedNotifier != null)
            {
                ApplicationAddedNotifier(application, null);
            }
        }

        private void RemoveAppFromMyApps(Application application, bool saveLocalStorage = true)
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

            if (System.Windows.Application.Current != null && MyApplications.Count < MyAppDisplayLimit)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(
                    new Action(() => MyApplications.Remove(application)));
            }

            if (ApplicationRemovedNotifier != null)
            {
                ApplicationRemovedNotifier(application, null);
            }
        }

        private void AddAppToSuggestedApps(Application application, bool saveLocalStorage = true)
        {
            application.PinnedToTaskbar = false;

            ServiceLocator.LocalStorage.LastSuggestedApps.Add(application);

            if (saveLocalStorage)
            {
                ServiceLocator.LocalStorage.SaveAppSettings();
            }

            if (System.Windows.Application.Current != null && MyApplications.Count < MyAppDisplayLimit)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(
                    new Action(() => SuggestedApplications.Add(application)));
            }
        }

        public void Logout()
        {
            ServiceLocator.CachedAppDirectApi.UnAuthenticate();
            ServiceLocator.LocalStorage.ClearLoginCredentials();

            SyncAppsWithApi();
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
