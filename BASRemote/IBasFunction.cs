using BASRemote.Interfaces;

namespace BASRemote
{
    /// <summary>
    ///     Basic interface for interacting with BAS functions.
    /// </summary>
    public interface IBasFunction : IFunctionRunner<IBasFunction>
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