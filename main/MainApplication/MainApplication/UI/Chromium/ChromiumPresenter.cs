using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using CefSharp;

namespace AppDirect.WindowsClient.UI.Chromium
{
    public class ChromiumPresenter : IRequestHandler, ICookieVisitor
    {
        public static void Init()
        {
            var settings = new Settings();
            if (CEF.Initialize(settings))
            {
                CEF.RegisterScheme("test", new SchemeHandlerFactory());
                CEF.RegisterJsObject("bound", new BoundObject());
            }
        }

        private readonly IWebBrowser _model;
        private readonly IChromiumView _view;
        private readonly Action<Action> _guiInvoke;

        public ChromiumPresenter(IWebBrowser model, IChromiumView view,
            Action<Action> guiInvoke)
        {
            _model = model;
            _view = view;
            _guiInvoke = guiInvoke;

            var version = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}",
                CEF.ChromiumVersion, CEF.CefVersion, CEF.CefSharpVersion);
            view.DisplayOutput(version);

            model.RequestHandler = this;
            model.PropertyChanged += model_PropertyChanged;
            model.ConsoleMessage += model_ConsoleMessage;

            // file
            view.ShowDevToolsActivated += view_ShowDevToolsActivated;
            view.CloseDevToolsActivated += view_CloseDevToolsActivated;
            view.ExitActivated += view_ExitActivated;

            // edit
            view.UndoActivated += view_UndoActivated;
            view.RedoActivated += view_RedoActivated;
            view.CutActivated += view_CutActivated;
            view.CopyActivated += view_CopyActivated;
            view.PasteActivated += view_PasteActivated;
            view.DeleteActivated += view_DeleteActivated;
            view.SelectAllActivated +=  view_SelectAllActivated;

            // navigation
            view.UrlActivated += view_UrlActivated;
            view.ForwardActivated += view_ForwardActivated;
            view.BackActivated += view_BackActivated;
        }

        private void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string @string = null;
            bool @bool = false;

            switch (e.PropertyName)
            {
                case "IsBrowserInitialized":
                    if (_model.IsBrowserInitialized)
                    {
                        foreach (Cookie c in _view.Session.Cookies)
                        {
                            CEF.SetCookie(@"https://" + c.Domain + c.Path, c.Domain, c.Name, c.Value, c.Path, c.Secure, c.HttpOnly, c.Expired, c.Expires);
                        }

                        _model.Load(_view.UrlAddress);
                    }
                    break;
                case "Title":
                    @string = _model.Title;
                    _guiInvoke(() => _view.SetTitle(@string));
                    break;
                case "Address":
                    @string = _model.Address;
                    _guiInvoke(() => _view.UrlAddress  = @string);
                    break;
                case "CanGoBack":
                    @bool = _model.CanGoBack;
                    _guiInvoke(() => _view.SetCanGoBack(@bool));
                    break;
                case "CanGoForward":
                    @bool = _model.CanGoForward;
                    _guiInvoke(() => _view.SetCanGoForward(@bool));
                    break;
                case "IsLoading":
                    @bool = _model.IsLoading;
                    _guiInvoke(() => _view.SetIsLoading(@bool));
                    break; 
            }
        }

        private void model_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            _guiInvoke(() => _view.DisplayOutput(e.Message));
        }

        private void view_ShowDevToolsActivated(object sender, EventArgs e)
        {
            _model.ShowDevTools();
        }

        private void view_CloseDevToolsActivated(object sender, EventArgs e)
        {
            _model.CloseDevTools();
        }

        private void view_ExitActivated(object sender, EventArgs e)
        {
            _model.Dispose();
            CEF.Shutdown();
            System.Environment.Exit(0);
        }

        void view_UndoActivated(object sender, EventArgs e)
        {
            _model.Undo();
        }

        void view_RedoActivated(object sender, EventArgs e)
        {
            _model.Redo();
        }

        void view_CutActivated(object sender, EventArgs e)
        {
            _model.Cut();
        }

        void view_CopyActivated(object sender, EventArgs e)
        {
            _model.Copy();
        }

        void view_PasteActivated(object sender, EventArgs e)
        {
            _model.Paste();
        }

        void view_DeleteActivated(object sender, EventArgs e)
        {
            _model.Delete();
        }

        void view_SelectAllActivated(object sender, EventArgs e)
        {
            _model.SelectAll();
        }

        private void view_UrlActivated(object sender, string url)
        {
            _model.Load(url);
        }

        private void view_BackActivated(object sender, EventArgs e)
        {
            _model.Back();
        }

        private void view_ForwardActivated(object sender, EventArgs e)
        {
            _model.Forward();
        }

        #region IRequestHandler Members

        bool IRequestHandler.OnBeforeBrowse(IWebBrowser browser, IRequest request, NavigationType naigationvType, bool isRedirect)
        {
            return false;
        }

        bool IRequestHandler.OnBeforeResourceLoad(IWebBrowser browser, IRequestResponse requestResponse)
        {
 
            return false;
        }

        void IRequestHandler.OnResourceResponse(IWebBrowser browser, string url, int status, string statusText, string mimeType, WebHeaderCollection headers)
        {

        }

        #endregion

        #region ICookieVisitor Members

        bool ICookieVisitor.Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            cookie.Expires = new DateTime(2100, 01, 01);
            cookie.Expired = false;

            return true;
        }

        #endregion
    }
}