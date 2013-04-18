namespace AppDirect.WindowsClient.InteropAPI
{
    public interface ITaskbarInteropCallback
    {
        /// <summary>
        /// We want to change width of deskband
        /// </summary>
        /// <param name="newWidth">new witdth to be set</param>
        /// <returns>Returns true if able to process</returns>
        bool ChangeWidth(int newWidth);
    }
}