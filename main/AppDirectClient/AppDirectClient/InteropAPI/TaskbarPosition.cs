using AppDirect.WindowsClient.InteropAPI.Internal;

namespace AppDirect.WindowsClient.InteropAPI
{
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
}