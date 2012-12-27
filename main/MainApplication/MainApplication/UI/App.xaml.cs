using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using Ninject;

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IKernel Kernel { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Kernel = new StandardKernel();
            Kernel.Bind<IAppDirectApi>().To<AppDirectApi>();
            Kernel.Bind<ICachedAppDirectApi>().To<CachedAppDirectApi>();
        }
    }
}
