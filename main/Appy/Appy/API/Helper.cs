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
        private const int RefreshAppsIntervalMins = 55;
        public static TimeSpan RefreshAppsTimeSpan = TimeSpan.FromMinutes(RefreshAppsIntervalMins);

        private const int MinimumPasswordLength = 4;
        private const int MaximumPasswordLength = 18;
        public static readonly Regex EmailMatchPattern = new Regex(@"^([0-9a-zA-Z]([-\.\w\+]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
        public static readonly Regex PasswordMatchPattern = new Regex(@"^(.{" + MinimumPasswordLength + "," + MaximumPasswordLength + "})$");
        
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
    }
}
