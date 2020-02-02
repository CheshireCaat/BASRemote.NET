using System;

namespace BASRemote.Exceptions
{
    [Serializable]
    public sealed class ScriptNotFoundException : Exception
    {
        private const string ExceptionMessage = "Script with selected name not found.";

        internal ScriptNotFoundException() : base(ExceptionMessage)
        {
        }

        internal ScriptNotFoundException(string message) : base(ExceptionMessage + message)
        {
        }
    }
}