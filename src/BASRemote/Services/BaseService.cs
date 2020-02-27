using System.Threading.Tasks;

namespace BASRemote.Services
{
    /// <summary>
    ///     Base service class.
    /// </summary>
    internal abstract class BaseService
    {
        /// <summary>
        ///     Create an instance of <see cref="BaseService" /> class.
        /// </summary>
        /// <param name="options">
        ///     Client options object.
        /// </param>
        protected BaseService(Options options)
        {
            Options = options;
        }

        /// <summary>
        ///     Client options object.
        /// </summary>
        protected Options Options { get; set; }

        /// <summary>
        ///     Asynchronously start the service with the specified port.
        /// </summary>
        /// <param name="port">
        ///     Selected port number.
        /// </param>
        public abstract Task StartAsync(int port);
    }
}