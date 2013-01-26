using System;
using AppDirect.WindowsClient.Common.API;

namespace AppDirect.WindowsClient.BrowserWindow.UI.Chromium
{
    public interface IChromiumView
    {
        // file
        event EventHandler ShowDevToolsActivated;
        event EventHandler CloseDevToolsActivated;
        event EventHandler ExitActivated;

        // edit
        event EventHandler UndoActivated;
        event EventHandler RedoActivated;
        event EventHandler CutActivated;
        event EventHandler CopyActivated;
        event EventHandler PasteActivated;
        event EventHandler DeleteActivated;
        event EventHandler SelectAllActivated;

        // navigation
        event Action<string> UrlActivated;
        event EventHandler BackActivated;
        event EventHandler ForwardActivated;

        string UrlAddress { get; set; }
        IAppDirectSession Session { get; set; }

        void SetTitle(string title);
        void SetCanGoBack(bool canGoBack);
        void SetCanGoForward(bool canGoForward);
        void SetIsLoading(bool isLoading);

        void ExecuteScript(string script);
        object EvaluateScript(string script);
        void DisplayOutput(string output);
    }
}