using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AppDirect.WindowsClient.Common.UI;

namespace AppDirect.WindowsClient.API
{
    public class ExplorerWatcher
    {
        public Process ExplorerProcess;
        private readonly Action _actionOnCrash;
        private readonly IUiHelper _uiHelper;
        private const string ExplorerProcessName = "explorer";

        public ExplorerWatcher(IUiHelper uiHelper, Action actionOnCrash)
        {
            _uiHelper = uiHelper;
            _actionOnCrash = actionOnCrash;
        }

        public void Start()
        {
            GetExplorerProcess();
        }

        public void Stop()
        {
            GetExplorerProcess();
            ExplorerProcess.Exited -= LaunchIfCrashed;
        }

        private void GetExplorerProcess()
        {
            while (ExplorerProcess == null)
            {
                var processesByName = Process.GetProcessesByName(ExplorerProcessName);

                if (processesByName.Any())
                {
                    ExplorerProcess = processesByName[0];
                    ExplorerProcess.EnableRaisingEvents = true;
                    ExplorerProcess.Exited += LaunchIfCrashed;
                }
                else
                {
                    _uiHelper.Sleep(500);
                }
            }
        }

        private void LaunchIfCrashed(object o, EventArgs e)
        {
            Process process = (Process)o;
            if (process.ExitCode != 0)
            {
                ExplorerProcess = null;
                GetExplorerProcess();
                ExplorerProcess.WaitForInputIdle();
                _uiHelper.Sleep(500);
                _actionOnCrash.Invoke();
            }
        }
    }
}