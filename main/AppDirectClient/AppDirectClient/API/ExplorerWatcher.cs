using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;

namespace AppDirect.WindowsClient.API
{
    public class ExplorerWatcher : IStartStop
    {
        private volatile Process _explorerProcess;
        private readonly Action _actionOnCrash;
        private readonly ILogger _logger;
        private readonly IUiHelper _uiHelper;
        private int _currentUserSessionId;
        private const string ExplorerProcessName = "explorer";

        private int CurrentUserSessionId
        {
            get
            {
                if (_currentUserSessionId == 0)
                {
                    var currentProcess = Process.GetCurrentProcess();
                    _currentUserSessionId = currentProcess.SessionId;
                }
                return _currentUserSessionId;
            }
        }

        public ExplorerWatcher(ILogger logger, IUiHelper uiHelper, Action actionOnCrash)
        {
            _logger = logger;
            _uiHelper = uiHelper;
            _actionOnCrash = actionOnCrash;
        }

        public void Start()
        {
            GetExplorerProcess();
        }

        public void Stop()
        {
            _uiHelper.IgnoreException(() => _explorerProcess.Exited -= LaunchIfCrashed);
        }

        private void GetExplorerProcess()
        {
            while (_explorerProcess == null)
            {
                var helper = ServiceLocator.GetTaskbarHelper();
                var process = helper.ExplorerProcess;

                if (helper.IsTaskbarPresent)
                {
                    _explorerProcess = helper.ExplorerProcess;
                    try
                    {
                        _explorerProcess.EnableRaisingEvents = true;
                        _explorerProcess.Exited += LaunchIfCrashed;
                        return;
                    }
                    catch (Win32Exception e)
                    {
                        _logger.InfoException("Exception thrown by attempting to EnableRaisingEvents", e);
                        //Vista users may not have the permissions necessary to set EnableRaisingEvents to true
                    }
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
                _explorerProcess = null;
                GetExplorerProcess(); 
                _uiHelper.Sleep(200);
                ServiceLocator.GetTaskbarHelper().WaitForRebar(_logger);
                _actionOnCrash.Invoke();
            }
        }
    }
}