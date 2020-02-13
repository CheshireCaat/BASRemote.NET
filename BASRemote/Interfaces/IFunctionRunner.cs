using BASRemote.Objects;

namespace BASRemote.Interfaces
{
    /// <summary>
    ///     Basic interface for running BAS functions.
    /// </summary>
    /// <typeparam name="TRunner">
    ///     Type that implements this interface.
    /// </typeparam>
    public interface IFunctionRunner<out TRunner>
    {
        /// <summary>
        ///     Call the BAS function asynchronously and get the result.
        /// </summary>
        /// <param name="functionName">
        ///     BAS function name as string.
        /// </param>
        /// <param name="functionParams">
        ///     BAS function arguments list.
        /// </param>
        /// <typeparam name="TResult">
        ///     The type of function result.
        /// </typeparam>
        TRunner RunFunction<TResult>(string functionName, Params functionParams);

        /// <summary>
        ///     Call the BAS function asynchronously and get the result.
        /// </summary>
        /// <param name="functionName">
        ///     BAS function name as string.
        /// </param>
        /// <param name="functionParams">
        ///     BAS function arguments list.
        /// </param>
        TRunner RunFunction(string functionName, Params functionParams);
    }
}