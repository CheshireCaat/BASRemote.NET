using System.Threading.Tasks;
using BASRemote.Objects;
using BASRemote.Promises;

namespace BASRemote.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBasThread
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// 
        /// </summary>
        int Id { get; }

        /// <summary>
        ///     Asynchronously calls the BAS function and returns the result.
        /// </summary>
        /// <param name="functionName">
        ///     BAS function name as string.
        /// </param>
        /// <param name="functionParams">
        ///     BAS function arguments list.
        /// </param>
        Task<dynamic> RunFunctionAsync(string functionName, Params functionParams);

        /// <summary>
        ///     Asynchronously calls the BAS function and returns the promise.
        /// </summary>
        /// <param name="functionName">
        ///     BAS function name as string.
        /// </param>
        /// <param name="functionParams">
        ///     BAS function arguments list.
        /// </param>
        IPromise<dynamic> RunFunction(string functionName, Params functionParams);

        void Stop();
    }
}