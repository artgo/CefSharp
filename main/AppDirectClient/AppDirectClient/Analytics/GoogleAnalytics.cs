using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.Log;
using GaDotNet.Common;
using GaDotNet.Common.Data;
using GaDotNet.Common.Helpers;
using System;

namespace AppDirect.WindowsClient.Analytics
{
    public class GoogleAnalytics : IAnalytics
    {
        private readonly RequestFactory _requestFactory = new RequestFactory();
        private readonly ILogger _log;

        public GoogleAnalytics(ILogger log)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            _log = log;
        }

        public void Notify(string action, string label, int? value)
        {
            try
            {
                var googleEvent = new GoogleEvent(Helper.BaseAnalyticsDomainName, Helper.GaCategory, action, label, value);

                _log.Debug("Building event " + googleEvent);
                var request = _requestFactory.BuildRequest(googleEvent);

                _log.Debug("Firing event " + googleEvent);
                GoogleTracking.FireTrackingEvent(request);
                _log.Debug("Event " + googleEvent + " Fired");
            }
            catch (Exception e)
            {
                _log.ErrorException("Error while trying to perform analytics event", e);
            }
        }

        public void NotifySimpleAction(string action)
        {
            Notify(action, action, 0);
        }
    }
}