using System.Collections.Generic;
using System.Net;

namespace AppDirect.WindowsClient.Browser.Interaction
{
    public interface IBrowserObject
    {
        void Unload();
        void Initialize();
        void SetCookies(IList<Cookie> cookies);
        void DeleteCookies();
    }
}