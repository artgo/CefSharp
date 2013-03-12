using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
        private Visibility _updateSpinnerVisibility = Visibility.Hidden;

        private Visibility _updateAvailableVisibility = ServiceLocator.LocalStorage.UpdateDownloaded
                                                            ? Visibility.Visible
                                                            : Visibility.Collapsed;

        private string _updateString = ServiceLocator.LocalStorage.UpdateDownloaded
                                           ? Properties.Resources.InstallUpdateString
                                           : Properties.Resources.GetUpdateString;

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
            get { return _updateString; }
            set
            {
                _updateString = value;
                NotifyPropertyChanged("UpdateString");
            }
        }
        
        public Visibility UpdateSpinnerVisibility
        {
            get { return _updateSpinnerVisibility; }
            set
            {
                _updateSpinnerVisibility = value;
                NotifyPropertyChanged("UpdateSpinnerVisibility");
            }
        }

        public string UpdateAvailableString
        {
            get { return Resources.UpdateAvailableString; }
            set { NotifyPropertyChanged("UpdateAvailableString"); }
        }

        public Visibility UpdateAvailableVisibility
        {
            get { return _updateAvailableVisibility; }
            set
            {
                _updateAvailableVisibility = value;
                NotifyPropertyChanged("UpdateAvailableVisibility");
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

        public ObservableCollection<ApplicationViewModel> MyApplications { get; set; }
        public ObservableCollection<ApplicationViewModel> SuggestedApplications { get; set; }

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

        public void Install(ApplicationViewModel applicationViewModel)
        {
            if (applicationViewModel.Application.IsLocalApp)
            {
                RemoveFromSuggestedApps(applicationViewModel, false);
                AddToMyApps(applicationViewModel, false);
                lock (ServiceLocator.LocalStorage.Locker)
                {
                    ServiceLocator.LocalStorage.SaveAppSettings();
                }
            }
            else
            {
                System.Diagnostics.Process.Start(Properties.Resources.InstallAppTarget + applicationViewModel.Application.Id);
            }
        }

        public void Uninstall(ApplicationViewModel applicationViewModel)
        {
            if (applicationViewModel.Application.IsLocalApp)
            {
                RemoveFromMyApps(applicationViewModel, false);
                AddToSuggestedApps(applicationViewModel, false);
                lock (ServiceLocator.LocalStorage.Locker)
                {
                    ServiceLocator.LocalStorage.SaveAppSettings();
                }
            }
            else
            {
                MessageBox.Show(applicationViewModel.Application.Name + " can not be removed.");
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
                    new ObservableCollection<ApplicationViewModel>();
                SuggestedApplications =
                    new ObservableCollection<ApplicationViewModel>();

                foreach (var installedApps in ServiceLocator.LocalStorage.AllInstalledApplications.Take(MyAppDisplayLimit))
                {
                    MyApplications.Add(new ApplicationViewModel(installedApps));
                } 

                foreach (var lastSuggestedApp in ServiceLocator.LocalStorage.LastSuggestedApps)
                {
                    SuggestedApplications.Add(new ApplicationViewModel(lastSuggestedApp));
                }
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
                
                var displayedApps = MyApplications.Select(a => a.Application).ToList();

                var newApps = apiApps.Except(displayedApps).ToList();
                var expiredApps = displayedApps.Where(a => !a.IsLocalApp).Except(apiApps).ToList();

                foreach (var application in expiredApps)
                {
                    var applicationViewModel = MyApplications.First(a => a.Application.Equals(application));
                    RemoveFromMyApps(applicationViewModel, false);
                }

                foreach (var application in newApps)
                {
                    application.LocalImagePath = ServiceLocator.LocalStorage.SaveAppIcon(application.ImagePath,
                                                                                            application.Id);
                    AddToMyApps(new ApplicationViewModel(application), false);
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

                    var displayedApps = SuggestedApplications.Select(a => a.Application).ToList();

                    var newApps = apiSuggestedApps.Except(displayedApps.Where(a => !a.IsLocalApp)).ToList();
                    var expiredApps = displayedApps.Where(a => !a.IsLocalApp).Except(apiSuggestedApps).ToList();

                    foreach (var application in expiredApps)
                    {
                        var applicationViewModel = SuggestedApplications.First(a => a.Application.Equals(application));
                        RemoveFromSuggestedApps(applicationViewModel, false);
                    }

                    foreach (var application in newApps)
                    {
                        application.LocalImagePath = ServiceLocator.LocalStorage.SaveAppIcon(application.ImagePath,
                                                                                             application.Id);
                        AddToSuggestedApps(new ApplicationViewModel(application), false);
                    }

                    ServiceLocator.LocalStorage.SaveAppSettings();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void AddToMyApps(ApplicationViewModel applicationViewModel, bool saveLocalStorage = true)
        {
            var application = applicationViewModel.Application;
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

            Helper.PerformInUiThread(() => MyApplications.Add(applicationViewModel));
            
            if (ApplicationAddedNotifier != null)
            {
                ApplicationAddedNotifier(applicationViewModel, null);
            }
        }

        private void RemoveFromMyApps(ApplicationViewModel applicationViewModel, bool saveLocalStorage = true)
        {
            lock (ServiceLocator.LocalStorage.Locker)
            {
                if (applicationViewModel.Application.IsLocalApp)
                {
                    ServiceLocator.LocalStorage.InstalledLocalApps.Remove(applicationViewModel.Application);
                }
                else
                {
                    ServiceLocator.LocalStorage.InstalledAppDirectApps.Remove(applicationViewModel.Application);
                }

                if (saveLocalStorage)
                {
                    ServiceLocator.LocalStorage.SaveAppSettings();
                }
            }

            Helper.PerformInUiThread(() => MyApplications.Remove(applicationViewModel));
            
            if (ApplicationRemovedNotifier != null)
            {
                ApplicationRemovedNotifier(applicationViewModel, null);
            }
        }

        private void AddToSuggestedApps(ApplicationViewModel applicationViewModel, bool saveLocalStorage = true)
        {
            applicationViewModel.PinnedToTaskbarNotifier = false;

            lock (ServiceLocator.LocalStorage.Locker)
            {
                ServiceLocator.LocalStorage.LastSuggestedApps.Add(applicationViewModel.Application);

                if (saveLocalStorage)
                {
                    ServiceLocator.LocalStorage.SaveAppSettings();
                }
            }

            Helper.PerformInUiThread(() => SuggestedApplications.Add(applicationViewModel));
        }

        private void RemoveFromSuggestedApps(ApplicationViewModel applicationViewModel, bool saveLocalStorage = true)
        {
            applicationViewModel.PinnedToTaskbarNotifier = false;
            lock (ServiceLocator.LocalStorage.Locker)
            {
                ServiceLocator.LocalStorage.LastSuggestedApps.Remove(applicationViewModel.Application);

                if (saveLocalStorage)
                {
                    ServiceLocator.LocalStorage.SaveAppSettings();
                }
            }

            Helper.PerformInUiThread(() => SuggestedApplications.Remove(applicationViewModel));
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void UpdateClick()
        {
            if (ServiceLocator.LocalStorage.UpdateDownloaded)
            {
                ServiceLocator.Updater.InstallUpdates();
            }
            else
            {
                UpdateSpinnerVisibility = Visibility.Visible;

                UpdateString = Resources.CheckingForUpdatesString;

                var backGroundWorker = new BackgroundWorker();
                backGroundWorker.DoWork += CheckForAvailableUpdate;
                backGroundWorker.RunWorkerAsync();
            }
        }

        private void CheckForAvailableUpdate(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var updateAvailable = false;

            const int millisecondsToDisplayCheckingString = 1000;
            Helper.PerformForMinimumTime(() => { updateAvailable = ServiceLocator.Updater.GetUpdates(Helper.ApplicationVersion); }, false, millisecondsToDisplayCheckingString);

            Helper.PerformInUiThread(() =>
                {
                    UpdateString = updateAvailable ? Resources.InstallUpdateString : Resources.NoUpdateFoundString;
                    UpdateSpinnerVisibility = Visibility.Hidden;
                    UpdateAvailableVisibility = updateAvailable ? Visibility.Visible : Visibility.Collapsed;
                });
        }

        public void ResetUpdateText()
        {
            UpdateString =  ServiceLocator.LocalStorage.UpdateDownloaded
                                         ? Properties.Resources.InstallUpdateString
                                         : Properties.Resources.GetUpdateString;
        }

        public void ShowAboutDialog()
        {
            MessageBox.Show(VersionString);
        }
    }
}
