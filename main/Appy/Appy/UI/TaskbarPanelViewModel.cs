using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.UI
{
    public class TaskbarPanelViewModel
    {
        public List<ApplicationViewModel> PinnedApps { get; set; }
        
        public TaskbarPanelViewModel()
        {
            if (ServiceLocator.LocalStorage.PinnedApps == null)
            {
                ServiceLocator.LocalStorage.PinnedApps = new List<Application>();
            }

            foreach (var application in ServiceLocator.LocalStorage.PinnedApps)
            {
                PinnedApps.Add(new ApplicationViewModel(application));
            }

            ServiceLocator.LocalStorage.SaveAppSettings();
        }

        public void AddPinnedApp(ApplicationViewModel clickedApp)
        {
            PinnedApps.Add(clickedApp);
            ServiceLocator.LocalStorage.PinnedApps.Add(clickedApp.Application);
            ServiceLocator.LocalStorage.SaveAppSettings();
        }

        public void RemovePinnedApp(ApplicationViewModel clickedApp)
        {
            PinnedApps.Remove(clickedApp);
            ServiceLocator.LocalStorage.PinnedApps.Remove(clickedApp.Application);
            ServiceLocator.LocalStorage.SaveAppSettings();
        }
    }
}
