using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Application = AppDirect.WindowsClient.Common.API.Application;

namespace AppDirect.WindowsClient.UI
{
    ///<summary>
    /// Represents the data and methods that are used to create the View
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        public LoginViewModel LoginViewModel = new LoginViewModel();
        private string _message = String.Empty;
        private Visibility _updateSpinnerVisibility = Visibility.Hidden;
        private string _updateString = Properties.Resources.GetUpdateString;
        private bool _updateAvailable = false;
        private bool _registrationInProgress = false;
        public EventHandler ApplicationAddedNotifier;
        public EventHandler ApplicationRemovedNotifier;
        public EventHandler LogoutNotifier;
        private bool _isLoggedIn = ServiceLocator.LocalStorage.HasCredentials;

        private static readonly ILogger _log = new NLogLogger("MainViewModel");

        public string VersionString
        {
            get
            {
                return String.Format(Resources.AboutString + Assembly.GetExecutingAssembly().GetName().Version, Resources.ApplicationName);
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

        public bool UpdateAvailable
        {
            get { return _updateAvailable; }
            set
            {
                _updateAvailable = value;
                NotifyPropertyChanged("UpdateAvailable");
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

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                _isLoggedIn = value;
                NotifyPropertyChanged("IsLoggedIn");
                LoginViewModel.SetVisibility(_isLoggedIn);
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (_message == value)
                {
                    return;
                }

                _message = value;
                NotifyPropertyChanged("Message");
            }
        }

        public ObservableCollection<ApplicationViewModel> MyApplications { get; set; }

        public ObservableCollection<ApplicationViewModel> SuggestedApplications { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            LoginViewModel.SetVisibility(IsLoggedIn);
        }

        public void Logout()
        {
            IsLoggedIn = false;
            LogoutNotifier(null, null);

            ServiceLocator.UiHelper.StartAsynchronously(() =>
                {
                    ServiceLocator.BrowserWindowsCommunicator.CloseAllApplicationsAndRemoveSessionInfo();
                    lock (ServiceLocator.LocalStorage.Locker)
                    {
                        ServiceLocator.CachedAppDirectApi.UnAuthenticate();
                        ServiceLocator.LocalStorage.ClearLoginCredentials();
                    }
                    SyncMyApplications();
                });
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
                applicationViewModel.InProgressVisibility = Visibility.Visible;
                applicationViewModel.Application.ApplicationStatus = Status.AttempingProvisioning;
                RemoveFromSuggestedApps(applicationViewModel);
                AddToMyApps(applicationViewModel);

                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += SubscribeAsynchronously;
                backgroundWorker.RunWorkerAsync(applicationViewModel);
            }
        }

        private void SubscribeAsynchronously(object sender, DoWorkEventArgs e)
        {
            ApplicationViewModel applicationVM = (ApplicationViewModel)e.Argument;
            string errorInfo = String.Empty;

            try
            {
                var subscriptionId = Helper.AddApplication(applicationVM.Application.Id);
                if (!String.IsNullOrEmpty(subscriptionId))
                {
                    Helper.PerformInUiThread(() =>
                        {
                            applicationVM.Application.SubscriptionId = subscriptionId;
                            applicationVM.InProgressVisibility = Visibility.Collapsed;
                        });
                }
                else
                {
                    throw new InvalidDataException("API returned invalid SubscriptionId");
                }

                var storedApplication =
                    ServiceLocator.LocalStorage.InstalledAppDirectApps.FirstOrDefault(
                        a => a.Equals(applicationVM.Application));
                if (storedApplication != null)
                {
                    storedApplication.ApplicationStatus = applicationVM.Application.ApplicationStatus;
                    storedApplication.SubscriptionId = applicationVM.Application.SubscriptionId;
                }
            }
            catch (FailedDependencyException ex)
            {
                _log.ErrorException("Provisioning Application Exception", ex);
                errorInfo = String.Format("{0} has been requested and will be added to your apps as soon as provisioning is complete.", applicationVM.Application.Name);
                applicationVM.Application.ApplicationStatus = Status.Provisioning;
            }
            catch (ConflictException ex)
            {
                _log.ErrorException("Provisioning Application Exception", ex);
                errorInfo = "{0} has been requested previously and will be added to your applications as soon as provisioning is complete.";
                applicationVM.Application.ApplicationStatus = Status.Provisioning;
            }
            catch (Exception ex)
            {
                _log.ErrorException("Provisioning Application Exception", ex);
                errorInfo = String.Format("{0} can not be added through {1} at this time. {2}",
                                        applicationVM.Application.Name, Helper.ApplicationName, errorInfo);
            }

            Helper.PerformInUiThread(() =>
                    {
                        RemoveFromMyApps(applicationVM);
                        AddToSuggestedApps(applicationVM);
                        Message = errorInfo;
                    });

            SyncMyApplications();
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
                if (!string.IsNullOrEmpty(applicationViewModel.Application.SubscriptionId))
                {
                    applicationViewModel.InProgressVisibility = Visibility.Visible;

                    BackgroundWorker backgroundWorker = new BackgroundWorker();
                    backgroundWorker.DoWork += UnsubscribeAsynchronously;
                    backgroundWorker.RunWorkerAsync(applicationViewModel);
                }
                else
                {
                    MessageBox.Show(applicationViewModel.Application.Name + " cannot be removed.");
                }
            }
        }

        private void UnsubscribeAsynchronously(object sender, DoWorkEventArgs e)
        {
            var applicationViewModel = (ApplicationViewModel)e.Argument;
            ServiceLocator.BrowserWindowsCommunicator.CloseApplication(applicationViewModel.Application.Id);

            try
            {
                var success = Helper.RemoveApplication(applicationViewModel.Application);

                if (success)
                {
                    RemoveFromMyApps(applicationViewModel);
                    GetSuggestedApplicationsWithApiCall();
                }
                else
                {
                    applicationViewModel.InProgressVisibility = Visibility.Collapsed;
                    Message = applicationViewModel.Application.Name + " cannot be removed.";
                }
            }
            catch (Exception ex)
            {
                _log.ErrorException("Deprovisioning Application Exception", ex);

                Helper.PerformInUiThread(() =>
                    {
                        Message = "Error Removing " + applicationViewModel.Application.Name;
                    });
            }

            SyncMyApplications();
        }

        public void InitializeAppsLists()
        {
            lock (ServiceLocator.LocalStorage.Locker)
            {
                if (ServiceLocator.LocalStorage.InstalledLocalApps.Any())
                {
                    ServiceLocator.LocalStorage.InstalledLocalApps.Remove(LocalApplications.AppStoreApp);
                    ServiceLocator.LocalStorage.InstalledLocalApps.Insert(0, LocalApplications.AppStoreApp);
                }
                else
                {
                    ServiceLocator.LocalStorage.InstalledLocalApps.Add(LocalApplications.AppStoreApp);
                }

                ServiceLocator.LocalStorage.LastSuggestedApps.RemoveAll(
                    a => ServiceLocator.LocalStorage.AllInstalledApplications.Contains(a));

                MyApplications =
                    new ObservableCollection<ApplicationViewModel>();
                SuggestedApplications =
                    new ObservableCollection<ApplicationViewModel>();

                foreach (var installedApps in ServiceLocator.LocalStorage.AllInstalledApplications)
                {
                    MyApplications.Add(new ApplicationViewModel(installedApps));
                }

                foreach (var lastSuggestedApp in ServiceLocator.LocalStorage.LastSuggestedApps)
                {
                    if (lastSuggestedApp.ApplicationStatus != Status.Provisioning)
                    {
                        SuggestedApplications.Add(new ApplicationViewModel(lastSuggestedApp));
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to sync MyApplications with API. Default behavior ignores exceptions.  Will fail if user is logged in and the API can not be reached.
        /// </summary>
        public void SyncMyApplications(bool throwExceptions = false, bool forceAuthentication = false)
        {
            try
            {
                var apiApps = new List<Application>();

                if (forceAuthentication || !ServiceLocator.CachedAppDirectApi.IsAuthenticated)
                {
                    bool loginSuccessful = false;
                    try
                    {
                        loginSuccessful = Helper.Authenticate();
                    }
                    catch (Exception e)
                    {
                        Logout();
                        _log.ErrorException("Exception thrown by authentication", e);
                    }

                    if (IsLoggedIn && !loginSuccessful)
                    {
                        Logout();
                    }
                }

                if (ServiceLocator.CachedAppDirectApi.IsAuthenticated)
                {
                    apiApps = ServiceLocator.CachedAppDirectApi.MyApps.ToList();
                    ServiceLocator.BrowserWindowsCommunicator.UpdateApplications(apiApps.Cast<IApplication>().ToList());
                }

                var displayedApps = MyApplications.Select(a => a.Application).ToList();

                var newApps = new List<Application>();
                var expiredApps = displayedApps.Where(a => !a.IsLocalApp && a.ApplicationStatus != Status.AttempingProvisioning).Except(apiApps).ToList();

                foreach (var application in apiApps)
                {
                    var matchedApp = displayedApps.FirstOrDefault(a => a.Equals(application));

                    if (matchedApp != null)
                    {
                        matchedApp.UrlString = application.UrlString;
                        matchedApp.ApplicationStatus = Status.Active;
                    }
                    else
                    {
                        newApps.Add(application);
                    }
                }

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
                if (throwExceptions)
                {
                    throw;
                }
                else
                {
                    _log.ErrorException("Sync error", e);
                }
            }
        }

        /// <summary>
        /// Attempts to sync SuggestedApplications with API. Default behavior ignores exceptions.  Will fail if the API can not be reached.
        /// </summary>
        public void GetSuggestedApplicationsWithApiCall(bool throwExceptions = false)
        {
            try
            {
                lock (ServiceLocator.LocalStorage.Locker)
                {
                    var apiSuggestedApps =
                        ServiceLocator.CachedAppDirectApi.SuggestedApps.Except(
                            ServiceLocator.LocalStorage.AllInstalledApplications).ToList();

                    apiSuggestedApps.RemoveAll(a => a.Price != null && !a.Price.Contains("Free"));

                    var displayedApps = ServiceLocator.LocalStorage.LastSuggestedApps;

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
                if (throwExceptions)
                {
                    throw;
                }
                else
                {
                    _log.ErrorException("Get suggested apps error", e);
                }
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

            if (applicationViewModel.Application.ApplicationStatus != Status.Provisioning)
            {
                Helper.PerformInUiThread(() => SuggestedApplications.Add(applicationViewModel));
            }
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
                string message = String.Format(Resources.CloseForUpdatesString, Resources.ApplicationName);

                string caption = Resources.ApplicationName + " Updater";

                var messageBoxResult = MessageBox.Show(message, caption, MessageBoxButton.YesNo);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    ServiceLocator.Updater.InstallUpdates();
                }
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
            Helper.PerformForMinimumTime(() => { updateAvailable = ServiceLocator.Updater.GetUpdates(Helper.ApplicationVersion, 1, 0); }, false, millisecondsToDisplayCheckingString);

            Helper.PerformInUiThread(() =>
                {
                    UpdateString = updateAvailable ? Resources.InstallUpdateString : Resources.NoUpdateFoundString;
                    UpdateSpinnerVisibility = Visibility.Hidden;
                    UpdateAvailable = updateAvailable;
                });
        }

        public void ResetUpdateText()
        {
            UpdateString = ServiceLocator.LocalStorage.UpdateDownloaded
                                         ? Properties.Resources.InstallUpdateString
                                         : Properties.Resources.GetUpdateString;
        }

        public void GetAboutDialog()
        {
            Message = VersionString;
        }

        public void LogInLogOutClicked()
        {
            if (IsLoggedIn)
            {
                Logout();
            }
            else
            {
                LoginViewModel.IsVisible = Visibility.Visible;
            }
        }

        public void CollapseLogin()
        {
            LoginViewModel.IsVisible = Visibility.Collapsed;
        }

        public void LoginSuccessful(object sender, EventArgs e)
        {
            try
            {
                Helper.RetryAction(() => SyncMyApplications(true), 3, TimeSpan.FromMilliseconds(200));
            }
            catch (Exception ex)
            {
                _log.ErrorException(ex.Message, ex);
                Message = Properties.Resources.ErrorGettingMyApps;
            }

            try
            {
                Helper.GetUserInfo();
            }
            catch (Exception ex)
            {
                _log.ErrorException(ex.Message, ex);
            }

            GetSuggestedApplicationsWithApiCall();

            IsLoggedIn = true;
        }
    }
}