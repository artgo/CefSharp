using AppDirect.WindowsClient.InteropAPI.Internal;
using AppDirect.WindowsClient.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppDirect.WindowsClient.InteropAPI
{
    public enum TaskbarIconsSize
    {
        Large = IconsSize.LARGE,
        Small = IconsSize.SMALL
    }

    public enum TaskbarPosition : uint
    {
        Bottom = TaskbarPlacement.ABE_BOTTOM,
        Left = TaskbarPlacement.ABE_LEFT,
        Top = TaskbarPlacement.ABE_TOP,
        Right = TaskbarPlacement.ABE_RIGHT
    }

    public static class TaskbarPositionExtensions
    {
        public static bool IsVertical(this TaskbarPosition position)
        {
            return (position == TaskbarPosition.Left) || (position == TaskbarPosition.Right);
        }
    }

    public interface ITaskBarControl
    {
        void SetAllowedSize(int allowedWidth, int allowedHeight);
    }

    public interface ITaskBarHost
    {
        void SetDesiredOffset(int offset);
    }

    public interface ITaskBarApi
    {
        void InsertTaskbarWindow(TaskbarPanel taskbarPanel);

        void RemoveTaskbarWindow();
    }
}
