using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Storage;
using AppDirect.WindowsClient.Updates;
using Ninject;

namespace AppDirect.WindowsClient
{
    public class ServiceLocator
    {
        public static IKernel Kernel;

        private ServiceLocator() {}

        public static ICachedAppDirectApi CachedAppDirectApi
        {
            get { return Kernel.Get<ICachedAppDirectApi>(); }
        }

        public static LocalStorage LocalStorage
        {
            get { return Kernel.Get<LocalStorage>(); }
        }

        public static BrowserWindowsCommunicator BrowserWindowsCommunicator
        {
            get { return Kernel.Get<BrowserWindowsCommunicator>(); }
        }

        public static IpcCommunicator IpcCommunicator
        {
            get { return Kernel.Get<IpcCommunicator>(); }
        }

        public static Updater Updater
        {
            get { return Kernel.Get<Updater>(); }
        }

        public static void Initialize()
        {
            Kernel = new StandardKernel();
            Kernel.Bind<IAppDirectApi>().ToConstant(new AppDirectApi());
            Kernel.Bind<ICachedAppDirectApi>().ToConstant(new CachedAppDirectApi(Kernel.Get<IAppDirectApi>()));
            Kernel.Bind<LocalStorage>().ToConstant(new LocalStorage(true));
            Kernel.Bind<BrowserWindowsCommunicator>().ToConstant(new BrowserWindowsCommunicator());
            Kernel.Bind<IpcCommunicator>().ToConstant(new IpcCommunicator()); 
            Kernel.Bind<Updater>().ToConstant(new Updater());
        }
    }
}
