using System.Collections.Generic;
using BASApi.CSharp.Objects;

namespace BASApi.CSharp.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBasApi : IFunctionRunner
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="async"></param>
        /// <returns></returns>
        int Send(string type, object data, bool async = false);

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IBasBrowser> GetBrowsers();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IBasTask> GetTasks();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void SetGlobalVariable(string name, object value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        void GetGlobalVariable(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isInstant"></param>
        void Stop(bool isInstant);
    }
}