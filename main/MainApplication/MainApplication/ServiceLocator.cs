using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static void Initialize()
        {
            Kernel = new StandardKernel();
            Kernel.Bind<IAppDirectApi>().To<AppDirectApi>();
            Kernel.Bind<ICachedAppDirectApi>().To<CachedAppDirectApi>();
        }
    }
}
