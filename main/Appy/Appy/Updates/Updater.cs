using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using AppDirect.WindowsClient.Properties;

namespace AppDirect.WindowsClient.Updates
{
    public class Updater
    {

        public const string UpdaterExe = "Appy_upd.exe";
        public Updater()
        {
            string currentVersion = GetCurrentVersion();

            CompareVersionAndDownloadUpdates(currentVersion);

            RunUpdater();
        }

        private void RunUpdater()
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = UpdaterExe;

            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;
            
            using (Process proc = Process.Start(start))
            {
                proc.WaitForExit();

                // Retrieve the app's exit code
                int exitcode = proc.ExitCode;
            }
        }

        private string GetCurrentVersion()
        {
            FileInfo applicationExe = new FileInfo(Resources.ApplicationExe);
            Assembly a = Assembly.LoadFile(applicationExe.FullName);
            AssemblyName name = a.GetName();
            return name.Version.ToString();
        }

        private void CompareVersionAndDownloadUpdates(string currentVersion)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var versionStream = client.OpenRead(Resources.VersionFileUrl);

                    var versionString = new StreamReader(versionStream).ReadToEnd();

                    if (currentVersion != versionString)
                    {
                        client.DownloadFile(Resources.UpdateFileUrl, UpdaterExe);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
