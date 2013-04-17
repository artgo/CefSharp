using System;
using System.Windows;

namespace AppDirect.WindowsClient.Common.API
{
    [Serializable]
    public class ApplicationWithState : IApplicationWithState
    {
        public override IApplication Application { get; set; }

        public override WindowState WindowState { get; set; }
    }
}