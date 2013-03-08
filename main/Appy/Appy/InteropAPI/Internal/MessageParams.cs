using System;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class MessageParams
    {
        public IntPtr LParam { get; set; }
        public IntPtr WParam { get; set; }
    }
}
