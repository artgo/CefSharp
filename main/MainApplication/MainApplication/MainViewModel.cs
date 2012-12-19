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
                    myApps.AddRange(ApplicationSettings.Instance.InstalledLocalApps); 
                    return myApps;
                }

                return ApplicationSettings.Instance.InstalledLocalApps;
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
                    return ApplicationSettings.Instance.SuggestedLocalApps;
                }

                var suggestedApps = ApplicationSettings.Instance.SuggestedLocalApps;
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
            ApplicationSettings.Instance.InstalledLocalApps.Remove(application);
            MyApplications.Remove(application);
            SuggestedApplications.Add(application);
            MyApplications.Add(new Application());
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

            ApplicationSettings.Instance.InstalledLocalApps.Add(application);
            MyApplications[MyApplicationsList.Count - 1] = application;
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
