using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.Browser.API
{
    public class IpcMainWindowStarter : AbstractServiceRunner<IBrowsersManagerApi>
    {
        public IStartStop Watcher;

        public IpcMainWindowStarter(IBrowsersManagerApi service)
            : base(service)
        {
        }

        public void Start(IStartStop watcher)
        {
            Watcher = watcher;
            Watcher.Start();
            base.Start();
        }

        public override void Stop()
        {
            Watcher.Stop();
            base.Stop();
        }
    }
}