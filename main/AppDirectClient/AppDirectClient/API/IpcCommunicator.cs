using AppDirect.WindowsClient.Common;
using AppDirect.WindowsClient.Common.API;
using System.Diagnostics;
using AppDirect.WindowsClient.Common.Log;

namespace AppDirect.WindowsClient.API
{
    public class IpcCommunicator : AbstractServiceRunner<MainApplication>, IIpcCommunicator
    {
        private static readonly string BrowserProjectName = Helper.BrowserProject + Helper.ExeExt;
        private volatile ProcessWatcher _watcher;

        public IpcCommunicator(MainApplication service)
            : base(service)
        {
        }

        public override void Start()
        {
            base.Start();
            var process = StartBrowserProcess();

            _watcher = new ProcessWatcher(process);
            _watcher.Start();
        }

        protected virtual Process StartBrowserProcess()
        {
            var browserWindowProcess = new Process { StartInfo = { FileName = BrowserProjectName } };
            browserWindowProcess.Start();
            return browserWindowProcess;
        }

        public override void Stop()
        {
            _watcher.Stop();
            base.Stop();
        }
    }
}