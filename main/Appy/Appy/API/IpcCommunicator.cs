using AppDirect.WindowsClient.Common.API;
using System.Diagnostics;
using System.Threading;

namespace AppDirect.WindowsClient.API
{
    public class IpcCommunicator : AbstractServiceRunner<MainApplication>, IIpcCommunicator
    {
        private static readonly string BrowserPostfix = Helper.BrowserProjectExt + Helper.ExeExt;

        public IpcCommunicator(MainApplication service)
            : base(service)
        {
        }

        public override void Start()
        {
            base.Start();

            StartBrowserProcess();
        }

        protected virtual void StartBrowserProcess()
        {
            var browserWindowProcess = new Process { StartInfo = { FileName = Helper.ApplicationName + BrowserPostfix } };
            browserWindowProcess.Start();
        }
    }
}