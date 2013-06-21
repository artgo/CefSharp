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
        private readonly Action _actionOnStartup;
        private readonly Action _actionOnShutdown;
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

        public ExplorerWatcher(ILogger logger, IUiHelper uiHelper, Action actionOnStartup, Action actionOnShutdown)
        {
            _logger = logger;
            _uiHelper = uiHelper;
            _actionOnStartup = actionOnStartup;
            _actionOnShutdown = actionOnShutdown;
        }

        public void Start()
        {
            WaitForExplorerProcess();

            if (_explorerProcess != null)
            {
                _actionOnStartup.Invoke();
            }
        }

        public void Stop()
        {
            _uiHelper.IgnoreException(() => _explorerProcess.Exited -= OnExplorerCrash);
        }

        private void WaitForExplorerProcess()
        {
            var helper = ServiceLocator.GetTaskbarHelper();

            helper.WaitForRebar(_logger);

            if (helper.IsTaskbarPresent)
            {
                _explorerProcess = helper.ExplorerProcess;
                try
                {
                    _explorerProcess.EnableRaisingEvents = true;
                    _explorerProcess.Exited += OnExplorerCrash;
                    return;
                }
                catch (Win32Exception e)
                {
                    _logger.InfoException("Exception thrown by attempting to EnableRaisingEvents", e);
                    //Vista users may not have the permissions necessary to set EnableRaisingEvents to true
                }
            }
        }

        private void OnExplorerCrash(object o, EventArgs e)
        {
            _actionOnShutdown.Invoke();

            _explorerProcess = null;
            Start();
        }
    }
}