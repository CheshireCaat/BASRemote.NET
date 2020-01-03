namespace BASApi.CSharp.Interfaces
{
    public interface IBasFunction : IIdGetter
    {
        object Result();

        /// <summary>
        ///     Stop the function immediately.
        /// </summary>
        void Stop();
    }
}