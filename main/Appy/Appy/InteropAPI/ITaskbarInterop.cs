namespace AppDirect.WindowsClient.InteropAPI
{
    public interface ITaskbarInterop
    {
        /// <summary>
        /// Should be called when user changes height of taskbar or choose small icons
        /// </summary>
        /// <param name="newHeight"></param>
        void HeightChanged(int newHeight);

        /// <summary>
        /// Should be called when user changes newPosition of taskbar on the screen
        /// </summary>
        /// <param name="newPosition"></param>
        void PositionChanged(TaskbarPosition newPosition);

        /// <summary>
        /// Sets interface for callback events (we want to expand deskband)
        /// Must be set right away on to support callback events
        /// </summary>
        ITaskbarInteropCallback TaskbarCallbackEvents { get; set; }
    }
}