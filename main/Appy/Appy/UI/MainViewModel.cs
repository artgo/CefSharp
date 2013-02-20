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
            
            var installedAppIds = ServiceLocator.LocalStorage.AllInstalledApplications.Select(a => a.Id).ToList();

            if (ServiceLocator.LocalStorage.LastSuggestedApps.Count == 0)
            {
                ServiceLocator.LocalStorage.LastSuggestedApps = LocalApplications.LocalApplicationsList.Where(a => !installedAppIds.Contains(a.Id)).ToList();
            }

            MyApplications = new ObservableCollection<Application>(ServiceLocator.LocalStorage.AllInstalledApplications.Take(MyAppDisplayLimit));
            SuggestedApplications = new ObservableCollection<Application>(ServiceLocator.LocalStorage.LastSuggestedApps);
        }

        public void GetMyApplications()
        {
            SyncStorageWithApi();
            SyncDisplayWithStoredList(MyAppDisplayLimit, MyApplications, ServiceLocator.LocalStorage.AllInstalledApplications);
        }

        private void SyncDisplayWithStoredList(int displayLimit, ObservableCollection<Application> displayedList,
                                               List<Application> storedList)
        {
            List<Application> storedApps;

            if (displayLimit == 0)
            {
                storedApps = storedList;
            }
            else
            {
                storedApps = storedList.Take(displayLimit).ToList();
            }

            for (int index = 0; index < storedApps.Count; index++)
            {
                if (displayedList.Count <= index)
                {
                    if (System.Windows.Application.Current != null)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(
                            new Action(() => displayedList.Add(storedApps[index])));
                    }
                }
                else if (!displayedList[index].Equals(storedApps[index]))
                {
                    if (System.Windows.Application.Current != null)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(
                            new Action(() => displayedList[index] = storedApps[index]));
                    }
                }
            }

            for (int index = displayedList.Count - 1; index >= storedList.Count; index--)
            {
                if (System.Windows.Application.Current != null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(
                        new Action(() => displayedList.RemoveAt(index)));
                }
            }

        }

        private void SyncStorageWithApi()
        {
            List<Application> apiApps = new List<Application>();

            if (ServiceLocator.LocalStorage.HasCredentials)
            {
                try
                {
                    apiApps = ServiceLocator.CachedAppDirectApi.MyApps.Where(a => !ServiceLocator.LocalStorage.HiddenApps.Contains(a.Id)).ToList();
                    MyAppsLoadError = String.Empty;
                }
                catch (Exception)
                {
                    LoginFailedMessage = AppDirect.WindowsClient.Properties.Resources.NetworkProblemError;
                }
            }

            foreach (var application in apiApps)
            {
                application.LocalImagePath = ServiceLocator.LocalStorage.SaveAppIcon(application.ImagePath,
                                                                                    application.Id);
            }

            SyncTaskbarPanelAndStorageWithApi(apiApps);
        }

        private void SyncTaskbarPanelAndStorageWithApi(List<Application> apiApps)
        {
            var appsToAdd = apiApps.Where(a => !ServiceLocator.LocalStorage.InstalledAppDirectApps.Contains(a)).ToList();
            var appsToRemove =
                ServiceLocator.LocalStorage.InstalledAppDirectApps.Where(a => !apiApps.Contains(a)).ToList();

            if (ApplicationRemovedNotifier != null && System.Windows.Application.Current != null)
            {
                foreach (var application in appsToRemove)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(
                        new Action(() => ApplicationRemovedNotifier(application, null)));
                }
            }

            ServiceLocator.LocalStorage.InstalledAppDirectApps.RemoveAll(appsToRemove.Contains);

            appsToAdd.ForEach(a => a.PinnedToTaskbar = true);

            if (ApplicationRemovedNotifier != null && System.Windows.Application.Current != null)
            {
                foreach (var application in appsToAdd)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(
                        new Action(() => ApplicationAddedNotifier(application, null)));
                }
            }

            ServiceLocator.LocalStorage.InstalledAppDirectApps.RemoveAll(a => !apiApps.Contains(a));
            ServiceLocator.LocalStorage.InstalledAppDirectApps.AddRange(appsToAdd);
        }

        private void GetSuggestedApplications()
        {
            var suggestedApps = LocalApplications.LocalApplicationsList;
            var apiSuggestedApps = ServiceLocator.LocalStorage.LastSuggestedApps.Where(a => !a.IsLocalApp);

            suggestedApps.AddRange(apiSuggestedApps);

            ServiceLocator.LocalStorage.LastSuggestedApps =
                suggestedApps.Except(ServiceLocator.LocalStorage.AllInstalledApplications).ToList();

            SyncDisplayWithStoredList(0, SuggestedApplications, ServiceLocator.LocalStorage.LastSuggestedApps);
        }

        public void SyncAppsWithApi()
        {
            if (ServiceLocator.LocalStorage.HasCredentials)
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
                        System.Windows.Application.Current.Dispatcher.Invoke(
                            new Action(() => LoginFailedMessage = Resources.NetworkProblemError));
                    }
                }
                finally
                {
                    GetMyApplications();
                }
            }

            GetSuggestedApplicationsWithApiCall();
            ServiceLocator.LocalStorage.SaveAppSettings();
        }

        public void GetSuggestedApplicationsWithApiCall()
        {
            var suggestedApps = LocalApplications.LocalApplicationsList;
            var apiSuggestedApps = new List<Application>();

            try
            {
                apiSuggestedApps = ServiceLocator.CachedAppDirectApi.SuggestedApps.ToList();

                SuggestedAppsLoadError = String.Empty;

            }
            catch (Exception e)
            {
                SuggestedAppsLoadError = e.Message;
            }

            foreach (var application in apiSuggestedApps)
            {
                application.LocalImagePath = ServiceLocator.LocalStorage.SaveAppIcon(application.ImagePath, application.Id);
            }

            suggestedApps.AddRange(apiSuggestedApps);

            ServiceLocator.LocalStorage.LastSuggestedApps =
                suggestedApps.Except(ServiceLocator.LocalStorage.AllInstalledApplications).ToList();

            SyncDisplayWithStoredList(0, SuggestedApplications, ServiceLocator.LocalStorage.LastSuggestedApps);
        }

        public bool Login(string username, string password)
        {
            if (ServiceLocator.CachedAppDirectApi.Authenticate(username, password))
            {
                ServiceLocator.LocalStorage.SetCredentials(username, password); 
                
                RefreshAppsLists();

                CloudSyncVisibility = Visibility.Hidden;
                LogOutVisibility = Visibility.Visible;
                return true;
            }

            return false;
        }

        public void Uninstall(Application application)
        {
            application.PinnedToTaskbar = false;
            if (application.IsLocalApp)
            {
                ServiceLocator.LocalStorage.InstalledLocalApps.Remove(application);
                ServiceLocator.LocalStorage.LastSuggestedApps.Add(application);
            }
            else
            {
                ServiceLocator.LocalStorage.HiddenApps.Add(application.Id);
            }
            
            MyApplications.Remove(application);
            if (ApplicationRemovedNotifier != null)
            {
                ApplicationRemovedNotifier(application, null);
            }

            RefreshAppsLists();
        }

        public void Install(Application application)
        {
            if (application.IsLocalApp)
            {
                application.PinnedToTaskbar = true;
                ServiceLocator.LocalStorage.InstalledLocalApps.Add(application);
                ServiceLocator.LocalStorage.LastSuggestedApps.Remove(application);

                if (ApplicationAddedNotifier != null)
                {
                    ApplicationAddedNotifier(application, null); 
                }
            }
            else
            {
                System.Diagnostics.Process.Start(Properties.Resources.InstallAppTarget + application.Id);
            }

            RefreshAppsLists();
        }

        public void Logout()
        {
            ServiceLocator.CachedAppDirectApi.UnAuthenticate();
            ServiceLocator.LocalStorage.ClearLoginCredentials();

            RefreshAppsLists();
        }

        public void RefreshAppsLists()
        {
            GetMyApplications();
            GetSuggestedApplications();
            ServiceLocator.LocalStorage.SaveAppSettings();
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
