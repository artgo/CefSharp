using System;
using System.Reflection;
using System.Threading;

namespace AppDirect.WindowsClient.API
{
    public class Helper
    {
        private Helper() {}

        public static readonly AssemblyName AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
        public static readonly string ApplicationName = AssemblyName.Name;
        public static readonly string ApplicationVersion = AssemblyName.Version.ToString();
        public static readonly string ApplicationDirectory = @"\AppDirect\" + ApplicationName;
        public static readonly string BrowserProjectExt = ".Browser";
        public static readonly string ExeExt = ".exe";


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
    }
}
