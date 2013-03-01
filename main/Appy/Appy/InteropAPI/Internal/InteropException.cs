using System;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class InteropException : Exception
    {
         public InteropException(string message) : base(message) { }
    }
}