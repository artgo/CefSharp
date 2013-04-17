using AppDirect.WindowsClient.InteropAPI.Internal;

namespace AppDirect.WindowsClient.InteropAPI
{
    /// <summary>
    /// This API needs to be implemented on UI and main code side to support taskbar buttons communications
    /// </summary>
    public interface ITaskbarInterop
    {
        /// <summary>
        /// Will be called when user changes height of taskbar or choose small icons
        /// </summary>
        /// <param name="newHeight"></param>
        void HeightChanged(int newHeight);

        /// <summary>
        /// Will be called when user changes newPosition of taskbar on the screen
        /// </summary>
        /// <param name="newPosition"></param>
        void PositionChanged(TaskbarPosition newPosition);

        /// <summary>
        /// Will be called when user selects or unselects "small icons" checkbox in taskbar properties
        /// </summary>
        /// <param name="newIconsSize"></param>
        void TaskbarIconsSizeChanged(TaskbarIconsSize newIconsSize);

        /// <summary>
        /// Exception occured
        /// </summary>
        /// <param name="eventArgs">Event details on exception</param>
        void Error(RegistryChangeEventArgs eventArgs);

        /// <summary>
        /// Sets interface for callback events (we want to expand deskband)
        /// Must be set right away on to support callback events
        /// </summary>
        ITaskbarInteropCallback TaskbarCallbackEvents { get; set; }
    }
}