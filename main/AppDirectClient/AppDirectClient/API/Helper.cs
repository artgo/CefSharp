﻿using AppDirect.WindowsClient.Common.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;
using AppDirect.WindowsClient.InteropAPI.Internal;
using AppDirect.WindowsClient.UI;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace AppDirect.WindowsClient.API
{
    public class Helper
    {
        private Helper()
        {
        }

        public static readonly AssemblyName AssemblyName = Assembly.GetExecutingAssembly().GetName();
        public static readonly string ApplicationName = AssemblyName.Name;
        public static readonly string ApplicationVersion = AssemblyName.Version.ToString();
        public static readonly string ApplicationDirectory = @"\AppDirect\" + ApplicationName;
        public static readonly string BrowserProject = "BrowserManager";
        public static readonly string ExeExt = ".exe";
        private const int MinimumPasswordLength = 4;
        private const int MaximumPasswordLength = 18;
        public static readonly Regex EmailMatchPattern = new Regex(@"^([0-9a-zA-Z]([-\.\w\+]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
        public static readonly Regex PasswordMatchPattern = new Regex(@"^(.{" + MinimumPasswordLength + "," + MaximumPasswordLength + "})$");
        public static readonly int DefaultBrowserWidth = 1000;
        public static readonly int DefaultBrowserHeight = 581;
        public static readonly bool DefaultBrowserResizable = true;
        public static readonly string BaseAppStoreDomainName = Properties.Resources.BaseAppStoreUrl;
        public static readonly string BaseAppStoreUrl = Properties.Resources.BaseUrlProtocol + BaseAppStoreDomainName;
        private static readonly IUiHelper UiHelper = new UiHelper(new NLogLogger("UiHelper"));
        private static readonly ILogger Log = new NLogLogger("Helper");

        public static void RetryAction(Action action, int numberOfTries, TimeSpan retryInterval, Action catchAction = null)
        {
            var tryAttemptsRemaining = numberOfTries;
            var accumulatingTimeSpan = retryInterval;

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            do
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception e)
                {
                    Log.ErrorException("Failed to invoke action", e);

                    if (catchAction != null)
                    {
                        catchAction();
                    }

                    if (tryAttemptsRemaining <= 1)
                    {
                        throw;
                    }

                    ServiceLocator.UiHelper.Sleep(accumulatingTimeSpan);
                    accumulatingTimeSpan += retryInterval;
                }

                tryAttemptsRemaining--;
            } while (tryAttemptsRemaining > 0);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="action"></param>
        /// <param name="numberOfTries"></param>
        /// <param name="retryInterval"></param>
        /// <param name="catchAction"></param>
        /// <param name="throwExceptions"></param>
        /// <returns>Default if fails</returns>
        public static ReturnType RetryAction<ReturnType>(Func<ReturnType> action, int numberOfTries, TimeSpan retryInterval, Action catchAction = null, bool throwExceptions = true)
        {
            var tryAttemptsRemaining = numberOfTries;
            var accumulatingTimeSpan = retryInterval;

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            do
            {
                try
                {
                    var result = action();
                    return result;
                }
                catch (Exception e)
                {
                    Log.ErrorException("Failed to invoke action", e);

                    if (catchAction != null)
                    {
                        catchAction();
                    }

                    if (tryAttemptsRemaining <= 1 && throwExceptions)
                    {
                        throw;
                    }

                    Thread.Sleep(accumulatingTimeSpan);
                    accumulatingTimeSpan += retryInterval;
                }

                tryAttemptsRemaining--;
            } while (tryAttemptsRemaining > 0);

            return default(ReturnType);
        }

        public static ApplicationViewModel GetApplicationFromButtonSender(object sender)
        {
            return ((Button)sender).DataContext as ApplicationViewModel;
        }

        public static void AppButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var clickedApp = GetApplicationFromButtonSender(sender);

                //if the status is not active the application is disabled in the UI
                if (clickedApp.Application.ApplicationStatus == Status.Active)
                {
                    LaunchApp(clickedApp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void LaunchApp(ApplicationViewModel clickedApp)
        {
            if ((clickedApp == null) || (String.IsNullOrEmpty(clickedApp.Application.UrlString)))
            {
                MessageBox.Show("Application developer didn't set application's URL");
            }
            else
            {
                ServiceLocator.BrowserWindowsCommunicator.DisplayApplication(clickedApp.Application);
            }
        }

        public static ApplicationViewModel GetApplicationViewModelFromContextMenuClick(object sender)
        {
            var clickedApp = ((MenuItem)sender).DataContext as ApplicationViewModel;
            return clickedApp;
        }

        /// <summary>
        /// MUST BE WRAPPED IN TRY-CATCH Throws exceptions for network errors or API Errors
        /// </summary>
        /// <returns></returns>
        public static bool Authenticate()
        {
            var localStorage = ServiceLocator.LocalStorage;

            lock (localStorage.Locker)
            {
                if (localStorage.HasCredentials)
                {
                    if (ServiceLocator.CachedAppDirectApi.Authenticate(localStorage.LoginInfo.Username,
                                                                       localStorage.LoginInfo.Password))
                    {
                        ServiceLocator.BrowserWindowsCommunicator.UpdateSession(ServiceLocator.CachedAppDirectApi.Session);

                        return true;
                    }

                    localStorage.ClearLoginCredentials();
                }
            }

            return false;
        }

        public static void PerformInUiThread(Action action)
        {
            UiHelper.PerformInUiThread(action);
        }

        public static void PerformForMinimumTime(Action action, bool requiresUiThread, int minimumMillisecondsBeforeReturn)
        {
            UiHelper.PerformForMinimumTime(action, requiresUiThread, minimumMillisecondsBeforeReturn);
        }

        public static bool PerformWhenIdle(Action action, TimeSpan idleTimeRequired, TimeSpan intervalBetweenIdleCheck, TimeSpan timeout)
        {
            var startTicks = Environment.TickCount;

            while (Environment.TickCount - startTicks < timeout.TotalMilliseconds)
            {
                var idleSeconds = GetIdleSeconds();
                if (idleSeconds > idleTimeRequired.TotalSeconds)
                {
                    try
                    {
                        action.Invoke();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Log.ErrorException("Failed to invoke action", e);
                        break;
                    }
                }

                Thread.Sleep(intervalBetweenIdleCheck);
            }

            return false;
        }

        private static int GetIdleSeconds()
        {
            int idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            int envTicks = Environment.TickCount;

            if (User32Dll.GetLastInputInfo(ref lastInputInfo))
            {
                uint lastInputTick = lastInputInfo.dwTime;
                idleTime = envTicks - (int)lastInputTick;
            }

            return ((idleTime > 0) ? (idleTime / 1000) : 0);
        }

        public static string AddApplication(string applicationId)
        {
            if (ServiceLocator.LocalStorage.UserInfo == null)
            {
                GetUserInfo();
            }

            var freeSubscriptionPlanId =
                ServiceLocator.CachedAppDirectApi.GetFreeSubscriptionPlanId(applicationId);
            return ServiceLocator.CachedAppDirectApi.ProvisionApplication(ServiceLocator.LocalStorage.UserInfo.UserId, ServiceLocator.LocalStorage.UserInfo.CompanyId, freeSubscriptionPlanId);
        }

        public static bool RemoveApplication(IApplication application)
        {
            return ServiceLocator.CachedAppDirectApi.DeprovisionApplication(application.SubscriptionId);
        }

        public static void GetUserInfo()
        {
            ServiceLocator.LocalStorage.UserInfo = ServiceLocator.CachedAppDirectApi.UserInfo;
        }
    }
}