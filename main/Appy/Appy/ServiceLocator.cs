using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Storage;
using AppDirect.WindowsClient.UI;
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

        public static void Initialize()
        {
            Kernel = new StandardKernel();
            Kernel.Bind<IAppDirectApi>().ToConstant(new AppDirectApi());
            Kernel.Bind<ICachedAppDirectApi>().ToConstant(new CachedAppDirectApi(Kernel.Get<IAppDirectApi>()));
            Kernel.Bind<LocalStorage>().ToConstant(new LocalStorage(true));
        }
    }
}
