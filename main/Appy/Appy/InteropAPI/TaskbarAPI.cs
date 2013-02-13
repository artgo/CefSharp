using System.Windows.Controls;

namespace AppDirect.WindowsClient.InteropAPI
{
    public class TaskbarApi
    {
        private static readonly object SyncObject = new object();
        private static volatile TaskbarApi _instance = null;

        private TaskbarApi()
        {
        }

        public static TaskbarApi Instance
        {
            get
            {
                lock (SyncObject)
                {
                    if (_instance == null)
                    {
                        _instance = new TaskbarApi();
                    }
                }

                return _instance;
            }
        }

        public void SetTaskbarWindow(Control control, ITaskbarInterop interopObject)
        {
            // MUST SET interopObject.TaskbarCallbackEvents = ...
        }

        public void RemoveTaskbarWindow()
        {

        }
    }
}