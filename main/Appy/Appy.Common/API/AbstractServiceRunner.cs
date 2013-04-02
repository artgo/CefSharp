using System;
using System.ServiceModel;

namespace AppDirect.WindowsClient.Common.API
{
    public abstract class AbstractServiceRunner<T> : IAbstractServiceRunner<T>
    {
        private volatile bool _started = false;
        private volatile ICommunicationObject _host;
        private readonly T _service;

        protected AbstractServiceRunner(T service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            _service = service;
        }

        protected virtual ICommunicationObject CreateServiceHost(T service)
        {
            return new ServiceHost(service);
        }

        public virtual void Start()
        {
            _host = CreateServiceHost(_service);
            _host.Open();
            _started = true;
        }

        public virtual void Stop()
        {
            if (!_started)
            {
                throw new InvalidOperationException("Service was not started");
            }

            _started = false;

            if ((_host != null) && (_host.State == CommunicationState.Opened))
            {
                _host.Close();
            }
        }

        public virtual T Service
        {
            get
            {
                return _service;
            }
        }
    }
}