using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MainApplication
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public IEnumerable<Application> MyApplications { get; set; }
        public IEnumerable<Application> SuggestedApplications { get; set; }
        public Defaults Defaults { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
       
        public MainViewModel() 
        {
            Defaults = new Defaults();

            MyApplications = Defaults.InstalledLocalApps;
            SuggestedApplications = CachedAppDirectApi.Instance.SuggestedApps;
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
