using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Watcher
{
    public class Program
    {
        static int count = 3;

        static void Main()
        {
            Launch();
            Thread.Sleep(Timeout.Infinite);
        }

        static void Launch()
        {
            Process process = new Process();
            process.StartInfo.FileName = "AppDirectClient.exe";
            process.EnableRaisingEvents = true;
            process.Exited += LaunchIfCrashed;

            process.Start();
        }

        static void LaunchIfCrashed(object o, EventArgs e)
        {
            Process process = (Process)o;
            if (process.ExitCode != 0)
            {
                if (count-- > 0) // restart at max count times
                    Launch();
                else
                    Environment.Exit(process.ExitCode);
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
}
