using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;

namespace AppDirect.WindowsClient.Common.API
{
    public class Pinger : IPinger
    {
        private const int PingIntervalMs = 5000;
        private readonly ILogger _logger;
        private readonly IUiHelper _uiHelper;
        private readonly IPingable _pingable;
        private readonly IProcessRestarter _processRestarter;

        public Pinger(IUiHelper uiHelper, ILogger logger, IPingable pingable, IProcessRestarter processRestarter)
        {
            _logger = logger;
            _uiHelper = uiHelper;
            _pingable = pingable;
            _processRestarter = processRestarter;
        }

        private void DoPings()
        {
            while (true)
            {
                _uiHelper.Sleep(PingIntervalMs);
                var randomValue = _uiHelper.GetCurrentMilliseconds();
                var result = _pingable.Ping(randomValue);
                var failed = result != (randomValue + 1);
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