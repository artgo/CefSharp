using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;
using System;
using System.Threading;

namespace AppDirect.WindowsClient.Common.API
{
    public class Pinger : IPinger
    {
        private const int MaxFailAttempts = 3;
        private const int PingIntervalMs = 3000;
        private const int SleepAfterRestartMs = 5000;

        private readonly ILogger _logger;
        private readonly IUiHelper _uiHelper;
        private readonly IPingable _pingable;
        private readonly IProcessRestarter _processRestarter;
        private readonly Action _mainAction;
        private volatile Thread _mainThread;

        public Pinger(IUiHelper uiHelper, ILogger logger, IPingable pingable, IProcessRestarter processRestarter)
        {
            _uiHelper = uiHelper;
            _logger = logger;
            _pingable = pingable;
            _processRestarter = processRestarter;
            _mainAction = DoPings;
            _mainThread = null;
        }

        private void DoPings()
        {
            var failCounter = 0;

            while (true)
            {
                failCounter = TestPing(failCounter);
            }
        }

        public int TestPing(int failCounter)
        {
            _uiHelper.Sleep(PingIntervalMs);

            var failed = false;

            try
            {
                var randomValue = _uiHelper.GetCurrentMilliseconds();
                var result = _pingable.Ping(randomValue);
                failed = result != (randomValue + 1);
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (ThreadInterruptedException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.Info("Exception while pinging another process " + e.Message);
                failed = true;
            }

            if (failed)
            {
                failCounter++;
            }

            if (failCounter >= MaxFailAttempts)
            {
                failCounter = 0;
                _processRestarter.RestartProcess();
                _uiHelper.Sleep(SleepAfterRestartMs);
            }

            return failCounter;
        }

        public void Start()
        {
            if (_mainThread != null)
            {
                throw new InvalidOperationException("Can't start service which has already been started");
            }

            _mainThread = _uiHelper.StartAsynchronously(_mainAction);
        }

        public void Stop()
        {
            if (_mainThread == null)
            {
                throw new InvalidOperationException("Can't stop service which has not been started");
            }

            try
            {
                _mainThread.Abort();
                _mainThread = null;
            }
            catch (Exception e)
            {
                _logger.ErrorException("Exception while stopping thread", e);
            }
        }
    }
}