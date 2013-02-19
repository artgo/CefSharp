using System.Windows.Controls;
using AppDirect.WindowsClient.InteropAPI.Internal;

namespace AppDirect.WindowsClient.InteropAPI
{
    public class TaskbarApi
    {
        private static readonly object SyncObject = new object();

        private readonly InteractionsObject _interactionsObject;
        private volatile Control _contorl = null;

        #region Singleton
        private static volatile TaskbarApi _instance = null;

        private TaskbarApi()
        {
            _interactionsObject = new InteractionsObject();
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

        public static void Cleanup()
        {
            if (_instance != null)
			{
				if (_instance._interactionsObject != null)
					;

				;	// TODO: -2 if needed
			}
        }

        #region public interface

        /// <summary>
        /// Place WPF window on Taskbar
        /// </summary>
        /// <param name="control">WPF window to be placed on taskbar</param>
        /// <param name="notifyee">An object which will be notified upon taskbar changes</param>
        /// <param name="initialWidth"></param>
        public void InsertTaskbarWindow(Control control, ITaskbarInterop notifyee, int initialWidth)
        {
            _contorl = control;
            _interactionsObject.Place(_contorl, notifyee, initialWidth);
        }

        public void RemoveTaskbarWindow()
        {
            // use _control
			_interactionsObject.Remove();
        }
        #endregion public interface
    }
}