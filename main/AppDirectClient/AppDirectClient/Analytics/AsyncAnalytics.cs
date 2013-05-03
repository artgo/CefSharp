using AppDirect.WindowsClient.Common.UI;

namespace AppDirect.WindowsClient.Analytics
{
    public class AsyncAnalytics : IAnalytics
    {
        private readonly IAnalytics _analyticsImpl;
        private readonly IUiHelper _uiHelper;

        public AsyncAnalytics(IAnalytics analyticsImpl, IUiHelper uiHelper)
        {
            _analyticsImpl = analyticsImpl;
            _uiHelper = uiHelper;
        }

        public void Notify(string action, string label, int? value)
        {
            _uiHelper.StartAsynchronously(() => _analyticsImpl.Notify(action, label, value));
        }

        public void NotifySimpleAction(string action)
        {
            _uiHelper.StartAsynchronously(() => _analyticsImpl.NotifySimpleAction(action));
        }
    }
}