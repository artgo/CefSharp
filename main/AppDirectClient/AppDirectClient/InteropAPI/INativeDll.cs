using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppDirect.WindowsClient.InteropAPI
{
    public interface INativeDll
    {
        bool IsValid();
        bool SetupSubclass(IntPtr hwndAdButton);
        bool TearDownSubclass();
        bool IsSubclassed();
    }
}
