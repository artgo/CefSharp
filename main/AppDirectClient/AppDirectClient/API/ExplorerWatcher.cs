using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AppDirect.WindowsClient.API
{
    public static class ExplorerWatcher
    {
        public static Process ExplorerProcess;
        private static Action ActionOnCrash;
        private const string ExplorerProcessName = "explorer";

        public static void Start(Action actionOnCrash)
        {
            GetExplorerProcess();
            ActionOnCrash = actionOnCrash;
        }

        public static void Stop()
        {
            GetExplorerProcess();
            ExplorerProcess.Exited -= LaunchIfCrashed;
        }

        private static void GetExplorerProcess()
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
                    Thread.Sleep(500);
                }
            }
        }

        private static void LaunchIfCrashed(object o, EventArgs e)
        {
            Process process = (Process)o;
            if (process.ExitCode != 0)
            {
                ExplorerProcess = null;
                GetExplorerProcess();
                Thread.Sleep(500);
                ActionOnCrash.Invoke();
            }
        }
    }
}