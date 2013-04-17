using System.Threading;

namespace AppDirect.WindowsClient.API
{
    public class Latch : ILatch
    {
        private readonly EventWaitHandle _eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        public bool Wait()
        {
            return _eventWaitHandle.WaitOne();
        }

        public bool Unlock()
        {
            return _eventWaitHandle.Set();
        }
    }
}