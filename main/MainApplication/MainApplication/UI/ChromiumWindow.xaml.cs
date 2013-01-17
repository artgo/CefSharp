﻿using System;
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
    public partial class ChromiumWindow : Window, IExampleView
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


        // test
        public event EventHandler TestResourceLoadActivated;
        public event EventHandler TestSchemeLoadActivated;
        public event EventHandler TestExecuteScriptActivated;
        public event EventHandler TestEvaluateScriptActivated;
        public event EventHandler TestBindActivated;
        public event EventHandler TestConsoleMessageActivated;
        public event EventHandler TestTooltipActivated;
        public event EventHandler TestPopupActivated;
        public event EventHandler TestLoadStringActivated;
        public event EventHandler TestCookieVisitorActivated;


        // navigation
        public event Action<object, string> UrlActivated;
        public event EventHandler BackActivated;
        public event EventHandler ForwardActivated;

        private string _urlAddress;
        private AppDirectSession _session;

        public string UrlAddress
        {
            get { return _urlAddress; }
            set
            {
                _urlAddress = value;
                urlTextBox.Text = _urlAddress;
            }
        }

        public AppDirectSession Session
        {
            get { return _session;  }
            set { _session = value; }
        }

        private IDictionary<object, EventHandler> handlers;

        public ChromiumWindow()
        {
            InitializeComponent();


            var presenter = new ExamplePresenter(web_view, this,
                invoke => Dispatcher.BeginInvoke(invoke));


            handlers = new Dictionary<object, EventHandler>
            {
                // navigation
                { backButton, BackActivated },
                { forwardButton, ForwardActivated },
            };
        }


        public void SetTitle(string title)
        {
            Title = title;
        }


        public void SetCanGoBack(bool can_go_back)
        {
            backButton.IsEnabled = can_go_back;
        }


        public void SetCanGoForward(bool can_go_forward)
        {
            forwardButton.IsEnabled = can_go_forward;
        }


        public void SetIsLoading(bool is_loading)
        {


        }


        public void ExecuteScript(string script)
        {
            web_view.ExecuteScript(script);
        }


        public object EvaluateScript(string script)
        {
            return web_view.EvaluateScript(script);
        }


        public void DisplayOutput(string output)
        {
            outputLabel.Content = output;
        }


        private void control_Activated(object sender, RoutedEventArgs e)
        {
            EventHandler handler;
            if (handlers.TryGetValue(sender, out handler) &&
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
                handler(this, urlTextBox.Text);
            }
        }
    }
}
