using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;

namespace AppDirect.WindowsClient.Common.API
{
    public class Pinger : IPinger
    {
        private const int PingIntervalMs = 5000;
        private readonly ILogger _logger;
        private readonly IUiHelper _uiHelper;

        public Pinger(IUiHelper uiHelper, ILogger logger)
        {
            _logger = logger;
            _uiHelper = uiHelper;
        }

        private void DoPings()
        {
            while (true)
            {
                _uiHelper.Sleep(PingIntervalMs);

            }
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}