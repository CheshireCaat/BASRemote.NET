namespace BASRunner.CSharp.Interfaces
{
    public interface IIdGetter
    {
        /// <summary>
        ///     Obtain task id from a thread object.
        /// </summary>
        /// <returns>Thread id number.</returns>
        int GetId();
    }
}