using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Application = AppDirect.WindowsClient.Common.API.Application;

namespace AppDirect.WindowsClient.API
{
    public class Helper
    {
        private Helper() {}

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

        public static Application GetApplicationFromButtonSender(object sender)
        {
            return ((Button)sender).DataContext as Application;
        }

        public static void AppButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var clickedApp = GetApplicationFromButtonSender(sender);

                if ((clickedApp == null) || (String.IsNullOrEmpty(clickedApp.UrlString)))
                {
                    MessageBox.Show("Application developer didn't set application's URL");
                }
                else
                {
                    ServiceLocator.BrowserWindowsCommunicator.OpenApp(clickedApp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static Application GetClickedAppFromContextMenuClick(object sender)
        {
            var clickedApp = ((MenuItem) sender).DataContext as Application;
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
                    if (ServiceLocator.CachedAppDirectApi.IsAuthenticated)
                    {
                        return true;
                    }
                    if (ServiceLocator.CachedAppDirectApi.Authenticate(localStorage.LoginInfo.Username,
                                                                       localStorage.LoginInfo.Password))
                    {
                        return true;
                    }

                    localStorage.ClearLoginCredentials();
                }
            }

            return false;
        }

        public static void PerformInUiThread(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var currentApplication = System.Windows.Application.Current;
            if ((currentApplication == null) || (Thread.CurrentThread == currentApplication.Dispatcher.Thread))
            {
                action.Invoke();
            }
            else
            {
                currentApplication.Dispatcher.Invoke(action);
            }
        }
    }
}
