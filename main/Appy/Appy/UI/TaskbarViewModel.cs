using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.UI
{
    public class TaskbarViewModel
    {
        public List<Application> PinnedApps { get; set; }
        
        public TaskbarViewModel()
        {
            if (ServiceLocator.LocalStorage.PinnedApps == null)
            {
                ServiceLocator.LocalStorage.PinnedApps = new List<Application>();
            }

            PinnedApps = ServiceLocator.LocalStorage.PinnedApps;
        }

        public void AddPinnedApp(Application clickedApp)
        {
            PinnedApps.Add(clickedApp);
            ServiceLocator.LocalStorage.SaveAppSettings();
        }

        public void RemovePinnedApp(Application clickedApp)
        {
            PinnedApps.Remove(clickedApp);
            ServiceLocator.LocalStorage.SaveAppSettings();
        }
    }
}
