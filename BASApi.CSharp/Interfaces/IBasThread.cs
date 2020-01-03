namespace BASApi.CSharp.Interfaces
{
    public interface IBasThread : IFunctionRunner, IIdGetter
    {
        /// <summary>
        ///     Check if the thread is already busy with running function.
        /// </summary>
        /// <returns>True if thread is busy, otherwise false.</returns>
        bool IsRunningFunction();

        /// <summary>
        ///     Stop the thread immediately.
        /// </summary>
        void StopThread();
    }
}