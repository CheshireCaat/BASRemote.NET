using System;

namespace BASRemote.Exceptions
{
    [Serializable]
    public sealed class AlreadyRunningException : Exception
    {
        private const string ExceptionMessage = "Another task is already running. Unable to start a new one. ";

        internal AlreadyRunningException()
            : base(ExceptionMessage)
        {
        }

        internal AlreadyRunningException(string message)
            : base($"{ExceptionMessage}{message}")
        {
        }

        internal AlreadyRunningException(string message, Exception inner)
            : base($"{ExceptionMessage}{message}", inner)
        {
        }
    }
}