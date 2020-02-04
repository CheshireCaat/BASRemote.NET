using System;
using System.Threading.Tasks;

namespace BASRemote.Services
{
    /// <summary>
    ///     Provides methods for interacting with BAS engine.
    /// </summary>
    internal interface IEngineService : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        event Action OnExtractStarted;

        /// <summary>
        /// 
        /// </summary>
        event Action OnExtractEnded;

        /// <summary>
        /// 
        /// </summary>
        event Action OnDownloadStarted;

        /// <summary>
        /// 
        /// </summary>
        event Action OnDownloadEnded;

        /// <summary>
        ///     Asynchronously start the engine with the specified port.
        /// </summary>
        /// <param name="port">
        ///     Selected port number.
        /// </param>
        Task StartEngineAsync(int port);

        /// <summary>
        /// 
        /// </summary>
        Task InitializeAsync();
    }
}