using BASRemote.Objects;

namespace BASRemote
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
        ///     Call the BAS function asynchronously.
        /// </summary>
        /// <param name="functionName">
        ///     BAS function name as string.
        /// </param>
        /// <param name="functionParams">
        ///     BAS function arguments list.
        /// </param>
        TRunner RunFunction(string functionName, Params functionParams);

        /// <summary>
        ///     Call the BAS function asynchronously.
        /// </summary>
        /// <param name="functionName">
        ///     BAS function name as string.
        /// </param>
        TRunner RunFunction(string functionName);
    }
}