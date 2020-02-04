using System;

namespace BASRemote.Exceptions
{
    [Serializable]
    public sealed class ClientNotStartedException : Exception
    {
        private const string ExceptionMessage = "Request can not be sent. Client is not started. ";

        internal ClientNotStartedException()
            : base(ExceptionMessage)
        {
        }

        internal ClientNotStartedException(string message)
            : base($"{ExceptionMessage}{message}")
        {
        }

        internal ClientNotStartedException(string message, Exception inner)
            : base($"{ExceptionMessage}{message}", inner)
        {
        }
    }
}