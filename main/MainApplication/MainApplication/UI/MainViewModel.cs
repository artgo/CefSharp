﻿using System;
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
        private string _myAppsLoadError = String.Empty;
        private string _suggestedAppsLoadError = String.Empty;
        private string _loginFailedMessage = String.Empty;
        private string _loginHeaderText = Properties.Resources.LoginHeaderDefault;

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
            if (ServiceLocator.LocalStorage.InstalledApps == null)
            {
                ServiceLocator.LocalStorage.InstalledApps = new List<Application>();
            }

            InitializeAppsLists();
            
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
                    LoginFailedMessage = AppDirect.WindowsClient.Properties.Resources.NetworkProblemError;
                }
            }

            RefreshAppsLists();
        }

        private void InitializeAppsLists()
        {
            MyApplications = new ObservableCollection<Application>(ServiceLocator.LocalStorage.InstalledApps);

            var suggestedApps = LocalApplications.GetLocalApplications().Where(a => !ServiceLocator.LocalStorage.InstalledApps.Contains(a));
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
                MyApplications.Remove(application);
                ServiceLocator.LocalStorage.InstalledApps.Remove(application);
            }

            int myAppCount = MyApplications.Count;
            foreach (Application application in appsToAdd)
            {
                if (myAppCount == MyAppDisplayLimit)
                {
                    break;
                }

                MyApplications.Add(application);
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
            if (SuggestedApplications == null)
            {
                SuggestedApplications = new ObservableCollection<Application>();
            }

            var suggestedAppsList = new List<Application>();

            suggestedAppsList.AddRange(LocalApplications.GetLocalApplications());
            
            var myAppIds = MyApplications.Select(a => a.Id).ToList();

            try
            {
                suggestedAppsList.AddRange(ServiceLocator.CachedAppDirectApi.SuggestedApps);
                SuggestedAppsLoadError = String.Empty;
            }
            catch (Exception e)
            {
                SuggestedAppsLoadError = e.Message;
            }

            suggestedAppsList.RemoveAll(application => myAppIds.Contains(application.Id));


            SuggestedApplications.Clear();

            int suggestedAppCount = 0;
            
            foreach (Application application in suggestedAppsList)
            {
                SuggestedApplications.Add(application);
                suggestedAppCount++;

                if (suggestedAppCount == 7)
                {
                    break;
                }
            }
        }

        public bool Login(string username, string password)
        {
            if (ServiceLocator.CachedAppDirectApi.Authenticate(username, password))
            {
                ServiceLocator.LocalStorage.SetCredentials(username, password);
                ServiceLocator.LocalStorage.SaveAppSettings();

                RefreshAppsLists();
                return true;
            }

            return false;
        }

        public void Uninstall(Application application)
        {
            if (application.IsLocalApp)
            {
                ServiceLocator.LocalStorage.InstalledApps.Remove(application);

                try
                {
                    ServiceLocator.LocalStorage.SaveAppSettings();
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to save uninstall info to local storage");
                }
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
                ServiceLocator.LocalStorage.InstalledApps.Add(application);
                ServiceLocator.LocalStorage.SaveAppSettings();
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
