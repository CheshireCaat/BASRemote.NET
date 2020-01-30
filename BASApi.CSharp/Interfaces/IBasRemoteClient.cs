using System.Collections.Generic;
using System.Threading.Tasks;
using BASApi.CSharp.Objects;

namespace BASApi.CSharp.Interfaces
{
    /// <summary>
    ///     Provides methods for interact with BAS.
    /// </summary>
    public interface IBasRemoteClient
    {
        /// <summary>
        /// </summary>
        Task Start();

        void Send(string type, dynamic data, bool isAsync = false);

        Task Async(string type, dynamic data);

        /// <summary>
        /// 
        /// </summary>
        Task Stop();
    }
}