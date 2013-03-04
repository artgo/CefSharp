using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Storage;
using AppDirect.WindowsClient.Updates;
using Ninject;

namespace AppDirect.WindowsClient
{
    public class ServiceLocator
    {
        public static readonly IKernel Kernel = new StandardKernel();

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

        /// <summary>
        /// Initializes Apis, Loads Local Storage, etc
        /// </summary>
        public static void Initialize()
        {
            Kernel.Rebind<IAppDirectApi>().ToConstant(new AppDirectApi());
            Kernel.Rebind<ICachedAppDirectApi>().ToConstant(new CachedAppDirectApi(Kernel.Get<IAppDirectApi>()));
            Kernel.Rebind<LocalStorage>().ToConstant(new LocalStorage());
            Kernel.Rebind<BrowserWindowsCommunicator>().ToConstant(new BrowserWindowsCommunicator());
            Kernel.Rebind<IpcCommunicator>().ToConstant(new IpcCommunicator());
            Kernel.Rebind<Updater>().ToConstant(new Updater());
        }
    }
}
