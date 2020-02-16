using System;

namespace BASRemote.Exceptions
{
    [Serializable]
    public sealed class SocketNotConnectedException : Exception
    {
        private const string ExceptionMessage = "Cannot connect to the WebSocket server. ";

        internal SocketNotConnectedException()
            : base(ExceptionMessage)
        {
        }

        internal SocketNotConnectedException(string message)
            : base($"{ExceptionMessage}{message}")
        {
        }

        internal SocketNotConnectedException(string message, Exception inner)
            : base($"{ExceptionMessage}{message}", inner)
        {
        }
    }
}