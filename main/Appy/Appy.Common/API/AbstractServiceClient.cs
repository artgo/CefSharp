using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.Common.UI;
using System;
using System.ServiceModel;

namespace AppDirect.WindowsClient.Common.API
{
    /// <summary>
    /// Abstract implementation of any service client implementing IStartStop interface
    /// and having TryToStart() to try to initialize communication. Will try to re-create
    /// communication object and therefore session on failure.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AbstractServiceClient<T> : IAbstractServiceClient<T> where T : class
    {
        private const int RestartIntervalMilliseconds = 100;
        private readonly string _connectionErrorStr = "Error while trying to connect to the service " + typeof(T).FullName;
        private readonly IServiceBuilder<T> _serviceStarter;
        private readonly IUiHelper _uiHelper;
        private readonly ILogger _log;
        private volatile ICommunicationObject _communicationObject;
        private volatile bool _failedState = true;

        public AbstractServiceClient(IServiceBuilder<T> serviceStarter, IUiHelper uiHelper, ILogger log)
        {
            if (serviceStarter == null)
            {
                throw new ArgumentNullException("serviceStarter");
            }

            if (uiHelper == null)
            {
                throw new ArgumentNullException("uiHelper");
            }

            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            _serviceStarter = serviceStarter;
            _uiHelper = uiHelper;
            _log = log;
        }

        public T Service
        {
            get { return _communicationObject as T; }
        }

        public void Start()
        {
            _failedState = true;
            var service = _serviceStarter.CreateServiceAndTryToConnect();
            if (!(service is ICommunicationObject))
            {
                throw new InvalidOperationException("Service is not an instance of ICommunicationObject");
            }

            _communicationObject = (ICommunicationObject)service;
            _failedState = false;
        }

        public void Stop()
        {
            if (_communicationObject == null)
            {
                throw new InvalidOperationException("Service was not started");
            }

            try
            {
                if (_communicationObject.State == CommunicationState.Opened)
                {
                    _communicationObject.Close();
                }
            }
            catch (Exception e)
            {
                _log.ErrorException("Error while closing service " + typeof(T).FullName, e);
            }
            finally
            {
                _failedState = true;
                _communicationObject = null;
            }
        }

        public bool TryToStart()
        {
            var started = true;
            try
            {
                Start();
            }
            catch (CommunicationException e)
            {
                LogCommunicationError(e);

                started = false;
            }

            return started;
        }

        public void StartIfNotStarted()
        {
            RetryStartUntilReady();
        }

        public virtual void MakeSureExecuteAction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            RetryStartUntilReady();

            try
            {
                action.Invoke();
            }
            catch (CommunicationException e)
            {
                LogCommunicationError(e);

                // Restart the communicator and retry
                try
                {
                    Start();
                    action.Invoke();
                }
                catch (CommunicationException e2)
                {
                    LogCommunicationError(e2);
                    throw;
                }
            }
        }

        public virtual TR MakeSureExecuteAction<TR>(Func<TR> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            RetryStartUntilReady();

            try
            {
                var result =  action.Invoke();
                return result;
            }
            catch (CommunicationException e)
            {
                LogCommunicationError(e);

                // Restart the communicator and retry
                try
                {
                    Start();
                    var result = action.Invoke();
                    return result;
                }
                catch (CommunicationException e2)
                {
                    LogCommunicationError(e2);
                    throw;
                }
            }
        }

        private void RetryStartUntilReady()
        {
            while ((_communicationObject == null) || _failedState)
            {
                if (!TryToStart())
                {
                    _uiHelper.Sleep(RestartIntervalMilliseconds);
                }
            }
        }

        protected virtual void LogCommunicationError(CommunicationException e)
        {
            _failedState = true;
            _log.ErrorException(_connectionErrorStr, e);
        }
    }
}