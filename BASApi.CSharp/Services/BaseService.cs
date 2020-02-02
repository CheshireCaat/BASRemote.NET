namespace BASRemote.Services
{
    /// <summary>
    /// 
    /// </summary>
    internal abstract class BaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        protected BaseService(BasRemoteOptions options)
        {
            Options = options;
        }

        public BasRemoteOptions Options { get; }
    }
}