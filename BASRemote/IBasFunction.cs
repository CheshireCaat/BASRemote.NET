using System.Threading.Tasks;

namespace BASRemote
{
    /// <summary>
    ///     Basic interface for interacting with BAS functions.
    /// </summary>
    public interface IBasFunction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult">
        ///
        /// </typeparam>
        Task<TResult> GetTask<TResult>();

        /// <summary>
        /// 
        /// </summary>
        Task<dynamic> GetTask();

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