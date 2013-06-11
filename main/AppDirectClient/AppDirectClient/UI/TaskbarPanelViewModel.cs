using System.Collections.Generic;

namespace AppDirect.WindowsClient.UI
{
    public class TaskbarPanelViewModel
    {
        public List<ApplicationViewModel> PinnedApps { get; set; }

        public TaskbarPanelViewModel(List<ApplicationViewModel> myApps)
        {
            PinnedApps = myApps;
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