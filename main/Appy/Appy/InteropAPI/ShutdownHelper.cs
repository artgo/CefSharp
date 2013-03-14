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
			Application.Current.Dispatcher.Invoke(new System.Action<Application>((_) => { Application.Current.Shutdown(); }), Application.Current);
			var ver = System.Environment.OSVersion.Version;
			if (ver.Major >= 6 && ver.Minor >= 2)	// win8
			{
				// force shutdown due to it structs with win8 tablets
				System.Diagnostics.Process.GetCurrentProcess().Kill();
			}
        }

        public bool Shutdown()
        {
            return TaskbarApi.Instance.RemoveTaskbarWindowAndShutdown(_shutdownCallback);
        }
    }
}