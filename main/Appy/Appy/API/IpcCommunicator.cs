using AppDirect.WindowsClient.Common.API;
using System.Collections.Generic;
using System.ServiceModel;

namespace AppDirect.WindowsClient.API
{
    public class IpcCommunicator : IIpcCommunicator
    {
        private readonly object _lockObject = new object();
        private readonly IDictionary<string, MainApplication> _clientsMap = new Dictionary<string, MainApplication>();

        private volatile ServiceHost _host;

        public void Start()
        {
            _host = new ServiceHost(typeof(MainApplication));
            _host.Open();
        }

        public void RegisterClient(string id, MainApplication client)
        {
            lock (_lockObject)
            {
                _clientsMap[id] = client;
            }
        }

        public bool RemoveClient(string id)
        {
            lock (_lockObject)
            {
                return _clientsMap.Remove(id);
            }
        }

        public void CloseAllClients()
        {
            lock (_lockObject)
            {
                foreach (var valuePair in _clientsMap)
                {
                    valuePair.Value.Callback.CloseWindow();
                }

                _clientsMap.Clear();
            }
        }

        private IMainApplicationCallback GetCallbackById(string id)
        {
            IMainApplicationCallback callback = null;

            lock (_lockObject)
            {
                if (!_clientsMap.ContainsKey(id))
                {
                    return null;
                }

                var client = _clientsMap[id];
                if (client != null)
                {
                    callback = client.Callback;
                }
            }

            return callback;
        }

        public bool ActivateBrowserIfExists(string id)
        {
            var callback = GetCallbackById(id);

            if (callback == null)
            {
                return false;
            }

            callback.ActivateWindow();

            return true;
        }

        public void CloseBrowser(string id)
        {
            var callback = GetCallbackById(id);

            if (callback != null)
            {
                callback.CloseWindow();
            }
        }

        public void Exit()
        {
            if (_host != null)
            {
                _host.Close();
            }

            _host = null;
        }
    }
}