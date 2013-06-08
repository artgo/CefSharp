using System.Reflection;
using System.Text.RegularExpressions;

namespace AppDirect.WindowsClient.API
{
    public abstract class Constants
    {
        private const int MinimumPasswordLength = 4;
        private const int MaximumPasswordLength = 18;
        public static readonly AssemblyName ThisAssemblyName = Assembly.GetExecutingAssembly().GetName();
        public static readonly string ApplicationName = ThisAssemblyName.Name;
        public static readonly string ApplicationVersion = ThisAssemblyName.Version.ToString();
        public static readonly string ApplicationDirectory = @"\AppDirect\" + ApplicationName;
        public static readonly string BrowserProject = "BrowserManager";
        public static readonly string ExeExt = ".exe";
        public static readonly Regex EmailMatchPattern = new Regex(@"^([0-9a-zA-Z]([-\.\w\+]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
        public static readonly Regex PasswordMatchPattern = new Regex(@"^(.{" + MinimumPasswordLength + "," + MaximumPasswordLength + "})$");
        public static readonly int DefaultBrowserWidth = 1000;
        public static readonly int DefaultBrowserHeight = 581;
        public static readonly bool DefaultBrowserResizable = true;
        public static readonly string BaseAnalyticsDomainName = Properties.Resources.BaseAnalyticsDomainName;
        public static readonly string GaCategory = Properties.Resources.GACategory;
        public static readonly string BaseAppStoreDomainName = Properties.Resources.BaseAppStoreUrl;
        public static readonly string BaseAppStoreUrl = Properties.Resources.BaseUrlProtocol + BaseAppStoreDomainName;
        public const int LoginUiTimeout = 4900;
    }
}