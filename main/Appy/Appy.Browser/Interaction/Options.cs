using CommandLine;

namespace AppDirect.WindowsClient.Browser.Interaction
{
    public class Options
    {
        [Option(null, "appid", DefaultValue = null, HelpText = "Application ID")]
        public string AppId { get; set; }
    }
}