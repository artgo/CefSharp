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


        public static void RetryAction(Action action, int numRetries, TimeSpan retryInterval)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            do
            {
                try { action(); return; }
                catch
                {
                    if (numRetries < 0)
                    {
                        throw;
                    }

                    Thread.Sleep(retryInterval);
                }

            } while (numRetries-- > 0);
        }
    }
}
