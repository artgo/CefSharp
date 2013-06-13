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
            _log.Info("Starting service " + typeof(T).FullName);
            _failedState = true;
            var service = _serviceStarter.CreateServiceAndTryToConnect();
            if (!(service is ICommunicationObject))
            {
                throw new InvalidOperationException("Service is not an instance of ICommunicationObject");
            }

            _communicationObject = (ICommunicationObject)service;

            _failedState = false;
            _log.Info("Service " + typeof(T).FullName + " was started.");
        }

        public void Stop()
        {
            if (_communicationObject == null)
            {
                throw new InvalidOperationException("Service was not started");
            }

            _log.Info("Stopping service " + typeof(T).FullName);

            try
            {
                if (_communicationObject.State == CommunicationState.Opened)
                {
                    _communicationObject.Close();
                }
            }
            catch (Exception e)
            {
                _log.Info(_connectionErrorStr + " Error while closing service " + typeof(T).FullName + " "+ e.Message);
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
                _failedState = true;
                _log.Info(_connectionErrorStr + " " + e.Message);

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

            var methodName = _uiHelper.GetPrevLocation(2);
            _log.Debug("Executing " + methodName);

            RetryStartUntilReady();

            bool failed = true;

            while (failed)
            {
                try
                {
                    action.Invoke();
                    failed = false;
                    _failedState = false;
                }
                catch (CommunicationException e)
                {
                    _failedState = true;
                    _log.Info(_connectionErrorStr + " while calling " + methodName + " error: " + e.Message);
                }

                if (failed)
                {
                    _uiHelper.Sleep(RestartIntervalMilliseconds);
                    TryToStart();
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

            while (true)
            {
                try
                {
                    var result =  action.Invoke();
                    _failedState = false;
                    return result;
                }
                catch (CommunicationException e)
                {
                    _failedState = true;
                    var methodName = _uiHelper.GetPrevLocation(2);
                    _log.Info(_connectionErrorStr + " while calling " + methodName + " error: " + e.Message);
                }

                _uiHelper.Sleep(RestartIntervalMilliseconds);
                TryToStart();
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
    }
}