using System;
using System.Windows;

namespace AppDirect.WindowsClient.Common.API
{
    [Serializable]
    public class WindowData : IWindowData
    {
        public override string ApplicationId { get; set; }

        public override WindowState WindowState { get; set; }
    }
}