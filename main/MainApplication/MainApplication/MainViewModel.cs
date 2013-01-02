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

namespace MainApplication
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ICommand _uninstallApp;
        private ICommand _installApp;
        private LoginObject _loginInfo;

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

        public ICommand UninstallAppCommand
        {
            get
            {
                if (_uninstallApp == null)
                {
                    _uninstallApp = new RelayCommand<Application>(Uninstall,
                        null);
                }
                return _uninstallApp;
            }
        }
       
        public ICommand InstallAppCommand
        {
            get
            {
                if (_installApp == null)
                {
                    return _installApp = new RelayCommand<Application>(Install,    
                        null);
                }
                return _installApp;
            }
        }

        public MainViewModel()
        {
            try
            {
                LocalStorage.Instance.LoadLocalStorage();
            }
            catch (Exception)
            {
                //MessageBox.Show("Unable to load applications from Storage or Local Applications list");
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

                if (myAppCount == 12)
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

                    GetSuggestedApplications();
                    GetMyApplications();
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
                    ClickLoginButton();
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

            GetSuggestedApplications();
            GetMyApplications();
        }

        private void ClickLoginButton()
        {
            if (IsLoggedIn)
            {
                Logout();
            }
            else
            {
                var loginWindow = new LoginWindow();
                loginWindow.ShowDialog();
            }
        }

        public void Logout()
        {
            LocalStorage.Instance.LoginInfo = new LoginObject();
            GetMyApplications();
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
