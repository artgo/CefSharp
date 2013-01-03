using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AppDirect.WindowsClient.Models;
using AppDirect.WindowsClient.Storage;
using Application = AppDirect.WindowsClient.Models.Application;

namespace AppDirect.WindowsClient.UI
{
    ///<summary>
    /// Represents the data and methods that are used to create the View
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private LoginObject _loginInfo;
        private const int MyAppDisplayLimit = 10;

        public bool IsLoggedIn
        {
            get { return LoginInfo.AuthToken != null; }
        }

        public string LoginButtonDisplayText
        {
            get { return String.IsNullOrEmpty(LoginInfo.UserName) ? "Log In" : "Log Out"; }
            set { NotifyPropertyChanged("LoginInfoButtonDisplayText"); }
        }

        public LoginObject LoginInfo
        {
            get
            {
                _loginInfo = LocalStorage.Instance.LoginInfo ?? new LoginObject();

                return _loginInfo;
            }
            set
            {
                _loginInfo = value;
                NotifyPropertyChanged("LoginInfoButtonDisplayText");
                NotifyPropertyChanged("LoginInfo");
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

        private void GetMyApplications()
        {
            if (MyApplications == null)
            {
                MyApplications = new ObservableCollection<Application>();
            }

            var myAppsList = new List<Application>();

            myAppsList.AddRange(LocalStorage.Instance.InstalledLocalApps);
            
            if (!String.IsNullOrEmpty(LoginInfo.AuthToken))
            {
                myAppsList.AddRange(CachedAppDirectApi.Instance.MyApps.ToList());
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

            suggestedAppsList.AddRange(LocalStorage.Instance.SuggestedLocalApps);

            var myAppIds = MyApplications.Select(a => a.Id).ToList();

            suggestedAppsList.AddRange(CachedAppDirectApi.Instance.SuggestedApps.Where(application => !myAppIds.Contains(application.Id)));

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

        public void Login(LoginObject loginObject)
        {
            try
            {
                CachedAppDirectApi.Instance.Login(loginObject);
                try
                {
                    LocalStorage.Instance.LoginInfo = loginObject;
                    LocalStorage.Instance.SaveAppSettings();
                    LoginInfo = LocalStorage.Instance.LoginInfo;
                    
                    GetMyApplications();
                    GetSuggestedApplications();
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to save login info to local storage");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Login Failed: " + e.Message);
            }
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
                //Start asynchronous call to Api to remove application

                CachedAppDirectApi.Instance.SuggestedApps.Add(application);
                CachedAppDirectApi.Instance.MyApps.RemoveAll(a => a.Id == application.Id);
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
                if (String.IsNullOrEmpty(LoginInfo.AuthToken))
                {
                    return;
                }

                string localFilename = Environment.SpecialFolder.ApplicationData + @"\AppDirect\" + application.Id + ".ico";

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(application.ImagePath, localFilename);
                }

                application.ImagePath = Path.GetFullPath(localFilename);

                //Start asynchronous call to Api to add application

                CachedAppDirectApi.Instance.MyApps.Add(application);
                CachedAppDirectApi.Instance.SuggestedApps.Remove(application);
            }

            GetMyApplications();
            GetSuggestedApplications();
        }

        public void Logout()
        {
            LocalStorage.Instance.LoginInfo = new LoginObject();
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
