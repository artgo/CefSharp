using System.Windows;

namespace AppDirect.WindowsClient.InteropAPI
{
    public class ShutdownHelper
    {
        public static void Shutdown()
        {
            TaskbarApi.Instance.RemoveTaskbarWindow();
            TaskbarApi.Cleanup();
            Application.Current.Shutdown();
        }
    }
}
