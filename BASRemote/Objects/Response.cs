namespace BASRemote.Objects
{
    internal sealed class Response
    {
        public string Message { get; set; }

        public dynamic Result { get; set; }

        public bool Success { get; set; }
    }
}