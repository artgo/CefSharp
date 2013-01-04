using System;
using System.Windows.Input;
using AppDirect.WindowsClient.UI;

namespace AppDirect.WindowsClient.Models
{
    ///<summary>
    /// Represents an Application of the sort that AppDirect distributes 
    ///</summary>
    [Serializable]
    public class Application
    {
        private ICommand _launchApp;
        public string Id { get; set; } 
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public int AlertCount { get; set; }
        public bool IsLocalApp { get; set; }
        public string UrlString { get; set; }
        
        public ICommand LaunchApp
        {
            get
            {
                if (_launchApp == null)
                {
                    _launchApp = new RelayCommand<object>(param => Launch(),
                        null);
                }
                return _launchApp;
            }
        } 

        private void Launch()
        {
            System.Diagnostics.Process.Start("iexplore.exe", UrlString);
        }
    }
}
