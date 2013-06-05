using AppDirect.WindowsClient.InteropAPI.Internal;
using AppDirect.WindowsClient.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppDirect.WindowsClient.InteropAPI
{
    public interface ITaskbarApi
    {
        void InsertPanelWhenTaskbarIsAvailable(TaskbarPanel taskbarPanel);

        void InsertPanel(TaskbarPanel taskbarPanel);

        void RemovePanel();
    }
}
