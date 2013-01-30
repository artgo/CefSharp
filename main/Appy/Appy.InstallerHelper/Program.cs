using System.IO.Pipes;
using System.Threading;
using AppDirect.WindowsClient.API;

namespace AppDirect.DesktopClient.InstallerHelper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var ipcCommunicator = new IpcCommunicator();
            //ipcCommunicator.Start();
            //Thread.Sleep(1000);
            //ipcCommunicator.Start();
            var pipeServer = new NamedPipeServerStream("MainApplication", PipeDirection.InOut, 3);
            Thread.Sleep(1000);
            pipeServer.Close();
        }
    }
}
