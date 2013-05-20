using System;

namespace TaskBarControl
{
    public class InteropException : Exception
    {
         public InteropException(string message) : base(message) { }
    }
}