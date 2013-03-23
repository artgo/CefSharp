using System.Threading;
using AppDirect.WindowsClient.Common.API;
using System;
using System.Diagnostics;

namespace AppDirect.WindowsClient.API
{
    public class BrowserWindowsCommunicator : IBrowserWindowsCommunicator
    {
        private static readonly string BrowserPostfix = Helper.BrowserProjectExt + Helper.ExeExt;
        private const string AppIdParameterName = "--appid=";

        private readonly IIpcCommunicator _ipcCommunicator;

        public BrowserWindowsCommunicator(IIpcCommunicator ipcCommunicator)
        {
            _ipcCommunicator = ipcCommunicator;
        }

        public void OpenOrActivateApp(IApplication application)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            (new Thread(() =>
                {
                    var browserExists = _ipcCommunicator.ActivateBrowserIfExists(application.Id);

                    if (!browserExists)
                    {
                        var browserWindowProcess = new Process();
                        browserWindowProcess.StartInfo.FileName = Helper.ApplicationName + BrowserPostfix;
                        browserWindowProcess.StartInfo.Arguments = AppIdParameterName + "\"" + application.Id + "\"";
                        browserWindowProcess.Start();
                    }
                })).Start();
        }

        public void CloseApp(IApplication application)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            _ipcCommunicator.CloseBrowser(application.Id);
        }
    }
}