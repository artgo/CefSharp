namespace AppDirect.WindowsClient.API
{
    public interface IIpcCommunicator
    {
        void Start();
        void RegisterClient(string id, MainApplication client);
        bool RemoveClient(string id);
        void CloseAllClients();

        /// <summary>
        /// Returns false if browser window does not exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool ActivateBrowserIfExists(string id);
        void CloseBrowser(string id);
        void Exit();
    }
}