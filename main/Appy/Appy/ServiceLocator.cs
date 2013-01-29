using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Storage;
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

        public static void Initialize()
        {
            Kernel = new StandardKernel();
            Kernel.Bind<IAppDirectApi>().ToConstant(new AppDirectApi());
            Kernel.Bind<ICachedAppDirectApi>().ToConstant(new CachedAppDirectApi(Kernel.Get<IAppDirectApi>()));
            Kernel.Bind<LocalStorage>().ToConstant(new LocalStorage(true));
            Kernel.Bind<BrowserWindowsCommunicator>().ToConstant(new BrowserWindowsCommunicator());
        }
    }
}
