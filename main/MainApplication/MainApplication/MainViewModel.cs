using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace MainApplication
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ICommand _uninstallApp;
        private ICommand _installApp;
        private string _userName;

        public List<Application> MyApplicationsList
        {
            get
            {
                if (!String.IsNullOrEmpty(_userName))
                {
                    var myApps = CachedAppDirectApi.Instance.MyApps.ToList();
                    myApps.AddRange(LocalApplicationData.Instance.InstalledLocalApps); 
                    return myApps;
                }

                return LocalApplicationData.Instance.InstalledLocalApps;
            }
        }

        public string DisplayUserName
        {
            get
            {
                if (String.IsNullOrEmpty(_userName))
                {
                    return "Log In";
                }
                return _userName;
            }
            set { _userName = value; }
        }

        public List<Application> SuggestedApplicationsList
        {
            get
            {
                if (String.IsNullOrEmpty(_userName))
                {
                    return LocalApplicationData.Instance.SuggestedLocalApps;
                }

                var suggestedApps = LocalApplicationData.Instance.SuggestedLocalApps;
                suggestedApps.AddRange(CachedAppDirectApi.Instance.SuggestedApps.ToList());

                return suggestedApps;
            }
        }

        public ObservableCollection<Application> MyApplications { get; set; }
        public ObservableCollection<Application> SuggestedApplications  { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
       
        public MainViewModel() 
        {
            MyApplications = new ObservableCollection<Application>(MyApplicationsList);
            SuggestedApplications = new ObservableCollection<Application>(SuggestedApplicationsList);

            while (MyApplications.Count < 12)
            {
                MyApplications.Add(new Application());
            }
        }

        public ICommand UninstallApp
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

        private void Uninstall(Application application)
        {
            LocalApplicationData.Instance.InstalledLocalApps.Remove(application);
            LocalApplicationData.Instance.SuggestedLocalApps.Add(application);
            MyApplications.Remove(application);
            SuggestedApplications.Add(application);
            MyApplications.Add(new Application());

            LocalApplicationData.SaveAppSettings();
        }

        public ICommand InstallApp
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

        private void Install(Application application)
        {
            SuggestedApplications.Remove(application);

            LocalApplicationData.Instance.InstalledLocalApps.Add(application);
            LocalApplicationData.Instance.SuggestedLocalApps.Remove(application);
            MyApplications[MyApplicationsList.Count - 1] = application;

            LocalApplicationData.SaveAppSettings();
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
