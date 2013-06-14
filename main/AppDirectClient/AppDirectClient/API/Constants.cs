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
        public const string BrowserProject = "BrowserManager";
        public const string ExeExt = ".exe";
        public static readonly Regex EmailMatchPattern = new Regex(@"^([0-9a-zA-Z]([-\.\w\+]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
        public static readonly Regex PasswordMatchPattern = new Regex(@"^(.{" + MinimumPasswordLength + "," + MaximumPasswordLength + "})$");
        public const int DefaultBrowserWidth = 1000;
        public const int DefaultBrowserHeight = 581;
        public const bool DefaultBrowserResizable = true;
        public static readonly string BaseAnalyticsDomainName = Properties.Resources.BaseAnalyticsDomainName;
        public static readonly string GaCategory = Properties.Resources.GACategory;
        public static readonly string BaseAppStoreDomainName = Properties.Resources.BaseAppStoreUrl;
        public static readonly string BaseAppStoreUrl = Properties.Resources.BaseUrlProtocol + BaseAppStoreDomainName;
        public const int LoginUiTimeout = 4900;
    }
}