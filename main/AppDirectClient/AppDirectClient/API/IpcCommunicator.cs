﻿using AppDirect.WindowsClient.Common;
using AppDirect.WindowsClient.Common.API;
using System.Diagnostics;
using AppDirect.WindowsClient.Common.Log;

namespace AppDirect.WindowsClient.API
{
    public class IpcCommunicator : AbstractServiceRunner<MainApplication>, IIpcCommunicator
    {
        private static readonly string BrowserProjectName = Helper.BrowserProject + Helper.ExeExt; 
        private volatile ProcessWatcher _browserWatcher;

        public IpcCommunicator(MainApplication service, ProcessWatcher browserWatcher)
            : base(service)
        {
            _browserWatcher = browserWatcher;
        }

        public override void Start()
        {
            base.Start();
            StartBrowserProcess();

            _browserWatcher.Start();
        }

        protected virtual void StartBrowserProcess()
        {
            var browserWindowProcess = new Process { StartInfo = { FileName = BrowserProjectName } };
            browserWindowProcess.Start();
        }
    }
}