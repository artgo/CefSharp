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

        public static IBrowserWindowsCommunicator BrowserWindowsCommunicator
        {
            get { return Kernel.Get<IBrowserWindowsCommunicator>(); }
        }

        public static IIpcCommunicator IpcCommunicator
        {
            get { return Kernel.Get<IIpcCommunicator>(); }
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
            Kernel.Rebind<IIpcCommunicator>().ToConstant(new IpcCommunicator());
            Kernel.Rebind<IBrowserWindowsCommunicator>().ToConstant(new BrowserWindowsCommunicator(Kernel.Get<IIpcCommunicator>()));
            Kernel.Rebind<Updater>().ToConstant(new Updater());
        }
    }
}
