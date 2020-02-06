using System;

namespace BASRemote.Exceptions
{
    [Serializable]
    public sealed class AuthenticationException : Exception
    {
        private const string ExceptionMessage = " Unsuccessful authentication. ";

        internal AuthenticationException()
            : base(ExceptionMessage)
        {
        }

        internal AuthenticationException(string message)
            : base($"{ExceptionMessage}{message}")
        {
        }

        internal AuthenticationException(string message, Exception inner)
            : base($"{ExceptionMessage}{message}", inner)
        {
        }
    }
}