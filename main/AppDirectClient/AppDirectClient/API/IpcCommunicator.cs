using AppDirect.WindowsClient.Common;
using AppDirect.WindowsClient.Common.API;
using System.Diagnostics;
using AppDirect.WindowsClient.Common.Log;

namespace AppDirect.WindowsClient.API
{
    public class IpcCommunicator : AbstractServiceRunner<MainApplication>, IIpcCommunicator
    {
        private static readonly string BrowserProjectName = Helper.BrowserProject + Helper.ExeExt;

        public IpcCommunicator(MainApplication service)
            : base(service)
        {
        }

        public override void Start()
        {
            base.Start();
            StartBrowserProcess();

            ServiceLocator.BrowserWatcher.Start();
        }

        protected virtual void StartBrowserProcess()
        {
            var browserWindowProcess = new Process { StartInfo = { FileName = BrowserProjectName } };
            browserWindowProcess.Start();
        }
    }
}