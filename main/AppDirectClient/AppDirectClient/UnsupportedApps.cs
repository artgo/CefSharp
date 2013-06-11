using System.Collections.Generic;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient
{
    public class UnsupportedApps
    {
        private static readonly HashSet<string> UnsupportedAppNames = new HashSet<string>(new string[] { "iMeet", "Galaxie" });

        public static bool IsUnsupported(IApplication application)
        {
            return UnsupportedAppNames.Contains(application.Name);
        }
    }
}