using System;

namespace BASRemote.Exceptions
{
    [Serializable]
    public sealed class FunctionException : Exception
    {
        internal FunctionException()
        {
        }

        internal FunctionException(string message)
            : base(message)
        {
        }

        internal FunctionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}