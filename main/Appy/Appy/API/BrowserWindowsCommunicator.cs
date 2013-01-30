using System;
using System.Collections.Generic;
using System.Diagnostics;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.API
{
    public class BrowserWindowsCommunicator
    {
        private readonly IDictionary<String, IntPtr> _chromiumWindows = new Dictionary<String, IntPtr>();

        private const string BrowserPostfix = ".Browser.exe";
        private const string AppIdParameterName = "--appid=";

        public void OpenApp(IApplication a)
        {
            var browserWindowProcess = new Process();
            browserWindowProcess.StartInfo.FileName = Helper.ApplicationName + BrowserPostfix;
            browserWindowProcess.StartInfo.Arguments = AppIdParameterName + "\"" + a.Id + "\"";
            browserWindowProcess.Start();
        }

        public void CloseApp(IApplication a)
        {
        }
    }
}
