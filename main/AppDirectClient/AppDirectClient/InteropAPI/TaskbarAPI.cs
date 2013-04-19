using System;
using System.Windows.Forms;
using AppDirect.WindowsClient.InteropAPI.Internal;
using Control = System.Windows.Controls.Control;

namespace AppDirect.WindowsClient.InteropAPI
{
    public class TaskbarApi
    {
        private readonly InteractionsObject _interactionsObject;
        private volatile Control _control = null;

        #region Singleton
        private static readonly object SyncObject = new object();
        private static volatile TaskbarApi _instance = null;

        private TaskbarApi()
        {
            _interactionsObject = new InteractionsObject();
            _interactionsObject.LoadInitialValues();
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
        #endregion Singleton

        public int TaskbarHeight { get { return _interactionsObject.TaskbarHeight; } }
        public TaskbarPosition TaskbarPosition { get { return _interactionsObject.TaskbarPosition; } }
        public TaskbarIconsSize TaskbarIconsSize { get { return _interactionsObject.TaskbarIconsSize; } }
        public Screen TaskbarScreen { get { return _interactionsObject.TaskbarScreen; } }
        public Double DpiScalingFactor { get { return _interactionsObject.DpiScalingFactor; } }

        public static void Cleanup()
        {
            if (_instance != null)
			{
				// TODO: -2 if needed
			}
        }

        #region public interface

        /// <summary>
        /// Place WPF window on Taskbar
        /// </summary>
        /// <param name="control">WPF window to be placed on taskbar</param>
        /// <param name="notifyee">An object which will be notified upon taskbar changes</param>
        /// <param name="initialWidth">Initial width</param>
        public void InsertTaskbarWindow(Control control, ITaskbarInterop notifyee, int initialWidth)
        {
            _control = control;
            _interactionsObject.Place(_control, notifyee, initialWidth);
        }
        #endregion public interface

        public delegate void ShutdownCallback();

        internal bool RemoveTaskbarWindowAndShutdown(ShutdownCallback shutdownCallback)
        {
            // use _control
            return _interactionsObject.Remove(shutdownCallback);
        }
    }
}