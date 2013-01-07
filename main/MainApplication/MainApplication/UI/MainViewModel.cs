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

        public string MyAppsLoadError
        {
            get { return _myAppsLoadError; }
            set
            {
                _myAppsLoadError = value;
                NotifyPropertyChanged("MyAppsLoadError");
            }
        }

        public ObservableCollection<Application> MyApplications { get; set; }
        public ObservableCollection<Application> SuggestedApplications { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            if (LocalStorage.Instance.SuggestedLocalApps == null)
            {
                LocalStorage.Instance.SuggestedLocalApps = LocalApplications.GetBackUpLocalAppsList();
                LocalStorage.Instance.SaveAppSettings();
            }

            if (LocalStorage.Instance.InstalledLocalApps == null)
            {
                LocalStorage.Instance.InstalledLocalApps = new List<Application>();
            }

            GetMyApplications();
            GetSuggestedApplications();
        }

        public void GetMyApplications()
        {
            if (MyApplications == null)
            {
                MyApplications = new ObservableCollection<Application>();
            }

            var myAppsList = new List<Application>();

            myAppsList.AddRange(LocalStorage.Instance.InstalledLocalApps);

            if (LocalStorage.Instance.HasCredentials)
            {
                try
                {
                    myAppsList.AddRange(ServiceLocator.CachedAppDirectApi.MyApps.ToList()); 
                    MyAppsLoadError = String.Empty;
                }
                catch (Exception e)
                {
                    MyAppsLoadError = e.Message;
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

        public void GetSuggestedApplications()
        {
            if (SuggestedApplications == null)
            {
                SuggestedApplications = new ObservableCollection<Application>();
            }

            var suggestedAppsList = new List<Application>();

            suggestedAppsList.AddRange(LocalStorage.Instance.SuggestedLocalApps);

            var myAppIds = MyApplications.Select(a => a.Id).ToList();

            try
            {
                suggestedAppsList.AddRange(ServiceLocator.CachedAppDirectApi.SuggestedApps.Where(application => !myAppIds.Contains(application.Id)));
            }
            catch (Exception e)
            {
                MyAppsLoadError = e.Message;
            }

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
                LocalStorage.Instance.LoginInfo = new LoginObject{UserName = username, Password = password};
                LocalStorage.Instance.SaveAppSettings();

                GetMyApplications();
                GetSuggestedApplications();
                return true;
            }

            return false;
        }

        public void Uninstall(Application application)
        {
            if (application.IsLocalApp)
            {
                LocalStorage.Instance.InstalledLocalApps.Remove(application);
                LocalStorage.Instance.SuggestedLocalApps.Add(application);

                try
                {
                    LocalStorage.Instance.SaveAppSettings();
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
            
            GetSuggestedApplications();
            GetMyApplications();
        }

        public void Install(Application application)
        {
            if (application.IsLocalApp)
            {
                LocalStorage.Instance.InstalledLocalApps.Add(application);
                LocalStorage.Instance.SuggestedLocalApps.Remove(application);

                LocalStorage.Instance.SaveAppSettings();
            }
            else
            {
                MessageBox.Show("Contact your administrator to install this application");
                //TODO: Determine if some AppDirect Apps will be installable by a user
            }

            GetMyApplications();
            GetSuggestedApplications();
        }

        public void Logout()
        {
            ServiceLocator.CachedAppDirectApi.UnAuthenticate();
            LocalStorage.Instance.LoginInfo = null;
            
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
