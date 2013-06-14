using System.ServiceModel;

namespace AppDirect.WindowsClient.Common.API
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IPingable
    {
        /// <summary>
        /// Should return value + 1
        /// </summary>
        /// <param name="value">Random number</param>
        /// <returns>Provided value + 1</returns>
        [OperationContract]
        int Ping(int value);
    }
}