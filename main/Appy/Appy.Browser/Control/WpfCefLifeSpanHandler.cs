﻿using System;
using Xilium.CefGlue;

namespace AppDirect.WindowsClient.Browser.Control
{
    internal sealed class WpfCefLifeSpanHandler : CefLifeSpanHandler
    {
        private readonly WpfCefBrowser _owner;

        public WpfCefLifeSpanHandler(WpfCefBrowser owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            _owner = owner;
        }

        protected override void OnAfterCreated(CefBrowser browser)
        {
            _owner.HandleAfterCreated(browser);
        }
    }
}
