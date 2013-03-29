using System;
using System.ServiceModel;

namespace AppDirect.WindowsClient.Common.API
{
    public abstract class AbstractServiceRunner<T> : IAbstractServiceRunner<T>
    {
        private readonly ICommunicationObject _host;
        private readonly T _service;

        protected AbstractServiceRunner(T service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            _service = service;
            _host = CreateServiceHost(_service);
        }

        protected virtual ICommunicationObject CreateServiceHost(T service)
        {
            return new ServiceHost(service);
        }

        public virtual void Start()
        {
            _host.Open();
        }

        public virtual void Stop()
        {
            if (_host.State == CommunicationState.Opened)
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