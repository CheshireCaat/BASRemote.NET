namespace BASApi.CSharp.Services
{
    internal abstract class BaseService
    {
        protected BaseService(BasRemoteOptions options)
        {
            Options = options;
        }

        public BasRemoteOptions Options { get; }
    }
}