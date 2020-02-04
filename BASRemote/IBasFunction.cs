using BASRemote.Interfaces;

namespace BASRemote
{
    /// <summary>
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