using System.Windows;

namespace AppDirect.WindowsClient.InteropAPI
{
    public class ShutdownHelper
    {
        private readonly TaskbarApi.ShutdownCallback _shutdownCallback;

        #region Singleton

        private static readonly object SyncObject = new object();
        private static volatile ShutdownHelper _instance = null;

        private ShutdownHelper()
        {
            _shutdownCallback = DoShutdown;
        }

        public static ShutdownHelper Instance
        {
            get
            {
                lock (SyncObject)
                {
                    if (_instance == null)
                    {
                        _instance = new ShutdownHelper();
                    }
                }

                return _instance;
            }
        }

        #endregion Singleton

        private void DoShutdown()
        {
            TaskbarApi.Cleanup();
            Application.Current.Shutdown();
        }

        public bool Shutdown()
        {
            return TaskbarApi.Instance.RemoveTaskbarWindowAndShutdown(_shutdownCallback);
        }
    }
}