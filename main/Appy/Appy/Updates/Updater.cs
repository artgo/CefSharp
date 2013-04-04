﻿using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;

namespace AppDirect.WindowsClient.Updates
{
    public class Updater
    {
        public static readonly string UpdaterExeFileName = "updater.exe";
        public const double RetryInterval = 15d;
        private const int RetryUpdatesLimit = 3;

        private static readonly ILogger _log = new NLogLogger("Updater");

        public bool GetUpdates(string currentVersion)
        {
            int retryCount = 0;
            while (retryCount < RetryUpdatesLimit)
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
                catch (Exception e)
                {
                    _log.ErrorException("Error during checking for updates", e);

                    retryCount += 1;
                    Thread.Sleep(TimeSpan.FromMinutes(RetryInterval * retryCount));
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to run updater.  If the process can not be started (Process.Start throws an exception) or the process starts successfully, the value of the UpdateDownloaded switch is set to false
        /// </summary>
        /// <returns></returns>
        public virtual void InstallUpdates()
        {
            try
            {
                var updater = new Process();
                updater.StartInfo.FileName = UpdaterExeFileName;
                updater.StartInfo.UseShellExecute = true;
                updater.Start();
            }
            catch (Exception e)
            {
                _log.ErrorException("Error starting updator process", e);

                MessageBox.Show(e.Message);
            }
            finally
            {
                ServiceLocator.LocalStorage.UpdateDownloaded = false;
                ServiceLocator.LocalStorage.SaveAppSettings();
            }
        }

        /// <summary>
        /// Wrap in try catch. Throws exceptions if there are network problems or if the Version file or the update file fail to download and save
        /// </summary>
        /// <param name="currentVersion"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Wrap in try catch. Throws exceptions if there are network problems or if the Version file fails to download and save
        /// </summary>
        /// <param name="currentVersion"></param>
        /// <returns></returns>
        public bool CheckVersion(string currentVersion)
        {
            using (var client = new WebClient())
            {
                var versionStream = client.OpenRead(Resources.VersionFileUrl);
                var versionString = new StreamReader(versionStream).ReadToEnd();
                return currentVersion != versionString;
            }
        }
    }
}