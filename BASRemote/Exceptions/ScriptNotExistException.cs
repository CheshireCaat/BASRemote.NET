using System;

namespace BASRemote.Exceptions
{
    [Serializable]
    public sealed class ScriptNotExistException : Exception
    {
        private const string ExceptionMessage = "Script with selected name not exist. ";

        internal ScriptNotExistException()
            : base(ExceptionMessage)
        {
        }

        internal ScriptNotExistException(string message)
            : base($"{ExceptionMessage}{message}")
        {
        }

        internal ScriptNotExistException(string message, Exception inner)
            : base($"{ExceptionMessage}{message}", inner)
        {
        }
    }
}