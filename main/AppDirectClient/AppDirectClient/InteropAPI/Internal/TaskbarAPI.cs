using System;
using System.Windows.Forms;
using AppDirect.WindowsClient.InteropAPI.Internal;
using Control = System.Windows.Controls.Control;
using Microsoft.Win32;
using AppDirect.WindowsClient.InteropAPI;
using AppDirect.WindowsClient.UI;

namespace AppDirect.WindowsClient.InteropAPI
{
    public class TaskBarApi : ITaskBarApi
    {
        public TaskBarApi()
        {
        }

        private TaskBarIcon _taskBarIcon;

        /// <summary>
        /// Place WPF window on Taskbar
        /// </summary>
        /// <param name="control">WPF window to be placed on taskbar</param>
        /// <param name="initialWidth">Initial width</param>
        public void InsertTaskbarWindow(TaskbarPanel panel)
        {
            if (_taskBarIcon != null)
            {
                RemoveTaskbarWindow();
            }

            _taskBarIcon = new TaskBarIcon(panel);

            _taskBarIcon.SetTaskBarControl(panel);
            panel.SetTaskBarHost(_taskBarIcon);
            
            _taskBarIcon.Setup();
        }

        public void RemoveTaskbarWindow()
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