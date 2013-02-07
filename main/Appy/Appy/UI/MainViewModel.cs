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
using Application = AppDirect.WindowsClient.Models.Application;

namespace AppDirect.WindowsClient.UI
{
    ///<summary>
    /// Represents the data and methods that are used to create the View
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private const int MyAppDisplayLimit = 10;
        private const int SuggestedAppsDisplayLimit = 5;
        private string _myAppsLoadError = String.Empty;
        private string _suggestedAppsLoadError = String.Empty;
        private string _loginFailedMessage = Properties.Resources.CredentialsProblemError;
        private string _loginHeaderText = Properties.Resources.LoginHeaderDefault;

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

        public Visibility LogOutVisibility
        {
            get
            {
                if (!ServiceLocator.LocalStorage.HasCredentials)
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
            var setupBackgroundWorker = new BackgroundWorker();
            setupBackgroundWorker.DoWork += setup_Worker_DoWork;
            InitializeAppsLists();

            setupBackgroundWorker.RunWorkerAsync();
        }

        private void setup_Worker_DoWork(object sender, DoWorkEventArgs ea)
        {
            while (true)
            {
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
                                                                                            LoginFailedMessage = Resources.NetworkProblemError));
                        }
                    }
                }

                GetMyApplications();
                GetSuggestedApplicationsWithApiCall();
                ServiceLocator.LocalStorage.SaveAppSettings();

                Thread.Sleep(TimeSpan.FromMinutes(Helper.RefreshAppsIntervalMins));
            }
        }

        private void worker_RefreshAppLists(object sender, DoWorkEventArgs ea)
        {
            RefreshAppsLists();
        }

        private void InitializeAppsLists()
        {
            if (ServiceLocator.LocalStorage.InstalledLocalApps == null)
            {
                ServiceLocator.LocalStorage.InstalledLocalApps = new List<Application>();
            }

            if (ServiceLocator.LocalStorage.InstalledAppDirectApps == null)
            {
                ServiceLocator.LocalStorage.InstalledAppDirectApps = new List<Application>();
            }

            var installedAppIds = ServiceLocator.LocalStorage.AllInstalledApplications.Select(a => a.Id).ToList();

            if (ServiceLocator.LocalStorage.LastSuggestedApps == null)
            {
                ServiceLocator.LocalStorage.LastSuggestedApps = LocalApplications.GetLocalApplications().Where(a => !installedAppIds.Contains(a.Id)).ToList();
            }

            MyApplications = new ObservableCollection<Application>(ServiceLocator.LocalStorage.AllInstalledApplications.Take(MyAppDisplayLimit));

            SuggestedApplications = new ObservableCollection<Application>(ServiceLocator.LocalStorage.LastSuggestedApps.Take(SuggestedAppsDisplayLimit));
        }
        
        private void GetMyApplications()
        {
            SyncStorageWithApi();

            SyncDisplayWithStoredList(MyAppDisplayLimit, MyApplications, ServiceLocator.LocalStorage.AllInstalledApplications);
        }

        private void SyncDisplayWithStoredList(int displayLimit, ObservableCollection<Application> displayedList, List<Application> storedList  )
        {
            List<Application> storedApps = storedList.Take(displayLimit).ToList();
            
            for (int index   = 0; index < storedApps.Count; index++)
            {
                if (displayedList.Count <= index)
                {
                    if (System.Windows.Application.Current != null)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(
                            new Action(() => displayedList.Add(storedApps[index])));
                    }
                }

                if (!displayedList[index].Equals(storedApps[index]))
                {
                    if (System.Windows.Application.Current != null)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(
                            new Action(() => displayedList[index] = storedApps[index]));
                    }
                }
            }

            for (int index = storedApps.Count; index < displayedList.Count; index++)
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

            ServiceLocator.LocalStorage.InstalledAppDirectApps = apiApps;
        }

        private void GetSuggestedApplications()
        {
            var suggestedApps = LocalApplications.GetLocalApplications();
            var apiSuggestedApps = ServiceLocator.LocalStorage.LastSuggestedApps.Where(a => !a.IsLocalApp);

            suggestedApps.AddRange(apiSuggestedApps);

            ServiceLocator.LocalStorage.LastSuggestedApps =
                suggestedApps.Except(ServiceLocator.LocalStorage.AllInstalledApplications).ToList();

            SyncDisplayWithStoredList(SuggestedAppsDisplayLimit, SuggestedApplications, ServiceLocator.LocalStorage.LastSuggestedApps);
        }

        private void GetSuggestedApplicationsWithApiCall()
        {
            var suggestedApps = LocalApplications.GetLocalApplications();
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

            SyncDisplayWithStoredList(SuggestedAppsDisplayLimit, SuggestedApplications, ServiceLocator.LocalStorage.LastSuggestedApps);
        }

        public bool Login(string username, string password)
        {
            if (ServiceLocator.CachedAppDirectApi.Authenticate(username, password))
            {
                ServiceLocator.LocalStorage.SetCredentials(username, password); 
                
                RefreshAppsLists();
                return true;
            }

            return false;
        }

        public void Uninstall(Application application)
        {
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

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += worker_RefreshAppLists;
            backgroundWorker.RunWorkerAsync();
        }

        public void Install(Application application)
        {
            if (application.IsLocalApp)
            {
                ServiceLocator.LocalStorage.InstalledLocalApps.Add(application);
                ServiceLocator.LocalStorage.LastSuggestedApps.Remove(application);
            }
            else
            {
                System.Diagnostics.Process.Start(Properties.Resources.InstallAppTarget + application.Id);
            }

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += worker_RefreshAppLists;
            backgroundWorker.RunWorkerAsync();
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
