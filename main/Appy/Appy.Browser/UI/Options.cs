using CommandLine;

namespace AppDirect.WindowsClient.Browser
{
    public class Options
    {
        [Option(null, "appid", DefaultValue = null, HelpText = "Application ID")]
        public string AppId { get; set; }
    }
}