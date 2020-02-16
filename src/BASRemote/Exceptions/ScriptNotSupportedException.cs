using System;

namespace BASRemote.Exceptions
{
    [Serializable]
    public sealed class ScriptNotSupportedException : Exception
    {
        private const string ExceptionMessage = "Script engine not supported (Required 22.4.2 or newer). ";

        internal ScriptNotSupportedException()
            : base(ExceptionMessage)
        {
        }

        internal ScriptNotSupportedException(string message)
            : base($"{ExceptionMessage}{message}")
        {
        }

        internal ScriptNotSupportedException(string message, Exception inner)
            : base($"{ExceptionMessage}{message}", inner)
        {
        }
    }
}