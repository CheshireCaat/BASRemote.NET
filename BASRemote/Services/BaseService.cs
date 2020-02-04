namespace BASRemote.Services
{
    internal abstract class BaseService
    {
        protected BaseService(Options options)
        {
            Options = options;
        }

        protected Options Options { get; set; }
    }
}