﻿using System.ServiceModel;

namespace AppDirect.WindowsClient.API
{
    public class IpcCommunicator
    {
        private volatile ServiceHost _host;

        public void Start()
        {
            _host = new ServiceHost(typeof(MainApplication));
            _host.Open();
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
