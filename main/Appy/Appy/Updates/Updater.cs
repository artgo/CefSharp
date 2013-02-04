using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using AppDirect.WindowsClient.Properties;
using AppDirect.WindowsClient.Storage;

namespace AppDirect.WindowsClient.Updates
{
    public class Updater
    {
        public const string UpdaterExe = "Appy_upd.exe";

        public Updater()
        {
            if (ServiceLocator.LocalStorage.UpdateDownloaded)
            {
                ServiceLocator.LocalStorage.UpdateDownloaded = false;
                ServiceLocator.LocalStorage.SaveAppSettings();
                InstallUpdates();
            }
        }

        public bool GetUpdates(string currentVersion)
        {
            while (true)
            {
                try
                {
                    if (CheckVersionGetUpdates(currentVersion))
                    {
                        ServiceLocator.LocalStorage.UpdateDownloaded = true;
                        ServiceLocator.LocalStorage.SaveAppSettings();
                        return true;
                    }

                    return false;
                }
                catch (Exception)
                {
                    Thread.Sleep(TimeSpan.FromMinutes(.5d));
                }
            }
        }

        public void InstallUpdates()
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = UpdaterExe;

            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;

            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                start.Verb = "runas";
            }

            try
            {
                Process.Start(start);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private bool CheckVersionGetUpdates(string currentVersion)
        {

            using (var client = new WebClient())
            {
                var versionStream = client.OpenRead(Resources.VersionFileUrl);

                var versionString = new StreamReader(versionStream).ReadToEnd();

                if (currentVersion != versionString)
                {
                    client.DownloadFile(Resources.UpdateFileUrl, UpdaterExe);
                    return true;
                }
            }

            return false;
        }
    }
}
