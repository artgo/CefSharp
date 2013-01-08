using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AppDirect.WindowsClient.Models;
using AppDirect.WindowsClient.Storage;
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
            if (ServiceLocator.LocalStorage.InstalledLocalApps == null)
            {
                ServiceLocator.LocalStorage. InstalledLocalApps = new List<Application>();
            }

            if (ServiceLocator.LocalStorage.HasCredentials && ServiceLocator.LocalStorage.LoginInfo.ExpirationDate > DateTime.Now)
            {
                try
                {
                    if (!ServiceLocator.CachedAppDirectApi.Authenticate(ServiceLocator.LocalStorage.LoginInfo.UnencryptedUsername,
                                                                        ServiceLocator.LocalStorage.LoginInfo.UnencryptedPassword))
                    {
                        ServiceLocator.LocalStorage.ClearLoginCredentials();
                    }
                }
                catch (Exception e)
                {
                    LoginFailedMessage = AppDirect.WindowsClient.Properties.Resources.NetworkProblemError;
                }
            }

            RefreshAppsLists();
        }

        private void GetMyApplications()
        {
            if (MyApplications == null)
            {
                MyApplications = new ObservableCollection<Application>();
            }

            var myAppsList = new List<Application>();

            myAppsList.AddRange(ServiceLocator.LocalStorage.InstalledLocalApps);

            if (ServiceLocator.LocalStorage.HasCredentials)
            {
                try
                {
                    myAppsList.AddRange(ServiceLocator.CachedAppDirectApi.MyApps.ToList()); 
                    MyAppsLoadError = String.Empty;
                }
                catch (Exception e)
                {
                    LoginFailedMessage = AppDirect.WindowsClient.Properties.Resources.NetworkProblemError;
                }
            }

            MyApplications.Clear();

            int myAppCount = 0;
            foreach (Application application in myAppsList)
            {
                MyApplications.Add(application);
                myAppCount++;

                if (myAppCount == MyAppDisplayLimit)
                {
                    break;
                }
            }
        }

        private void GetSuggestedApplications()
        {
            if (SuggestedApplications == null)
            {
                SuggestedApplications = new ObservableCollection<Application>();
            }

            var suggestedAppsList = new List<Application>();
                
            suggestedAppsList.AddRange(LocalApplications.Applications);
            
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
                ServiceLocator.LocalStorage.LoginInfo = new LoginObject{UnencryptedUsername = username, UnencryptedPassword = password};
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
                ServiceLocator.LocalStorage.InstalledLocalApps.Remove(application);

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
                MessageBox.Show(AppDirect.WindowsClient.Properties.Resources.UninstallAppDirectApp);
                //TODO: Determine if AppDirect apps can be uninstalled by Users
                //Start asynchronous call to Api to remove application
            }

            RefreshAppsLists();
        }

        public void Install(Application application)
        {
            if (application.IsLocalApp)
            {
                ServiceLocator.LocalStorage.InstalledLocalApps.Add(application);
                ServiceLocator.LocalStorage.SaveAppSettings();
            }
            else
            {
                MessageBox.Show("Contact your administrator to install this application");
                //TODO: Determine if some AppDirect Apps will be installable by a user
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
