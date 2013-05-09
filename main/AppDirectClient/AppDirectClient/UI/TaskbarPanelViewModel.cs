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
            PinnedApps = new List<ApplicationViewModel>();

            if (ServiceLocator.LocalStorage.AllInstalledApplications == null)
            {
                return;
            }

            foreach (var application in ServiceLocator.LocalStorage.AllInstalledApplications.Where(a => a.PinnedToTaskbar))
            {
                PinnedApps.Add(new ApplicationViewModel(application));
            }

            ServiceLocator.LocalStorage.SaveAppSettings();
        }

        public void AddPinnedApp(ApplicationViewModel clickedApp)
        {
            PinnedApps.Add(clickedApp);
        }

        public void RemovePinnedApp(ApplicationViewModel clickedApp)
        {
            PinnedApps.Remove(clickedApp);
        }

        public void RemoveAllPinnedApps()
        {
            PinnedApps.Clear();
        }
    }
}
