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
        public static readonly string BrowserProjectExt = ".Browser";
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
        public static readonly IUiHelper UiHelper = new UiHelper();

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
                catch
                {
                    if (catchAction != null)
                    {
                        catchAction();
                    }

                    if (tryAttemptsRemaining <= 1)
                    {
                        throw;
                    }

                    Thread.Sleep(accumulatingTimeSpan);
                    accumulatingTimeSpan += retryInterval;
                }

                tryAttemptsRemaining--;
            } while (tryAttemptsRemaining > 0);
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

                LaunchApp(clickedApp);
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
                    catch (Exception)
                    {
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
    }
}