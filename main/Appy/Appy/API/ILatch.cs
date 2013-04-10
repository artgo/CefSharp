namespace AppDirect.WindowsClient.API
{
    public interface ILatch
    {
        /// <summary>
        /// Waits until latch is unlocked.  If latch is already locked then it returns immediately.
        /// </summary>
        /// <returns></returns>
        bool Wait();

        /// <summary>
        /// Releases all threads that are currently waiting
        /// </summary>
        /// <returns></returns>
        bool Unlock();
    }
}