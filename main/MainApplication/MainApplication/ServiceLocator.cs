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
        private ServiceLocator() {}

        public static ICachedAppDirectApi CachedAppDirectApi
        {
            get { return App.Kernel.Get<ICachedAppDirectApi>(); }
        } 
    }
}
