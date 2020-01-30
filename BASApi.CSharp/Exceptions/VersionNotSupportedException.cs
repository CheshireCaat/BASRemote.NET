using System;

namespace BASApi.CSharp.Exceptions
{
    [Serializable]
    public class VersionNotSupportedException : Exception
    {
        private const string ExceptionMessage = "Versions below 22.4.2 are not supported.";

        internal VersionNotSupportedException() : base(ExceptionMessage)
        {
        }

        internal VersionNotSupportedException(string message) : base(ExceptionMessage + message)
        {

        }
    }
}
