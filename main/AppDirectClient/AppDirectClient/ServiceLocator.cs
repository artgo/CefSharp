using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;
using AppDirect.WindowsClient.Storage;
using AppDirect.WindowsClient.Updates;
using Ninject;

namespace AppDirect.WindowsClient
{
    public class ServiceLocator
    {
        public static readonly IKernel Kernel = new StandardKernel();

        private ServiceLocator()
        {
        }

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

        public static IUiHelper UiHelper
        {
            get { return Kernel.Get<IUiHelper>(); }
        }

        /// <summary>
        /// Initializes Apis, Loads Local Storage, etc
        /// </summary>
        public static void Initialize()
        {
            Kernel.Rebind<IUiHelper>().ToConstant(new UiHelper(new NLogLogger("UiHelper")));
            Kernel.Rebind<IAppDirectApi>().ToConstant(new AppDirectApi());
            Kernel.Rebind<ICachedAppDirectApi>().ToConstant(new CachedAppDirectApi(Kernel.Get<IAppDirectApi>(), new NLogLogger("CachedAppDirectApi")));
            Kernel.Rebind<LocalStorage>().ToConstant(new LocalStorage());
            Kernel.Rebind<IBrowserWindowsCommunicator>().ToConstant(new BrowserWindowsCommunicator(new BrowsersManagerApiServiceBuilder(), Kernel.Get<IUiHelper>(), new NLogLogger("BrowserWindowsCommunicator")));
            Kernel.Rebind<IIpcCommunicator>().ToConstant(new IpcCommunicator(new MainApplication(Kernel.Get<LocalStorage>(), Kernel.Get<IBrowserWindowsCommunicator>())));
            Kernel.Rebind<Updater>().ToConstant(new Updater());
        }
    }
}