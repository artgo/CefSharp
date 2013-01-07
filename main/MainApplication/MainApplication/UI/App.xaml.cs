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
        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceLocator.Initialize();

            base.OnStartup(e);
        }
    }
}
