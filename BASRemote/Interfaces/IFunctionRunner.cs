using System;
using System.Threading.Tasks;
using BASRemote.Objects;

namespace BASRemote.Interfaces
{
    public interface IFunctionRunner<out TRunner>
    {
        /// <summary>
        ///     Call the BAS function asynchronously returns the result.
        /// </summary>
        /// <param name="functionName">
        ///     BAS function name as string.
        /// </param>
        /// <param name="functionParams">
        ///     BAS function arguments list.
        /// </param>
        Task<dynamic> RunFunction(string functionName, Params functionParams);

        /// <summary>
        ///     Call the BAS function asynchronously returns the result.
        /// </summary>
        /// <param name="functionName">
        ///     BAS function name as string.
        /// </param>
        /// <param name="functionParams">
        ///     BAS function arguments list.
        /// </param>
        /// <typeparam name="T">
        ///     The type of function result.
        /// </typeparam>
        Task<T> RunFunction<T>(string functionName, Params functionParams);

        /// <summary>
        ///     Call the BAS function asynchronously and returns the promise.
        /// </summary>
        /// <param name="name">
        ///     BAS function name as string.
        /// </param>
        /// <param name="functionParams">
        ///     BAS function arguments list.
        /// </param>
        /// <param name="onResult"></param>
        /// <param name="onError"></param>
        TRunner RunFunctionSync(
            string name,
            Params functionParams,
            Action<dynamic> onResult,
            Action<Exception> onError);
    }
}