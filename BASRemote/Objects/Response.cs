namespace BASRemote.Objects
{
    /// <summary>
    ///     Class that represents default BAS response.
    /// </summary>
    internal sealed class Response
    {
        public string Message { get; set; }

        public dynamic Result { get; set; }

        public bool Success { get; set; }
    }
}