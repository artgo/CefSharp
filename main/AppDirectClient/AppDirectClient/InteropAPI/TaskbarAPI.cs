using System;
using System.Windows.Forms;
using AppDirect.WindowsClient.InteropAPI.Internal;
using Control = System.Windows.Controls.Control;
using Microsoft.Win32;
using AppDirect.WindowsClient.InteropAPI;
using AppDirect.WindowsClient.UI;

namespace AppDirect.WindowsClient.InteropAPI
{
    public class TaskbarApi
    {
        private TaskBarIcon _taskBarIcon;

        #region Singleton
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
        #endregion Singleton

        public int TaskbarHeight { get { return new TaskBarHelper().TaskBarRect.Height; } }
        public TaskbarPosition TaskbarPosition { get { return new TaskBarHelper().TaskBarPosition; } }
        public TaskbarIconsSize TaskbarIconsSize { get { return new TaskBarHelper().TaskBarIconSize; } }
        public Screen TaskbarScreen { get { return Screen.FromHandle(new TaskBarHelper().TaskBarHwnd); } }
        public Double DpiScalingFactor { get { return GetDpiScaleFactor(); } }

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
        /// <param name="initialWidth">Initial width</param>
        public void InsertTaskbarWindow(TaskbarPanel panel)
        {
            if (_taskBarIcon != null)
            {
                RemoveTaskbarWindowAndShutdown();
            }
            else
            {
                ControlWrapper wrapper = new ControlWrapper(panel);
                wrapper.DesiredOffset = panel.GetCurrentDimension();
                _taskBarIcon = new TaskBarIcon(wrapper);
                _taskBarIcon.Setup();
                panel.CurrentDimensionChanged += panel_CurrentDimensionChanged;
                wrapper.AllowedSizeChanged += wrapper_AllowedSizeChanged;
            }
        }

        void wrapper_AllowedSizeChanged(object sender, EventArgs e)
        {
            if (_taskBarIcon != null && _taskBarIcon.Wrapper.Control.GetType() == typeof(TaskbarPanel))
            {
                int allowedWidth = _taskBarIcon.Wrapper.AllowedSize.Width;
                int allowedHeight = _taskBarIcon.Wrapper.AllowedSize.Height;
                ((TaskbarPanel)_taskBarIcon.Wrapper.Control).LayoutIcons(allowedHeight, allowedWidth);
            }
        }

        void panel_CurrentDimensionChanged(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(TaskbarPanel)) 
            {
                _taskBarIcon.Wrapper.DesiredOffset = ((TaskbarPanel)sender).GetCurrentDimension();
            }
        }

        public void RemoveTaskbarWindowAndShutdown()
        {
            // use _control
            if (_taskBarIcon != null)
            {
                _taskBarIcon.TearDown();
                _taskBarIcon = null;
            }
        }

        #endregion public interface

        private const string DpiSettingPath = @"HKEY_CURRENT_USER\Control Panel\Desktop\Windowmetrics";
        private const string DpiSettingName = @"AppliedDPI";
        private const double StandardDpi = 96;

        private double GetDpiScaleFactor()
        {
            var dpiSetting = (double)(int)(Registry.GetValue(DpiSettingPath, DpiSettingName, StandardDpi) ?? StandardDpi);
            return dpiSetting / StandardDpi;
        }
    }
}