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
        private const int SuggestedAppsDisplayLimit = 6;
        private string _myAppsLoadError = String.Empty;
        private string _suggestedAppsLoadError = String.Empty;
        private string _loginFailedMessage = String.Empty;
        private string _loginHeaderText = Properties.Resources.LoginHeaderDefault;

        private readonly BackgroundWorker appListBackgroundWorker = new BackgroundWorker();
        private readonly BackgroundWorker setupBackgroundWorker = new BackgroundWorker();

        public string VersionString
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
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
            appListBackgroundWorker.DoWork += appList_Worker_DoWork;
            setupBackgroundWorker.DoWork += setup_Worker_DoWork;

            InitializeAppsLists();

            setupBackgroundWorker.RunWorkerAsync();
        }

        private void appList_Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            RefreshAppsLists();
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

            appListBackgroundWorker.RunWorkerAsync();
        }

        private void InitializeAppsLists()
        {
            if (ServiceLocator.LocalStorage.InstalledApps == null)
            {
                ServiceLocator.LocalStorage.InstalledApps = new List<Application>();
            }

            MyApplications = new ObservableCollection<Application>(ServiceLocator.LocalStorage.InstalledApps);
            var installedAppIds = ServiceLocator.LocalStorage.InstalledApps.Select(a => a.Id).ToList();

            var suggestedApps = LocalApplications.GetLocalApplications().Where(a => !installedAppIds.Contains(a.Id));
            SuggestedApplications = new ObservableCollection<Application>(suggestedApps);
        }
        
        private void GetMyApplications()
        {
            SyncStorageWithApi();

            var displayedAppIds = MyApplications.Select(a => a.Id).ToList();
            var installedAppIds = ServiceLocator.LocalStorage.InstalledApps.Select(a => a.Id).ToList();

            var appsToAdd = ServiceLocator.LocalStorage.InstalledApps.Where(a => !displayedAppIds.Contains(a.Id)).ToList();
            var appsToRemove = MyApplications.Where(a => !installedAppIds.Contains(a.Id)).ToList();


            foreach (Application application in appsToRemove)
            {
                if (System.Windows.Application.Current != null)
                {
                    Application application1 = application;
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => MyApplications.Remove(application1)));
                }

                ServiceLocator.LocalStorage.InstalledApps.Remove(application);
            }

            int myAppCount = MyApplications.Count;
            foreach (Application application in appsToAdd)
            {
                if (myAppCount == MyAppDisplayLimit)
                {
                    break;
                }

                if (System.Windows.Application.Current != null)
                {
                    Application application1 = application;
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => MyApplications.Add(application1)));
                }
                
                myAppCount++;
            }
        }

        private void SyncStorageWithApi()
        {
            var myAppsList = new List<Application>();

            if (ServiceLocator.LocalStorage.HasCredentials)
            {
                try
                {
                    myAppsList.AddRange(ServiceLocator.CachedAppDirectApi.MyApps.Where(a => !ServiceLocator.LocalStorage.HiddenApps.Contains(a.Id)));
                    MyAppsLoadError = String.Empty;
                }
                catch (Exception)
                {
                    LoginFailedMessage = AppDirect.WindowsClient.Properties.Resources.NetworkProblemError;
                }
            }

            var installedAppIds = ServiceLocator.LocalStorage.InstalledApps.Select(a => a.Id).ToList();
            var appsToAdd = myAppsList.Where(a => !installedAppIds.Contains(a.Id)).ToList();

            foreach (Application application in appsToAdd)
            {
                if (!application.IsLocalApp)
                {
                    application.ImagePath = ServiceLocator.LocalStorage.SaveAppIcon(application.ImagePath, application.Name);
                    ServiceLocator.LocalStorage.InstalledApps.Add(application);
                }
            }


            var apiAppIds = myAppsList.Select(a => a.Id).ToList();
            ServiceLocator.LocalStorage.InstalledApps.RemoveAll(a => !a.IsLocalApp && !apiAppIds.Contains(a.Id));
        }

        private void GetSuggestedApplications()
        {
            var suggestedAppsList = new List<Application>();

            try
            {
                suggestedAppsList.AddRange(ServiceLocator.CachedAppDirectApi.SuggestedApps);
                SuggestedAppsLoadError = String.Empty;
            }
            catch (Exception e)
            {
                SuggestedAppsLoadError = e.Message;
            }

            var displayedAppIds = SuggestedApplications.Select(a => a.Id).ToList();
            var installedAppIds = ServiceLocator.LocalStorage.InstalledApps.Select(a => a.Id).ToList();

            var appsToRemove = suggestedAppsList.Where(a => installedAppIds.Contains(a.Id)).ToList();
            appsToRemove.AddRange(SuggestedApplications.Where(a => installedAppIds.Contains(a.Id)));

            var appsToAdd = suggestedAppsList.Where(a => !displayedAppIds.Contains(a.Id)).ToList();
            appsToAdd.AddRange(LocalApplications.GetLocalApplications().Where(a => !installedAppIds.Contains(a.Id)));

            foreach (Application application in appsToRemove)
            {
                if (System.Windows.Application.Current != null)
                {
                    Application application1 = application;
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => SuggestedApplications.Remove(application1)));
                }
            }

            int myAppCount = SuggestedApplications.Count;

            foreach (Application application in appsToAdd)
            {
                if (myAppCount == SuggestedAppsDisplayLimit)
                {
                    break;
                }

                if (System.Windows.Application.Current != null)
                {
                    Application application1 = application;
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => SuggestedApplications.Add(application1)));
                }

                myAppCount++;
            }
        }

        public bool Login(string username, string password)
        {
            if (ServiceLocator.CachedAppDirectApi.Authenticate(username, password))
            {
                ServiceLocator.LocalStorage.SetCredentials(username, password);
                appListBackgroundWorker.RunWorkerAsync();
                return true;
            }

            return false;
        }

        public void Uninstall(Application application)
        {
            if (application.IsLocalApp)
            {
                ServiceLocator.LocalStorage.InstalledApps.Remove(application);
            }
            else
            {
                ServiceLocator.LocalStorage.HiddenApps.Add(application.Id);
            }

            appListBackgroundWorker.RunWorkerAsync();
        }

        public void Install(Application application)
        {
            if (application.IsLocalApp)
            {
                ServiceLocator.LocalStorage.InstalledApps.Add(application);
            }
            else
            {
                System.Diagnostics.Process.Start(Properties.Resources.InstallAppTarget + application.Id);
            }

            appListBackgroundWorker.RunWorkerAsync();
        }

        public void Logout()
        {
            ServiceLocator.CachedAppDirectApi.UnAuthenticate();
            ServiceLocator.LocalStorage.ClearLoginCredentials();

            appListBackgroundWorker.RunWorkerAsync();
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
