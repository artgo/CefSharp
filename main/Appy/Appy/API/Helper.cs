using System.Reflection;

namespace AppDirect.WindowsClient.API
{
    public class Helper
    {
        private Helper() {}

        public static readonly AssemblyName AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
        public static readonly string ApplicationName = AssemblyName.Name;
        public static readonly string ApplicationVersion = AssemblyName.Version.ToString();
    }
}
