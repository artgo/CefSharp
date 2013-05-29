using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class MessageParams
    {
        public IntPtr LParam { get; set; }
        public IntPtr WParam { get; set; }
    }
}
