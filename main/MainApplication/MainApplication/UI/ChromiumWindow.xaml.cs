﻿﻿using System;
﻿using System.Collections.Generic;
﻿using System.Windows;
﻿using System.Windows.Input;
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
    }
}
