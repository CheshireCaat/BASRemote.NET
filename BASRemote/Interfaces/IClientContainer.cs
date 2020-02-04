namespace BASRemote.Interfaces
{
    internal interface IClientContainer
    {
        /// <summary>
        ///     Remote client object.
        /// </summary>
        IBasRemoteClient Client { get; }
    }
}