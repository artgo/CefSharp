using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.InteropAPI.Internal;
using AppDirect.WindowsClient.Properties;
using AppDirect.WindowsClient.Storage;

namespace AppDirect.WindowsClient.Updates
{
    public class Updater
    {
        public static readonly string UpdaterExeFileName = "updater.exe";
        public const double RetryInterval = 15d;
        private const int RetryUpdatesLimit = 3;

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
                catch (Exception)
                {
                    retryCount += 1;
                    Thread.Sleep(TimeSpan.FromMinutes(RetryInterval * retryCount));
                }
            }

            return false;
        }

        private void InstallUpdatesWhenIdle()
        {
            
        }

        private static int GetLastInputTime()
        {
            uint idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            uint envTicks = (uint)Environment.TickCount;

            if (User32Dll.GetLastInputInfo(ref lastInputInfo))
            {
                uint lastInputTick = lastInputInfo.dwTime;

                idleTime = envTicks - lastInputTick;
            }

            return (int)((idleTime > 0) ? (idleTime / 1000) : 0);
        }

        /// <summary>
        /// Attempts to run updater.  If the process can not be started (Process.Start throws an exception) or the process starts successfully, the value of the UpdateDownloaded switch is set to false
        /// </summary>
        /// <param name="currentVersion"></param>
        /// <returns></returns>
        public virtual void InstallUpdates()
        {
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = UpdaterExeFileName;

                start.WindowStyle = ProcessWindowStyle.Hidden;
                start.CreateNoWindow = true;

                if (System.Environment.OSVersion.Version.Major >= 6)
                {
                    start.Verb = "runas";
                }
                Process.Start(start);
            }
            catch (Exception e)
            {
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
        /// Wrap in try catch. Throws exceptions if there are network problems or if the Version file or the update file fail to download and save
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
