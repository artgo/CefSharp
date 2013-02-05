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
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Properties;
using AppDirect.WindowsClient.Storage;

namespace AppDirect.WindowsClient.Updates
{
    public class Updater
    {
        public static readonly string UpdaterExeFileName = Helper.ApplicationName + "_upd.exe";
        public const double RetryInterval = 15d;


        public Updater()
        {
            if (ServiceLocator.LocalStorage.UpdateDownloaded)
            {
                InstallUpdates();
            }
        }

        public bool GetUpdates(string currentVersion)
        {
            int retryCount = 0;
            while (retryCount < 3)
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
                    retryCount += 1;
                    Thread.Sleep(TimeSpan.FromMinutes(RetryInterval * retryCount));
                }
            }

            return false;
        }

        public void InstallUpdates()
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = UpdaterExeFileName;

            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;

            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                start.Verb = "runas";
            }

            try
            {
                Process.Start(start);
                ServiceLocator.LocalStorage.UpdateDownloaded = false;
                ServiceLocator.LocalStorage.SaveAppSettings();
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
                    client.DownloadFile(Resources.UpdateFileUrl, UpdaterExeFileName);
                    return true;
                }
            }

            return false;
        }
    }
}
