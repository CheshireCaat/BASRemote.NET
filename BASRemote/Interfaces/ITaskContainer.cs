using System.Threading.Tasks;

namespace BASRemote.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITaskContainer
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="TResult">
        ///     The type of task result.
        /// </typeparam>
        Task<TResult> GetTask<TResult>();

        /// <summary>
        /// </summary>
        Task<dynamic> GetTask();
    }
}