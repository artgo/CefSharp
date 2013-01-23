using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AppDirect.WindowsClient.Models;
using AppDirect.WindowsClient.Storage;
using CefSharp;
using Ninject;
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
                _myAppsLoadError = value;
                NotifyPropertyChanged("MyAppsLoadError");
            }
        }

        public string SuggestedAppsLoadError
        {
            get { return _suggestedAppsLoadError; }
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
                catch (System.Security.Cryptography.CryptographicException e)
                {
                    ServiceLocator.LocalStorage.ClearLoginCredentials();
                    MessageBox.Show("Credentials were present, but there was an error decrypting: " + e.Message);
                }
                catch (Exception)
                {
                    if (System.Windows.Application.Current != null)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => 
                    LoginFailedMessage = Properties.Resources.NetworkProblemError));
                    }
                }
            }

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

            var allApps =
                ServiceLocator.LocalStorage.InstalledLocalApps.Concat(ServiceLocator.LocalStorage.InstalledAppDirectApps);
            MyApplications = new ObservableCollection<Application>(allApps);

            SuggestedApplications = new ObservableCollection<Application>(ServiceLocator.LocalStorage.LastSuggestedApps.Take(SuggestedAppsDisplayLimit));
        }
        
        private void GetMyApplications()
        {
            SyncStorageWithApi();

            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => MyApplications.Clear()));
            }


            var allApps =
                ServiceLocator.LocalStorage.InstalledLocalApps.Concat(ServiceLocator.LocalStorage.InstalledAppDirectApps);

            foreach (Application application in allApps)
            {
                if (MyApplications.Count == MyAppDisplayLimit)
                {
                    break;
                }

                if (System.Windows.Application.Current != null)
                {
                    Application application1 = application;
                    System.Windows.Application.Current.Dispatcher.Invoke(
                        new Action(() => MyApplications.Add(application1)));
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
            var installedAppIds = ServiceLocator.LocalStorage.AllInstalledApplications.Select(a => a.Id).ToList();

            var suggestedAppsList = new List<Application>();
            suggestedAppsList.AddRange(LocalApplications.GetLocalApplications().Where(a => !installedAppIds.Contains(a.Id)).ToList());

            try
            {
                suggestedAppsList.AddRange(ServiceLocator.CachedAppDirectApi.SuggestedApps);
                SuggestedAppsLoadError = String.Empty;
            }
            catch (Exception e)
            {
                SuggestedAppsLoadError = e.Message;
            }

            suggestedAppsList.RemoveAll(a => installedAppIds.Contains(a.Id));

            foreach (var application in suggestedAppsList)
            {
                if (!application.IsLocalApp)
                {

                    application.LocalImagePath = ServiceLocator.LocalStorage.SaveAppIcon(application.ImagePath ,
                                                                                         application.Id);
                }
            }

            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => SuggestedApplications.Clear()));
            }
           
            foreach (Application application in suggestedAppsList)
            {
                if (SuggestedApplications.Count == SuggestedAppsDisplayLimit)
                {
                    break;
                }

                if (System.Windows.Application.Current != null)
                {
                    Application application1 = application;
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => SuggestedApplications.Add(application1)));
                }
            }

            ServiceLocator.LocalStorage.LastSuggestedApps = suggestedAppsList;
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
            }
            else
            {
                ServiceLocator.LocalStorage.HiddenApps.Add(application.Id);
            }
            RefreshAppsLists();
        }

        public void Install(Application application)
        {
            if (application.IsLocalApp)
            {
                ServiceLocator.LocalStorage.InstalledLocalApps.Add(application);
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
