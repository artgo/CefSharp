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
        private ICommand _login; 
        private ICommand _clickLogin;
        private LoginObject _loginInfo;

        public bool IsLoggedIn
        {
            get { return LoginInfo.AuthToken != null; }
        }

        public string LoginButtonDisplayText
        {
            get { return String.IsNullOrEmpty(LoginInfo.UserName) ? "Log In" : "Logout"; }
            set
            {
                NotifyPropertyChanged("LoginInfo");
            }
        }

        public LoginObject LoginInfo
        {
            get
            {
                _loginInfo = LocalApplicationData.Instance.LoginInfo ?? new LoginObject();

                return _loginInfo;
            }
            set
            {
                _loginInfo = value;
                NotifyPropertyChanged("LoginInfo");
            }
        }

        public ObservableCollection<Application> MyApplications { get; set; }
        public ObservableCollection<Application> SuggestedApplications { get; set; }
        
        public void GetMyApplications()
        {
            if (MyApplications == null)
            {
                MyApplications = new ObservableCollection<Application>();
            }


            var myAppsList = new List<Application>();

            myAppsList.AddRange(LocalApplicationData.Instance.InstalledLocalApps);

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

        public void GetSuggestedApplications()
        {
            if (SuggestedApplications == null)
            {
                SuggestedApplications = new ObservableCollection<Application>();
            }

            var suggestedAppsList = new List<Application>();

            suggestedAppsList.AddRange(LocalApplicationData.Instance.SuggestedLocalApps);
            var myAppIds = MyApplications.Select(a => a.Id).ToList();

            suggestedAppsList.AddRange(CachedAppDirectApi.Instance.SuggestedApps.Where(application => !myAppIds.Contains(application.Id)));

            SuggestedApplications.Clear();

            int suggestedAppCount = 0;
            foreach (Application application in suggestedAppsList)
            {
                SuggestedApplications.Add(application);
                suggestedAppCount ++;

                if (suggestedAppCount == 7)
                {
                    break;
                }
            }
        }

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

        public ICommand ClickLoginCommand
        {
            get
            {
                if (_clickLogin == null)
                {
                    return _clickLogin = new RelayCommand<bool>(ClickLoginButton,
                        null);
                }
                return _clickLogin;
            }
        }

        public ICommand LoginToAppStore
        {
            get
            {
                if (_login == null)
                {
                    return _login = new RelayCommand<LoginObject>(Login,
                        null);
                }
                return _login;
            }
        }

        public MainViewModel()
        {            
            GetMyApplications();
            GetSuggestedApplications();
        }

        private void Login(LoginObject loginObject)
        {
            if (CachedAppDirectApi.Instance.Login(loginObject))
            {
                LocalApplicationData.Instance.LoginInfo = loginObject;

                GetSuggestedApplications();
                GetMyApplications();

                LocalApplicationData.SaveAppSettings();
            }
        }

        private void Uninstall(Application application)
        {
            if (application.IsLocalApp)
            {
                LocalApplicationData.Instance.InstalledLocalApps.Remove(application);
                LocalApplicationData.Instance.SuggestedLocalApps.Add(application);
                
                LocalApplicationData.SaveAppSettings();
            }
            else
            {
                //Start asynchronous call to Api to remove application

                CachedAppDirectApi.Instance.SuggestedApps.Add(application);
                CachedAppDirectApi.Instance.MyApps.Remove(application);
            }
            
            GetSuggestedApplications();
            GetMyApplications();
        }

        private void Install(Application application)
        {
            if (application.IsLocalApp)
            {
                LocalApplicationData.Instance.InstalledLocalApps.Add(application);
                LocalApplicationData.Instance.SuggestedLocalApps.Remove(application);

                LocalApplicationData.SaveAppSettings();
            }
            else
            {
                if (String.IsNullOrEmpty(LoginInfo.AuthToken))
                {
                    ClickLoginButton(false);
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

        private void ClickLoginButton(bool isLoggedIn)
        {
            if (IsLoggedIn)
            {
                ClickLogout();
            }
            else
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
            }
        }

        private void ClickLogout()
        {
            LocalApplicationData.Instance.LoginInfo = new LoginObject();
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
