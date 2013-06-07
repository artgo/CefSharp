using System;
using System.Windows.Forms;
using AppDirect.WindowsClient.InteropAPI.Internal;
using Control = System.Windows.Controls.Control;
using Microsoft.Win32;
using AppDirect.WindowsClient.InteropAPI;
using AppDirect.WindowsClient.UI;
using System.Windows.Threading;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class TaskBarApi : ITaskbarApi
    {
        public TaskBarApi()
        {
        }

        private TaskbarHost _taskBarIcon;

        /// <summary>
        /// Place WPF window on Taskbar
        /// </summary>
        /// <param name="control">WPF window to be placed on taskbar</param>
        /// <param name="initialWidth">Initial width</param>
        public void InsertPanel(TaskbarPanel panel)
        {
            if (_taskBarIcon != null)
            {
                RemovePanel();
            }

            _taskBarIcon = new TaskbarHost(panel);

            _taskBarIcon.SetTaskBarControl(panel);
            panel.SetTaskBarHost(_taskBarIcon);
            
            _taskBarIcon.Setup();
        }

        public void RemovePanel()
        {
            // use _control
            if (_taskBarIcon != null)
            {
                _taskBarIcon.TearDown();
                _taskBarIcon = null;
            }
        }
    }
}