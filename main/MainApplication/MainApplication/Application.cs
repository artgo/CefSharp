using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MainApplication
{
    public class Application
    {
        private ICommand _launchApp;

        public string Id { get; set; } 
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public Uri URL { get; set; }
        public int AlertCount { get; set; }

        public ICommand LaunchApp
        {
            get
            {
                if (_launchApp == null)
                {
                    _launchApp = new RelayCommand(param => this.Launch(),
                        null);
                }
                return _launchApp;
            }
        } 

        private void Launch()
        {
            System.Diagnostics.Process.Start("iexplore.exe", URL.AbsoluteUri);
        }
    }
}
