using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Analytics;
using AppDirect.WindowsClient.Common;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;
using AppDirect.WindowsClient.Storage;
using AppDirect.WindowsClient.Updates;
using Ninject;
using AppDirect.WindowsClient.InteropAPI;
using AppDirect.WindowsClient.InteropAPI.Internal;
using Ninject.Parameters;

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

        public static IProcessWatcher BrowserWatcher
        {
            get { return Kernel.Get<IProcessWatcher>(); }
        }

        public static IUiHelper UiHelper
        {
            get { return Kernel.Get<IUiHelper>(); }
        }

        public static IAnalytics Analytics
        {
            get { return Kernel.Get<IAnalytics>(); }
        }

        public static ITaskbarApi TaskbarApi
        {
            get { return Kernel.Get<ITaskbarApi>(); }
        }

        public static ITaskbarHelper GetTaskbarHelper()
        {
            return Kernel.Get<ITaskbarHelper>();
        }

        public static INativeDll GetNativeDll(string path)
        {
            return Kernel.Get<INativeDll>(new ConstructorArgument("path", path));
        }

        /// <summary>
        /// Initializes Apis, Loads Local Storage, etc
        /// </summary>
        public static void Initialize()
        {
            Kernel.Rebind<IProcessWatcher>().ToConstant(new ProcessWatcher("BrowserManager", new AbstractProcess("BrowserManager"), new NLogLogger("Browser Process Watcher")));
            Kernel.Rebind<IUiHelper>().ToConstant(new UiHelper(new NLogLogger("UiHelper")));
            Kernel.Rebind<IAnalytics>().ToConstant(new AsyncAnalytics(new GoogleAnalytics(new NLogLogger("Analytics")), Kernel.Get<IUiHelper>()));
            Kernel.Rebind<ICachedAppDirectApi>().ToConstant(new CachedAppDirectApi(new AppDirectApi(), new NLogLogger("CachedAppDirectApi")));
            Kernel.Rebind<LocalStorage>().ToConstant(new LocalStorage());
            Kernel.Rebind<IBrowserWindowsCommunicator>().ToConstant(new BrowserWindowsCommunicator(new BrowsersManagerApiServiceBuilder(), Kernel.Get<IUiHelper>(), new NLogLogger("BrowserWindowsCommunicator")));
            Kernel.Rebind<IIpcCommunicator>().ToConstant(new IpcCommunicator(new MainApplication(Kernel.Get<LocalStorage>(), Kernel.Get<IBrowserWindowsCommunicator>()), BrowserWatcher));
            Kernel.Rebind<Updater>().ToConstant(new Updater());
            Kernel.Rebind<ITaskbarApi>().ToConstant(new TaskBarApi());
            Kernel.Rebind<ITaskbarHelper>().To<TaskbarHelper>();
            Kernel.Rebind<INativeDll>().To<NativeDll>();
        }
    }
}