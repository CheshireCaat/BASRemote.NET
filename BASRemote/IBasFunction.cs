using System.Threading.Tasks;
using BASRemote.Interfaces;

namespace BASRemote
{
    /// <summary>
    ///     Basic interface for interacting with BAS functions.
    /// </summary>
    public interface IBasFunction : ITaskContainer
    {
        /// <summary>
        ///     Gets current thread id.
        /// </summary>
        int Id { get; }

        /// <summary>
        ///     Immediately stops function execution.
        /// </summary>
        void Stop();
    }
}