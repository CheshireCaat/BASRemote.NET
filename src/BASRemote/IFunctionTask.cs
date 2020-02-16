using System.Threading.Tasks;

namespace BASRemote
{
    /// <summary>
    /// </summary>
    public interface IFunctionTask
    {
        /// <summary>
        ///     Get generic task object.
        /// </summary>
        /// <typeparam name="TResult">
        ///     The type of task result.
        /// </typeparam>
        Task<TResult> GetTask<TResult>();

        /// <summary>
        ///     Get default task object.
        /// </summary>
        Task<dynamic> GetTask();
    }
}