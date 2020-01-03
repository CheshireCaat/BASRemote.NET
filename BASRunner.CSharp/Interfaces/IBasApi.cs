namespace BASRunner.CSharp.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBasApi : IFunctionRunner
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        void Init(int port);

        /// <summary>
        ///     Show browser window for specific thread.
        /// </summary>
        /// <param name="browserId">Unique browser identifier.</param>
        void ShowBrowser(int browserId);

        /// <summary>
        ///     Hide browser window for specific thread.
        /// </summary>
        /// <param name="browserId">Unique browser identifier.</param>
        void HideBrowser(int browserId);

        object GetTasks();
    }
}