﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TaskBarControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        TaskBarIcon _icon;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SampleIconControl control = new SampleIconControl();
            control.InitializeComponent();
            ControlWrapper wrapper = new ControlWrapper(control);
            _icon = new TaskBarIcon(wrapper);
            _icon.Setup();
        }
    }
}
