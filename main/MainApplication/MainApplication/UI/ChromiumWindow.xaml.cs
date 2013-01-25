﻿using System;
﻿using System.Collections.Generic;
﻿using System.ComponentModel;
﻿using System.IO;
﻿using System.Windows;
﻿using System.Windows.Controls;
﻿using System.Windows.Input;
﻿using System.Windows.Media;
﻿using System.Windows.Media.Imaging;
﻿using AppDirect.WindowsClient.API;
﻿using AppDirect.WindowsClient.UI.Chromium;


namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for ChromiumWindow.xaml
    /// </summary>
    public partial class ChromiumWindow : Window, IChromiumView
    {
        // file
        public event EventHandler ShowDevToolsActivated;
        public event EventHandler CloseDevToolsActivated;
        public event EventHandler ExitActivated;
        public event EventHandler CloseWindow;

        // edit
        public event EventHandler UndoActivated;
        public event EventHandler RedoActivated;
        public event EventHandler CutActivated;
        public event EventHandler CopyActivated;
        public event EventHandler PasteActivated;
        public event EventHandler DeleteActivated;
        public event EventHandler SelectAllActivated;

        // navigation
        public event Action<object, string> UrlActivated;
        public event EventHandler BackActivated;
        public event EventHandler ForwardActivated;

        private string _urlAddress;

        public string UrlAddress
        {
            get { return _urlAddress; }
            set
            {
                _urlAddress = value;
                UrlTextBox.Text = _urlAddress;
            }
        }

        public AppDirectSession Session { get; set; }
        public string ApplicationId { get; set; }

        private readonly IDictionary<object, EventHandler> _handlers;

        public ChromiumWindow()
        {
            InitializeComponent();

            new ChromiumPresenter(WebViewObject, this,
                invoke => Dispatcher.BeginInvoke(invoke));

            _handlers = new Dictionary<object, EventHandler>
            {
                // navigation
                { BackButton, BackActivated },
                { ForwardButton, ForwardActivated },
            };
        }


        public void SetTitle(string title)
        {
            Title = title;
        }


        public void SetCanGoBack(bool canGoBack)
        {
            BackButton.IsEnabled = canGoBack;
        }


        public void SetCanGoForward(bool canGoForward)
        {
            ForwardButton.IsEnabled = canGoForward;
        }


        public void SetIsLoading(bool isLoading)
        {

        }

        public void ExecuteScript(string script)
        {
            WebViewObject.ExecuteScript(script);
        }


        public object EvaluateScript(string script)
        {
            return WebViewObject.EvaluateScript(script);
        }


        public void DisplayOutput(string output)
        {
            OutputLabel.Content = output;
        }

        private void control_Activated(object sender, RoutedEventArgs e)
        {
            EventHandler handler;
            if (_handlers.TryGetValue(sender, out handler) &&
                handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void urlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            var handler = UrlActivated;
            if (handler != null)
            {
                handler(this, UrlTextBox.Text);
            }
        }

        private void Minimize_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            Maximize.Visibility = Visibility.Hidden;
            RestoreDown.Visibility = Visibility.Visible;
        }

        private void RestoreDown_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            RestoreDown.Visibility = Visibility.Hidden;
            Maximize.Visibility = Visibility.Visible;
        }

        public void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void ChromiumWindow_OnClosing(object sender, CancelEventArgs e)
        {
            CloseWindow(this, EventArgs.Empty);
        }
    }
}
