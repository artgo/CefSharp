using System.IO;
using CefSharp;

namespace AppDirect.WindowsClient.BrowserWindow.UI.Chromium
{
    class SchemeHandlerFactory : ISchemeHandlerFactory
    {
        public ISchemeHandler Create()
        {
            return new SchemeHandler();
        }
    }

    class SchemeHandler: ISchemeHandler
    {
        public bool ProcessRequest(IRequest request, ref string mimeType, ref Stream stream)
        {
            return false;
        }
    }
}
